using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelectIcon : MonoBehaviour
{
    [SerializeField] Image image;
    Network network;
    UnitType type;

    public void SetData(UnitType type, Network network)
    {
        this.type = type;
        this.network = network;
        image.sprite = UnitTable.Instance.GetUnitModel(type).Icon;
    }

    public void IconClick()
    {
        network.SetUnitType(type);
    }
}
