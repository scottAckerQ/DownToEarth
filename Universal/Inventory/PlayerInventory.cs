/*****************************************************************************
// File Name: PlayerInventory.cs
// Creation Date: An inventory specifically for the player. Extends Inventory.cs
// Adds functionality for an equipped item
//
// Brief Description: Stores data about a particular slot in inventory to be stored
*****************************************************************************/

using System.Diagnostics;
using UnityEngine;

public class PlayerInventory : Inventory
{
    
    private readonly InventorySlot _equipSlot;
    private readonly InventoryIcon _toolboxIcon;

    /// <summary>
    /// Instantiates an inventory with a specified size
    /// </summary>
    /// <param name="myDatabase">The database of all items that the inventory must draw from</param>
    /// <param name="size">How many slots this inventory will have</param>
    public PlayerInventory(int size) : base( size)
    {
        _equipSlot = new InventorySlot();
        GameController.Instance.playerInventory = this;
        _toolboxIcon = UIComponentReference.instance.GetToolBoxIcon();
        
    }

    /// <summary>
    /// Equip the currently selected item
    /// </summary>
    public void Equip()
    {
        var toBeEquipped = GetCurrentInvSlot();
        var oldEquipItem = _equipSlot.GetItem();
        var count = _equipSlot.Quantity;
        
        if (toBeEquipped.IsEmptySlot()) return;
        if (!toBeEquipped.GetItem().isTool) return;

        _equipSlot.CopySlot(toBeEquipped);
        _toolboxIcon.SetIconVisuals(_equipSlot);

        RemoveCurrentItemStack(); //can only equip current slot
        ReturnToMainInventory(oldEquipItem, count);
    }

    /// <summary>
    /// Removes item from "equipped" slot and returns it to main inventory
    /// </summary>
    private void ReturnToMainInventory(Item oldTool, int count)
    {
        if (oldTool == null || count == 0) return;

        AddItem(oldTool.itemID, count);
    }

    public Item GetEquippedTool()
    {
        return _equipSlot.GetItem();
    }

    public int GetEquippedQuantity()
    {
        return _equipSlot.Quantity;
    }

    public int GetEquippedToolCount()
    {
        return _equipSlot.Quantity;
    }

    public void DecreaseEquippedItemCount(int x)
    {
        _equipSlot.Quantity -= x;
        if(_equipSlot.Quantity <= 0)
        {
            _equipSlot.EraseSlot();
            
        }
    }

    public InventorySlot GetEquipSlot()
    {
        return _equipSlot;
    }

    /// <summary>
    /// Returns true if there is an item equipped
    /// </summary>
    /// <returns></returns>
    public bool HasItemEquipped()
    {
        return !_equipSlot.IsEmptySlot();
    }

    public override string InventoryContents()
    {
        string fullString = base.InventoryContents();
        fullString += $"\n EquippedTool: {_equipSlot.GetItem().itemName} [{_equipSlot.Quantity}]";
        return fullString;
    }

    
}
