using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MapBuilder : MonoBehaviour {
    [SerializeField] GameObject blockPrefab;
    [SerializeField] Material material;
    List<GameObject> blockInstances = new List<GameObject>();
    // Use this for initialization
    void Start()
    {
        CreateChild();
        //Combine();
    }

    void CreateChild()
    {
        string path = GameObject.Find("Game").GetComponent<Game>().GetConfig().GetString("Map");

        StreamReader sr = new StreamReader(path);
        string json = sr.ReadToEnd();
        sr.Close();

        MapData data = JsonUtility.FromJson<MapData>(json);
        for (int x = 0; x < data.width; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                bool isBlock = data.blocks[x + y * data.width];

                if (isBlock && x != 0 && x != data.width - 1 && y != 0 && y != data.height - 1)
                {
                    GameObject block = Instantiate(blockPrefab, transform);
                    block.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
                    blockInstances.Add(block);
                }
            }
        }
    }

    void Combine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        GetComponent<MeshRenderer>().material = material;

        blockInstances.ForEach(x => Destroy(x));
        blockInstances.Clear();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
