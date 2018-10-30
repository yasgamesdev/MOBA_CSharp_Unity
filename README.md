# MOBA_CSharp_Unity
Framework for MOBA games. The server does not depend on Unity. Run on Linux.
## Features
- Client and Server written C#.
- Framework does not depend on Unity.
- The server is just a console application. So run on Linux with Mono.
- First collision detection using Uniform Grid.
- Pathfinding using Navigation Mesh.
- Sight shared by team.
- Fog of War.
## Quick Setup
### Client
1. Open `MOBA_CSharp_Unity_Client` with Unity.
2. Build
3. Copy `MOBA_CSharp_Unity_Client/ClientConfig.txt` and `MOBA_CSharp_Unity_Client/map.json` to build folder.
4. Open `ClientConfig.txt`. And modify `Host` and `Port`.
### Server
1. Open `MOBA_CSharp_Server/MOBA_CSharp_Server.sln` with Visual Studio or MonoDevelop.
2. Build
3. Open `ServerConfig.txt` in the build folder. And modify `Port`.
## Software
- Language: C#
- UDP: [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp)
- Serialization: [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp)
- Geometry: [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite)
- Collision: own program
- Pathfinding: [SharpNav](https://github.com/Robmaister/SharpNav)
- Fog of War: [Fog-of-war](https://github.com/LeLocTai/Fog-of-war)
- Client Game Engine: [Unity](https://unity3d.com)