using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionDot : MonoBehaviour
{
    [SerializeField] Image outline, avatar;

    public void SetData(UnitType type, Team team, Vector2 position)
    {
        avatar.sprite = UnitTable.Instance.GetUnitModel(type).Icon;

        switch (team)
        {
            case Team.Blue:
                outline.color = new Color(0, 0, 1);
                break;
            case Team.Red:
                outline.color = new Color(1, 0, 0);
                break;
            default:
                outline.color = new Color(1, 1, 0);
                break;
        }

        float x = position.x / MinimapUI.MapWidth * MinimapUI.MinimapWidth;
        float y = position.y / MinimapUI.MapHeight * MinimapUI.MinimapHeight;

        GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
}
