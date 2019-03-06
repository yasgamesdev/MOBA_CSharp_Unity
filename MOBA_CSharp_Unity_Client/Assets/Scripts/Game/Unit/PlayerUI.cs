using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Slider hpSlider, mpSlider, expSlider;
    [SerializeField] Image hpImage;
    [SerializeField] TextMeshProUGUI hpText, mpText;
    [SerializeField] Text goldText;

    [SerializeField] RectTransform effectTransform;
    [SerializeField] GameObject iconPrefab;

    List<GameObject> effectIconInstances = new List<GameObject>();

    [SerializeField] List<GameObject> itemsInstance;
    [SerializeField] List<GameObject> skillsInstance;

    // Start is called before the first frame update
    void Start()
    {
        int effectIconNum = 13;
        for(int i=0; i<effectIconNum; i++)
        {
            GameObject effectIcon = Instantiate(iconPrefab, effectTransform);
            effectIcon.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            effectIcon.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            effectIcon.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            effectIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 50f, 0);
            effectIcon.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
            effectIcon.SetActive(false);
            effectIconInstances.Add(effectIcon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerData(PlayerObj playerObj, ChampionObj championObj)
    {
        switch (championObj.Team)
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

        hpSlider.value = championObj.CurHP / championObj.MaxHP;
        mpSlider.value = championObj.CurMP / championObj.MaxMP;
        expSlider.value = playerObj.Exp / playerObj.NextExp;

        hpText.text = championObj.CurHP.ToString("F0") + "/" + championObj.MaxHP.ToString("F0");
        mpText.text = championObj.CurMP.ToString("F0") + "/" + championObj.MaxMP.ToString("F0");
        goldText.text = playerObj.Gold.ToString("F0") + "G";

        for(int i=0; i<effectIconInstances.Count; i++)
        {
            if(i < playerObj.Effects.Length)
            {
                effectIconInstances[i].SetActive(true);
                effectIconInstances[i].GetComponent<Icon>().SetCombatObj(playerObj.Effects[i]);
            }
            else
            {
                effectIconInstances[i].SetActive(false);
            }
        }

        for (int i = 0; i < itemsInstance.Count; i++)
        {
            if (playerObj.Items.ToList().Any(x => x.SlotNum == i))
            {
                itemsInstance[i].SetActive(true);
                itemsInstance[i].GetComponent<Icon>().SetCombatObj(playerObj.Items.ToList().First(x => x.SlotNum == i));
            }
            else
            {
                itemsInstance[i].SetActive(false);
            }
        }
        foreach(CombatObj item in playerObj.Items)
        {
            itemsInstance[item.SlotNum].GetComponent<Icon>().SetCombatObj(item);
        }

        for(int i=0; i<skillsInstance.Count; i++)
        {
            if(playerObj.Skills.ToList().Any(x => x.SlotNum == i))
            {
                skillsInstance[i].SetActive(true);
                skillsInstance[i].GetComponent<Icon>().SetCombatObj(playerObj.Skills.ToList().First(x => x.SlotNum == i));
            }
            else
            {
                skillsInstance[i].SetActive(false);
            }
        }
    }
}
