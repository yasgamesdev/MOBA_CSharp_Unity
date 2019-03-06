using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChampionHealthBar : MonoBehaviour
{
    [SerializeField] Text displayName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] Slider hpSlider, mpSlider;
    [SerializeField] Image hpImage;
    
    public void SetData(Vector3 worldPos, string displayName, int level, float maxHP, float curHP, float maxMP, float curMP, Team team)
    {
        GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos) + new Vector2(0, 20);

        this.displayName.text = displayName;
        this.level.text = level.ToString();
        hpSlider.value = curHP / maxHP;
        mpSlider.value = curMP / maxMP;
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
