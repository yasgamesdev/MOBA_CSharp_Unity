using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralDot : MonoBehaviour
{
    [SerializeField] Image dot;

    public void SetData(Team team, Vector2 position)
    {
        switch (team)
        {
            case Team.Blue:
                dot.color = new Color(0, 0, 1);
                break;
            case Team.Red:
                dot.color = new Color(1, 0, 0);
                break;
            default:
                dot.color = new Color(1, 1, 0);
                break;
        }

        float x = position.x / MinimapUI.MapWidth * MinimapUI.MinimapWidth;
        float y = position.y / MinimapUI.MapHeight * MinimapUI.MinimapHeight;

        GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
    }
}
