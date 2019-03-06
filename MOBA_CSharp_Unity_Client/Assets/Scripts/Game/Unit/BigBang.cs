using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBang : MonoBehaviour
{
    [SerializeField] GameObject sphere, particle;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        var yamlObject = GameObject.Find("Network").GetComponent<Network>().GetYAMLObject(CombatType.BigBang);
        timer = yamlObject.GetData<float>("Distance") / yamlObject.GetData<float>("Speed");
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            sphere.SetActive(false);
            particle.SetActive(true);
        }
    }
}
