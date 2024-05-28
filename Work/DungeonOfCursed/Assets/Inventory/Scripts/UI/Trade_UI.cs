using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_UI : MonoBehaviour
{
    public Button[] t_ItemSlot;
    public Text[] txt;


    public void SetItemImg(Sprite icon, int i = 0)
    {
        t_ItemSlot[i].image.sprite = icon;
    }

    public void SetGold(ItemData itemData, int i = 0)
    {
        txt[i].text = itemData.Price.ToString();
    }
}
