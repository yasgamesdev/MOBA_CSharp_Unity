using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientUINode : MonoBehaviour
{
    [SerializeField] Image teamColorImage, iconImage;
    [SerializeField] Text nameText, levelText, unitTypeText;

    public void SetData(UnitType type, Team team, string name, int level)
    {
        if(team == Team.Blue)
        {
            teamColorImage.color = new Color(0, 0, 1);
        }
        else
        {
            teamColorImage.color = new Color(1, 0, 0);
        }

        iconImage.sprite = UnitTable.Instance.GetUnitModel(type).Icon;
        nameText.text = name;
        levelText.text = "Lv" + level.ToString();
        unitTypeText.text = type.ToString();
    }
}
