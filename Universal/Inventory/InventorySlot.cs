/*****************************************************************************
// File Name: InventorySlot.cs
// Author:
// Creation Date: 
//
// Brief Description: Stores data about a particular slot in inventory to be stored
*****************************************************************************/


using System;
using UnityEngine;

public class InventorySlot 
{
    private Item _myItem;

    private Sprite _mySprite;
    private int _quantity;

    public InventorySlot()
    {
        _myItem = null;
        _quantity = 1;
    }

    public void CopySlot(InventorySlot otherSlot)
    {
        if(otherSlot._myItem != null)
        {
            _myItem = otherSlot._myItem;
        }
        _mySprite = otherSlot._mySprite;
        _quantity = otherSlot._quantity;
    }
    
    public Item GetItem() { return _myItem;}

    public void SetItem(Item toSet) { _myItem = toSet; }

    public int Quantity
    {
        get { return _quantity; }
        set
        {
            _quantity = value;
            if (_quantity < 0) _quantity = 0;
        }
    }

    public void EraseSlot()
    {
        _myItem = null;
        _mySprite = null;
        _quantity = 0;
    }

    public void SetSprite(Sprite newSprite)
    {  _mySprite = newSprite; }

    public Sprite GetSprite()
    { return _mySprite; }

    public bool IsEmptySlot()
    {
        return _myItem == null;
    }
}
