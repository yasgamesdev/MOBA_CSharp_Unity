# MOBA_CSharp_Unity
Framework for MOBA games. The server doesn't depend on Unity. Run programs on Linux.
[![Doujin Allstars Release Trailer: Open-source MOBA](https://img.youtube.com/vi/-eD6KAgt7co/0.jpg)](https://www.youtube.com/watch?v=-eD6KAgt7co)
## Features
- Client and Server written in C#.
- The code is only 3000 lines.
- Framework doesn't depend on Unity.
- The server is just a console application. So run programs on Linux with Mono.
- Pathfinding using Navigation Mesh.
- Sight is shared by team.
- Bush
- Scriptable Heroes & Skills & Buffs & Items.
## Quick Setup
### Client
1. Open `MOBA_CSharp_Unity_Client` with Unity.
2. Build
3. Copy `MOBA_CSharp_Unity_Client/YAML` and `MOBA_CSharp_Unity_Client/CSV` to build folder.
4. Open `YAML/ClientConfig.yml`. And modify `Host` and `Port`.
### Server
1. Open `MOBA_CSharp_Server/MOBA_CSharp_Server.sln` with Visual Studio or MonoDevelop.
2. Build
3. Open `YAML/ServerConfig.yml` in the build folder. And modify `Port`.
## References
### Games
- Inspired By [League of Legends](https://na.leagueoflegends.com/)
- Inspired By [Heroes of the Storm](https://heroesofthestorm.com/)
### Software
- Language: C#
- UDP: [ENet-CSharp](https://github.com/nxrighthere/ENet-CSharp)
- Serialization: [MessagePack-CSharp](https://github.com/neuecc/MessagePack-CSharp)
- Physics: [VelcroPhysics](https://github.com/VelcroPhysics/VelcroPhysics)
- Pathfinding: [SharpNav](https://github.com/Robmaister/SharpNav)
- CSV: [CsvHelper](https://joshclose.github.io/CsvHelper/)
- Json: [Json.NET](https://www.newtonsoft.com/json)
- YAML: [YamlDotNet](https://github.com/aaubry/YamlDotNet)
- Client Game Engine: [Unity 2018.3.4f1](https://unity3d.com)
## Q&A
### How can I change the parameters of the unit?
Open `MOBA_CSharp_Server\MOBA_CSharp_Server\CSV\ExpTables` and modify CSV files.
### How can I change the parameters of the item?
Open `MOBA_CSharp_Server\MOBA_CSharp_Server\CSV\Items.csv` and modify it.
