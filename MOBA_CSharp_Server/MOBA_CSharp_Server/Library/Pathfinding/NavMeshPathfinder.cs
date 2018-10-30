using SharpNav;
using SharpNav.IO.Json;
using SharpNav.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using SVector3 = SharpNav.Geometry.Vector3;

namespace Pathfinding
{
    public class NavMeshPathfinder
    {
        TiledNavMesh tiledNavMesh;
        NavMeshQuery navMeshQuery;

        public void Load(string path)
        {
            tiledNavMesh = new NavMeshJsonSerializer().Deserialize(path);
            navMeshQuery = new NavMeshQuery(tiledNavMesh, 2048);
        }

        public Vector2[] GetPath(Vector2 start, Vector2 end)
        {
            NavPoint startPt = navMeshQuery.FindNearestPoly(new SVector3(-start.X, 0, start.Y), new SharpNav.Geometry.Vector3(2f, 2f, 2f));
            NavPoint endPt = navMeshQuery.FindNearestPoly(new SVector3(-end.X, 0, end.Y), new SharpNav.Geometry.Vector3(2f, 2f, 2f));
            Path path = new Path();
            navMeshQuery.FindPath(ref startPt, ref endPt, new NavQueryFilter(), path);

            List<SVector3> smoothPath;

            //find a smooth path over the mesh surface
            int npolys = path.Count;
            SVector3 iterPos = new SVector3();
            SVector3 targetPos = new SVector3();
            navMeshQuery.ClosestPointOnPoly(startPt.Polygon, startPt.Position, ref iterPos);
            navMeshQuery.ClosestPointOnPoly(path[npolys - 1], endPt.Position, ref targetPos);

            smoothPath = new List<SVector3>(2048);
            smoothPath.Add(iterPos);

            float STEP_SIZE = 0.5f;
            float SLOP = 0.01f;
            while (npolys > 0 && smoothPath.Count < smoothPath.Capacity)
            {
                //find location to steer towards
                SVector3 steerPos = new SVector3();
                StraightPathFlags steerPosFlag = 0;
                NavPolyId steerPosRef = NavPolyId.Null;

                if (!GetSteerTarget(navMeshQuery, iterPos, targetPos, SLOP, path, ref steerPos, ref steerPosFlag, ref steerPosRef))
                    break;

                bool endOfPath = (steerPosFlag & StraightPathFlags.End) != 0 ? true : false;
                bool offMeshConnection = (steerPosFlag & StraightPathFlags.OffMeshConnection) != 0 ? true : false;

                //find movement delta
                SVector3 delta = steerPos - iterPos;
                float len = (float)Math.Sqrt(SVector3.Dot(delta, delta));

                //if steer target is at end of path or off-mesh link
                //don't move past location
                if ((endOfPath || offMeshConnection) && len < STEP_SIZE)
                    len = 1;
                else
                    len = STEP_SIZE / len;

                SVector3 moveTgt = new SVector3();
                VMad(ref moveTgt, iterPos, delta, len);

                //move
                SVector3 result = new SVector3();
                List<NavPolyId> visited = new List<NavPolyId>(16);
                NavPoint startPoint = new NavPoint(path[0], iterPos);
                navMeshQuery.MoveAlongSurface(ref startPoint, ref moveTgt, out result, visited);
                path.FixupCorridor(visited);
                npolys = path.Count;
                float h = 0;
                navMeshQuery.GetPolyHeight(path[0], result, ref h);
                result.Y = h;
                iterPos = result;

                //handle end of path when close enough
                if (endOfPath && InRange(iterPos, steerPos, SLOP, 1.0f))
                {
                    //reached end of path
                    iterPos = targetPos;
                    if (smoothPath.Count < smoothPath.Capacity)
                    {
                        smoothPath.Add(iterPos);
                    }
                    break;
                }

                //store results
                if (smoothPath.Count < smoothPath.Capacity)
                {
                    smoothPath.Add(iterPos);
                }
            }

            return smoothPath.Select(x => new Vector2(-x.X, x.Z)).ToArray();
        }

        private bool GetSteerTarget(NavMeshQuery navMeshQuery, SVector3 startPos, SVector3 endPos, float minTargetDist, SharpNav.Pathfinding.Path path,
            ref SVector3 steerPos, ref StraightPathFlags steerPosFlag, ref NavPolyId steerPosRef)
        {
            StraightPath steerPath = new StraightPath();
            navMeshQuery.FindStraightPath(startPos, endPos, path, steerPath, 0);
            int nsteerPath = steerPath.Count;
            if (nsteerPath == 0)
                return false;

            //find vertex far enough to steer to
            int ns = 0;
            while (ns < nsteerPath)
            {
                if ((steerPath[ns].Flags & StraightPathFlags.OffMeshConnection) != 0 ||
                    !InRange(steerPath[ns].Point.Position, startPos, minTargetDist, 1000.0f))
                    break;

                ns++;
            }

            //failed to find good point to steer to
            if (ns >= nsteerPath)
                return false;

            steerPos = steerPath[ns].Point.Position;
            steerPos.Y = startPos.Y;
            steerPosFlag = steerPath[ns].Flags;
            if (steerPosFlag == StraightPathFlags.None && ns == (nsteerPath - 1))
                steerPosFlag = StraightPathFlags.End; // otherwise seeks path infinitely!!!
            steerPosRef = steerPath[ns].Point.Polygon;

            return true;
        }

        private void VMad(ref SVector3 dest, SVector3 v1, SVector3 v2, float s)
        {
            dest.X = v1.X + v2.X * s;
            dest.Y = v1.Y + v2.Y * s;
            dest.Z = v1.Z + v2.Z * s;
        }

        private bool InRange(SVector3 v1, SVector3 v2, float r, float h)
        {
            float dx = v2.X - v1.X;
            float dy = v2.Y - v1.Y;
            float dz = v2.Z - v1.Z;
            return (dx * dx + dz * dz) < (r * r) && Math.Abs(dy) < h;
        }
    }
}
