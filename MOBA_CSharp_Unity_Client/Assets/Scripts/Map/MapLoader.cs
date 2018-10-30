using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour {
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] Transform ground, left, right, top, bottom;
    [SerializeField] Transform collisionRoot;
    [SerializeField] string loadFileName = "map";
    [SerializeField] GameObject edgePrefab, circlePrefab, polyPrefab;
    // Use this for initialization
    void Start () {
        Load();
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
}
