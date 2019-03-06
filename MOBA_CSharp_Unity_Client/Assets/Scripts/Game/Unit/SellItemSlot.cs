using MessagePack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellItemSlot : MonoBehaviour
{
    public int SlotNum;
    Network network;
    [SerializeField] Image iconImage;
    [SerializeField] Text sellingPriceText;
    [SerializeField] Button sellButton;
    
    void Start()
    {
        network = GameObject.Find("Network").GetComponent<Network>();
    }

    public void SetButtonInteractable(bool interactable)
    {
        sellButton.interactable = interactable;
    }

    public void SetData(Sprite sprite, float sellingPrice)
    {
        iconImage.sprite = sprite;
        sellingPriceText.text = sellingPrice.ToString("F0") + "G";
    }

    public void SellItem()
    {
        network.Send(MessageType.SellItem, MessagePackSerializer.Serialize((byte)SlotNum), ENet.PacketFlags.Reliable);
    }
}
