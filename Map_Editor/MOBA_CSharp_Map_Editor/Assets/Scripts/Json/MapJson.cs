using System;

[Serializable]
public class MapJson
{
    public int width;
    public int height;
    public EdgeJson[] edges;
    public CircleJson[] circles;
    public PolyJson[] polies;
}