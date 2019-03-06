using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemIcon : MonoBehaviour
{
    ItemUI itemUI;
    CombatType type;
    [SerializeField] Image iconImage;
    [SerializeField] Text buyingPriceText;

    public void SetData(ItemUI itemUI, CombatType type, Sprite sprite, float buyingPrice)
    {
        this.itemUI = itemUI;
        this.type = type;
        iconImage.sprite = sprite;
        buyingPriceText.text = buyingPrice.ToString("F0") + "G";
    }

    public void BuyItemIconClicked()
    {
        itemUI.BuyItemIconClicked(type);
    }
}
