using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper_Scr : ScaledObject_Scr, Interactable
{
    public void Interact(GameObject player)
    {
        if (ShopKeeperUIManager.instance.IsActive()) return;

        ShopKeeperUIManager.instance.ShowShopMenu();
        
    }
}
