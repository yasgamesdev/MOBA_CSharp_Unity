using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingModel : MonoBehaviour
{
    [SerializeField] Renderer sphereRender;
    [SerializeField] Transform sphereTransform;
    Unit unit;
    bool destroyed;
    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponentsInParent<Unit>()[0];
        destroyed = false;

        if(((BuildingObj)unit.Info).Team == Team.Blue)
        {
            sphereRender.material.color = new Color(0, 0, 1f);
        }
        else
        {
            sphereRender.material.color = new Color(1f, 0, 0);
        }

        SetModel();
    }

    // Update is called once per frame
    void Update()
    {
        SetModel();
    }

    void SetModel()
    {
        if(!destroyed && ((BuildingObj)unit.Info).CurHP <= 0)
        {
            destroyed = true;
            sphereTransform.gameObject.SetActive(false);
        }
    }
}
