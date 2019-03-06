using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] Image hpImage;
    
    public void SetData(Vector3 worldPos, float maxHP, float curHP, Team team)
    {
        GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos) + new Vector2(0, 20);

        hpSlider.value = curHP / maxHP;
        switch(team)
        {
            case Team.Blue:
                hpImage.color = new Color(0, 0, 1);
                break;
            case Team.Red:
                hpImage.color = new Color(1, 0, 0);
                break;
            default:
                hpImage.color = new Color(1, 1, 0);
                break;
        }
    }
}
