using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopInventoryIcon_Scr : InventoryIcon
{
    public TMP_Text priceText, descriptionText;
    protected override void Start()
    {
        base.Start();
        quantityText.gameObject.SetActive(false);
    }

    public override void SetIconVisuals(InventorySlot slotData)
    {
        base.SetIconVisuals(slotData);
        if (slotData == null) return;
        priceText.text = slotData.GetItem().storePrice.ToString();
        descriptionText.text = slotData.GetItem().itemDescription;
    }
}
