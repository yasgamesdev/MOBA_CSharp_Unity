using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNode : MonoBehaviour
{
    [SerializeField] Image outline, icon, readyIcon;
    [SerializeField] Sprite readySprite, notReadySprite;
    [SerializeField] Text nameText;

    public void SetData(UnitType type, Team team, string name, bool ready)
    {
        icon.sprite = UnitTable.Instance.GetUnitModel(type).Icon;

        if(team == Team.Blue)
        {
            outline.color = new Color(0, 0, 1);
        }
        else
        {
            outline.color = new Color(1, 0, 0);
        }

        nameText.text = name;

        if(ready)
        {
            readyIcon.sprite = readySprite;
        }
        else
        {
            readyIcon.sprite = notReadySprite;
        }
    }
}
