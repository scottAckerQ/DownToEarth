using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ShippingBox_Scr : ScaledObject_Scr, Interactable
{
    public int _shippingValue;
    public Inventory shippingInventory;
    private Inventory _playerInventory;

    public TMP_Text moneyText;
    
    private const int MaxItems = 36;

    // Start is called before the first frame update
    private void Start()
    {
        _playerInventory = GameController.Instance.playerInventory;

        if(DayManager.Instance.HasSavedShippingToday())
        {
            shippingInventory = DayManager.Instance.GetSavedShippingInventory();
            _shippingValue = DayManager.Instance.GetSavedShippingValue();
        }
        else
            shippingInventory = new Inventory(MaxItems);
    }

    public void ShipItem(int itemIndex)
    {
        InventorySlot thisSlot = _playerInventory.GetSlotAt(itemIndex);
        if (thisSlot.IsEmptySlot()) return;
        
        Item thisItem = thisSlot.GetItem();
        
        _shippingValue += thisItem.sellPrice * thisSlot.Quantity;
        
        shippingInventory.AddItem(thisItem.itemID,thisSlot.Quantity);
        _playerInventory.RemoveIndexItemStack(itemIndex);
        
        UpdateMoneyText();
    }

    public void UnshipItem(int itemIndex)
    {
        InventorySlot thisSlot = shippingInventory.GetSlotAt(itemIndex);
        if (thisSlot.IsEmptySlot()) return;
        
        Item thisItem = thisSlot.GetItem();

        _shippingValue -= thisItem.sellPrice * thisSlot.Quantity;
        
        _playerInventory.AddItem(thisItem.itemID,thisSlot.Quantity);
        shippingInventory.RemoveIndexItemStack(itemIndex);
        UpdateMoneyText();
    }

    public void ShowMoneyText()
    {
        moneyText.gameObject.SetActive(true);
        UpdateMoneyText();
    }
    
    public void HideMoneyText()
    {
        moneyText.gameObject.SetActive(false);
    }
    
    private void UpdateMoneyText()
    {
        moneyText.text = _shippingValue.ToString();
    }

    /// <summary>
    /// When interacted with, opens the Menu if there is no held item, ships current item otherwise
    /// </summary>
    public void Interact(GameObject player)
    {
        if(player.GetComponent<PlayerInventoryController>().holdingItem)
        {
            player.GetComponent<PlayerInventoryController>().ShipItem(this);
        }
        else if (!UIComponentReference.instance.GetDualInventory().IsActive())
        {
            DualInventory dInv = UIComponentReference.instance.GetDualInventory();
            dInv.ShowDualInventory();
            dInv.AssignDualInventories(_playerInventory,shippingInventory);
            dInv.AssignShippingBox(this);
        }
    }

    /// <summary>
    /// Returns total profits for the day and resets count
    /// </summary>
    /// <returns></returns>
    public int GetShippingValue()
    {
        int finalValue = _shippingValue;

        _shippingValue = 0;
        shippingInventory.EmptyInventory();

        print(finalValue);
        return finalValue;
    }
}