using Newtonsoft.Json;
using SharpNav;
using SharpNav.CLI;
using SharpNav.Geometry;
using SharpNav.IO;
using SharpNav.IO.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapBuilder : MonoBehaviour {
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Transform ground, left, right, top, bottom;
    [SerializeField] Transform collisionRoot;
    [SerializeField] string saveFileName, loadFileName;
    [SerializeField] GameObject edgePrefab, circlePrefab, polyPrefab;
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        SetCollision();
    }

    void SetCollision()
    {
        ground.position = new UnityEngine.Vector3(width * 0.5f, 0, height * 0.5f);
        ground.localScale = new UnityEngine.Vector3(width * 0.1f, 1f, height * 0.1f);

        left.position = new UnityEngine.Vector3(0, 0, height * 0.5f);
        left.eulerAngles = new UnityEngine.Vector3(0, 90f, 0);
        left.localScale = new UnityEngine.Vector3(height, 1f, 1f);

        right.position = new UnityEngine.Vector3(width, 0, height * 0.5f);
        right.eulerAngles = new UnityEngine.Vector3(0, 90f, 0);
        right.localScale = new UnityEngine.Vector3(height, 1f, 1f);

        top.position = new UnityEngine.Vector3(width * 0.5f, 0, height);
        top.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
        top.localScale = new UnityEngine.Vector3(width, 1f, 1f);

        bottom.position = new UnityEngine.Vector3(width * 0.5f, 0, 0);
        bottom.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
        bottom.localScale = new UnityEngine.Vector3(width, 1f, 1f);
    }

    public List<GameObject> GetGameObjects(string tag, Transform target)
    {
        List<GameObject> list = new List<GameObject>();

        for (int i = 0; i < target.childCount; i++)
        {
            list.AddRange(GetGameObjects(tag, target.GetChild(i)));
        }

        if (target.tag == tag)
        {
            list.Add(target.gameObject);
        }

        return list;
    }

    public void Save()
    {
        var edges = GetGameObjects("Edge", collisionRoot);
        var circles = GetGameObjects("Circle", collisionRoot);
        var rects = GetGameObjects("Rect", collisionRoot);
        var polies = GetGameObjects("Poly", collisionRoot);

        List<PolyJson> polyJsons = new List<PolyJson>();
        polyJsons.AddRange(rects.Select(x => x.GetComponent<RectBuilder>().GetJson()));
        polyJsons.AddRange(polies.Select(x => x.GetComponent<PolyBuilder>().GetJson()));

        var map = new MapJson()
        {
            width = width,
            height = height,
            edges = edges.Select(x => x.GetComponent<EdgeBuilder>().GetJson()).ToArray(),
            circles = circles.Select(x => x.GetComponent<CircleBuilder>().GetJson()).ToArray(),
            polies = polyJsons.ToArray()
        };

        string json = JsonConvert.SerializeObject(map);

        StreamWriter sw = new StreamWriter(saveFileName + ".json", false);
        sw.WriteLine(json);
        sw.Flush();

        ExportObj(loadFileName);
        GenerateNavMesh(loadFileName);
    }

    public void Load()
    {
        StreamReader sr = new StreamReader(loadFileName + ".json");
        string json = sr.ReadToEnd();
        sr.Close();

        var map = JsonConvert.DeserializeObject<MapJson>(json);

        width = map.width;
        height = map.height;

        for(int i=0; i<collisionRoot.transform.childCount; i++)
        {
            Destroy(collisionRoot.transform.GetChild(i).gameObject);
        }

        foreach(var edge in map.edges)
        {
            GameObject instance = Instantiate(edgePrefab, collisionRoot);
            instance.GetComponent<EdgeBuilder>().Init(edge);
        }

        foreach (var circle in map.circles)
        {
            GameObject instance = Instantiate(circlePrefab, collisionRoot);
            instance.GetComponent<CircleBuilder>().Init(circle);
        }

        foreach (var poly in map.polies)
        {
            GameObject instance = Instantiate(polyPrefab, collisionRoot);
            instance.GetComponent<PolyBuilder>().Init(poly);
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    void ExportObj(string fileName)
    {
        Terrain terrain = UnityEngine.Object.FindObjectOfType<Terrain>();
        MeshFilter[] mfs = UnityEngine.Object.FindObjectsOfType<MeshFilter>();
        SkinnedMeshRenderer[] smrs = UnityEngine.Object.FindObjectsOfType<SkinnedMeshRenderer>();
        ExportScene.ExportSceneToObj(fileName + ".obj", terrain, mfs, smrs, false, true);
    }

    void GenerateNavMesh(string fileName)
    {
        NavMeshConfigurationFile file = new NavMeshConfigurationFile();
        file.GenerationSettings.AgentRadius = 0.31f;
        file.ExportPath = fileName + ".snj";
        NavMeshConfigurationFile.MeshSettings loadMesh = new NavMeshConfigurationFile.MeshSettings() { Path = fileName + ".obj", Position = new float[3], Scale = 1.0f };
        file.InputMeshes.Add(loadMesh);

        List<string> meshes = new List<string>();
        List<ObjModel> models = new List<ObjModel>();

        foreach (var mesh in file.InputMeshes)
        {
            //Log.("Path:  " + mesh.Path, 2);
            //Log.Debug("Scale: " + mesh.Scale, 2);
            //Log.Debug("Position: " + mesh.Position.ToString(), 2);
            meshes.Add(mesh.Path);

            SharpNav.Geometry.Vector3 position = new SharpNav.Geometry.Vector3(mesh.Position[0], mesh.Position[1], mesh.Position[2]);

            if (File.Exists(mesh.Path))
            {
                ObjModel obj = new ObjModel(mesh.Path);
                float scale = mesh.Scale;
                //TODO SCALE THE OBJ FILE
                models.Add(obj);
            }
            else
            {
                //Log.Error("Mesh file does not exist.");
                return;
            }

        }

        var tris = Enumerable.Empty<Triangle3>();
        foreach (var model in models)
            tris = tris.Concat(model.GetTriangles());

        TiledNavMesh navmesh = NavMesh.Generate(tris, file.GenerationSettings);

        new NavMeshJsonSerializer().Serialize(file.ExportPath, navmesh);
    }
}
