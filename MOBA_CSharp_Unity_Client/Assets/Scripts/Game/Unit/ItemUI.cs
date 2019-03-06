using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] List<SellItemSlot> sellItemSlots;
    [SerializeField] Button buyButton;

    //Buy
    [SerializeField] GameObject buyItemIconPrefab;
    [SerializeField] RectTransform buyItemIconTransform;
    CombatType itemType;
    Network network;

    //Detail
    [SerializeField] Image detailIconImage;
    [SerializeField] Text itemNameText, buyingPriceText, descriptionText;

    // Start is called before the first frame update
    void Start()
    {
        network = GameObject.Find("Network").GetComponent<Network>();

        int count = 0;
        float width = 100f;
        float height = 100f;
        foreach (CombatType type in Enum.GetValues(typeof(CombatType)))
        {
            if(type < CombatType.Potion)
            {
                continue;
            }

            int x = count % 7;
            int y = count / 7;

            count++;

            float xpos = x * width;
            float ypos = -y * height;

            GameObject instance = Instantiate(buyItemIconPrefab, buyItemIconTransform);
            instance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            instance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            instance.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(xpos, ypos);

            instance.GetComponent<BuyItemIcon>().SetData(this, type, CombatTable.Instance.GetCombatData(type).Sprite, network.GetItemTable(type).BuyingPrice);
        }

        BuyItemIconClicked(CombatType.Potion);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerData(PlayerObj playerObj, ChampionObj championObj)
    {
        for(int i=0; i<sellItemSlots.Count; i++)
        {
            if (playerObj.Items.ToList().Any(x => x.SlotNum == i))
            {
                sellItemSlots[i].gameObject.SetActive(true);
                CombatObj itemObj = playerObj.Items.ToList().First(x => x.SlotNum == i);
                sellItemSlots[i].SetData(CombatTable.Instance.GetCombatData(itemObj.Type).Sprite, network.GetItemTable(itemObj.Type).SellingPrice);
            }
            else
            {
                sellItemSlots[i].gameObject.SetActive(false);
            }
        }

        goldText.text = playerObj.Gold.ToString("F0") + "G";

        if(playerObj.Effects.Any(x => x.Type == CombatType.OnBase))
        {
            sellItemSlots.ForEach(x => x.SetButtonInteractable(true));
            buyButton.interactable = true;
        }
        else
        {
            sellItemSlots.ForEach(x => x.SetButtonInteractable(false));
            buyButton.interactable = false;
        }
    }

    public void BuyItemIconClicked(CombatType type)
    {
        itemType = type;

        ItemTable itemTable = network.GetItemTable(type); 

        detailIconImage.sprite = CombatTable.Instance.GetCombatData(type).Sprite;
        itemNameText.text = itemTable.ItemName;
        buyingPriceText.text = itemTable.BuyingPrice.ToString("F0") + "G";
        descriptionText.text = itemTable.Description;
    }

    public void BuyItemButtonClicked()
    {
        network.Send(MessageType.BuyItem, MessagePackSerializer.Serialize(itemType), ENet.PacketFlags.Reliable);
    }
}
