/*****************************************************************************
// File Name: PlayerInventoryController.cs
// Author: Scott Acker
// Creation Date: 12/12/2020
//
// Brief Description: Controls all functions of an inventory
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    /// <summary> 
    /// The actual Inventory created 
    /// </summary>
    protected List<InventorySlot> inv;

    protected readonly ItemDatabase database;

    /// <summary> 
    /// The index of the current spot in the inventory 
    /// </summary>
    protected int currentItemIndex;

    /// <summary> The inventory's maximum size </summary>
    protected int invSize;

    /// <summary> How many slots in the inventory are filled </summary>
    protected int fillCount;

    /// <summary>
    /// Instantiates a new inventory of size 15 and assigns the item database to it
    /// </summary>
    /// <param name="myDatabase">The database of all items that the inventory must draw from</param>
    public Inventory()
    {
        inv = new List<InventorySlot>();
        currentItemIndex = 0;
        invSize = 15;
        database = ItemDatabase.instance;

        InitializeInventory();
    }

    /// <summary>
    /// Instantiates an inventory with a specified size
    /// </summary>
    /// <param name="myDatabase">The database of all items that the inventory must draw from</param>
    /// <param name="size">How many slots this inventory will have</param>
    public Inventory(int size)
    {
        inv = new List<InventorySlot>();
        currentItemIndex = 0;
        invSize = size;
        database = ItemDatabase.instance;

        InitializeInventory();
    }


    /// <summary>
    /// Fills the inventory list with empty/null slots to be filled later
    /// </summary>
    public void InitializeInventory()
    {
        for (int i = 0; i < invSize; i++)
        {
            InventorySlot newSlot = new InventorySlot();
            inv.Add(newSlot);
        }
    }

    /// <summary>
    /// Adds an item to the inventory if the inventory is not filled
    /// </summary>
    /// <param name="itemID">The item to be added.</param>
    public void AddItem(int itemID, int itemCount = 1)
    {
        InventorySlot newSlot = new InventorySlot(); //Create a new invSlot with the item stored in it
        Item toAdd = database.GetItem(itemID); //Grab itemID and find corresponding member of database

        newSlot.SetItem(toAdd);
        
        newSlot.Quantity = itemCount;

        PlaceItemInInventory(newSlot); 
    }

    public void AddItem(int itemID, float starValue, int itemCount = 1)
    {
        InventorySlot newSlot = new InventorySlot(); //Create a new invSlot with the item stored in it
        Item toAdd = database.GetItem(itemID); //Grab itemID and find corresponding member of database
        toAdd.hasStars = true;

        toAdd.sellPrice = Mathf.RoundToInt(toAdd.sellPrice * 1.05f * starValue);

        toAdd.SetStars(starValue);
        newSlot.SetItem(toAdd);
        newSlot.Quantity = itemCount;

        PlaceItemInInventory(newSlot); //find a place for this item
        //UpdateSprite();
    }



    /// <summary>
    /// Switches the contents of two inventory slots in the same inventory
    /// </summary>
    /// <param name="one">The index of one item you'd like to swap</param>
    /// <param name="two">The index of one item you'd like to swap</param>
    public void Swap(int one, int two)
    {
        InventorySlot temp = inv[one];
        inv[one].CopySlot(inv[two]);
        inv[two].CopySlot(temp);
    }

    /// <summary>
    /// Finds the first empty slot in the array or the first matching slot and adds 
    /// </summary>
    /// <param name="itemSlot">The item to be added.</param>
    protected void PlaceItemInInventory(InventorySlot itemSlot)
    {
        if (!HasSpaceForThisItem(itemSlot))
        {
            Debug.LogWarning("There is not enough room in this inventory!!! Check further up the line");
            return;
        }

        //if not empty, find out which block this can be added to.
        int foundIndex = FindMatchingSlotIndex(itemSlot.GetItem().itemID);

        if (foundIndex != -1)
        {
            inv[foundIndex].Quantity += itemSlot.Quantity;
            return;
        }
        
        //if there is nothing that it can add Quantity to, just tack it onto the first empty slot
        foreach (InventorySlot t in inv)
        {
            if (t.IsEmptySlot())
            {
                t.CopySlot(itemSlot);
                return; 
            }
        }
        ++fillCount;
        Debug.Log($"Inventory has {fillCount} slots filled ");
    }

    /// <summary>
    /// Checks to see if there is space to put this item in the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasSpaceForThisItem(InventorySlot item)
    {
        return HasSpaceForThisItem(item.GetItem().itemID);
    }

    /// <summary>
    /// Auto converts to an item class based on ID to go through the other HasSpaceForThisItem
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public bool HasSpaceForThisItem(int itemID)
    {
        if (IsFull() && FindMatchingSlotIndex(itemID) == -1)
        {
            return false;
        }
        return true;
    }

    private int FindMatchingSlotIndex(int itemID)
    {
        if (!ItemDatabase.instance.GetItem(itemID).canStack) return - 1;

        int answer = -1;
        int index = 0;

        for(int i = 0; i < GetFilledSize(); i++)
        {
            InventorySlot itemSlot = inv[i];
            if (itemSlot.IsEmptySlot()) break;

            if (itemSlot.GetItem().itemID == itemID)
            {
                answer = index;
                break;
            }
        }

        return answer;
    }

    /// <summary>
    /// Removes the currently-selected item from the inventory
    /// </summary>
    public void RemoveOneItem()
    {
        InventorySlot toBeRemoved = inv[currentItemIndex];

        toBeRemoved.Quantity--;

        if (toBeRemoved.Quantity == 0)
        {
            toBeRemoved.EraseSlot();

            if (!IsEmpty())
            {
                ShiftPosition(1);
            }
            else
            {
                currentItemIndex = 0;
            }
        }


        //UpdateSprite();
    }

    /// <summary>
    /// Removes the entire stack of an item
    /// </summary>
    public void RemoveCurrentItemStack()
    {
        InventorySlot toBeRemoved = inv[currentItemIndex];

        toBeRemoved.Quantity = 0;
        toBeRemoved.EraseSlot();

        //if (!IsEmpty())
        //{
        //    ShiftPosition(1);
        //}
        //else
        //{
        //    currentItemIndex = 0;
        //}
        //UpdateSprite();
    }

    public void RemoveIndexItemStack(int index)
    {
        InventorySlot toBeRemoved = inv[index];

        toBeRemoved.Quantity = 0;
        toBeRemoved.EraseSlot();

        //if (!IsEmpty())
        //{
        //    ShiftPosition(1);
        //}
        //else
        //{
        //    currentItemIndex = 0;
        //}
    }

    /// <summary>
    /// Depletes Quantity item at the specified index. If there is only one of that item left, the item is deleted from the inventory
    /// </summary>
    /// <param name="index">The index of the array that will be removed from the inventory</param>
    /// <param name="removeNum">The amount of the item that will be removed from the inventory</param>
    public void RemoveIndexItem(int index, int removeNum)
    {
        InventorySlot toBeRemoved = inv[index];

        if (removeNum > toBeRemoved.Quantity)
        {
            throw new Exception("There aren't enough items to get rid of.");
        }

        toBeRemoved.Quantity -= removeNum;

        if (toBeRemoved.Quantity == 0)
        {
            inv[index].EraseSlot();
            //if (!IsEmpty())
            //{
            //    ShiftPosition(1);
            //}
            //else
            //{
            //    currentItemIndex = 0;
            //}
        }


        //UpdateSprite();
    }

    /// <summary>
    /// Shifts inventory position to the left if direction is negative and to the right if direction is positive or zero.
    /// Cycles through the inventory until it finds the next non-null slot in that direction.
    /// </summary>
    /// <param name="direction"></param>
    public void ShiftPosition(float direction)
    {
        if (!IsEmpty())
        {
            int front = 0;
            int rear = inv.Count - 1;

            if (direction < 0)
            {
                if (currentItemIndex == rear)//if you start at rear, make the next position the front
                {
                    currentItemIndex = front;
                }
                else
                {
                    currentItemIndex++;
                }


                //while (Inv[currentItemIndex] == null)//if that is null, increase the counter until you reach something that is not null
                //{
                //    if (currentItemIndex == rear)
                //        currentItemIndex = front;
                //    else
                //        currentItemIndex++;
                //}
            }
            else //if going in reverse
            {
                if (currentItemIndex == 0)
                {
                    currentItemIndex = rear;
                }
                else
                    currentItemIndex--;

                //while (Inv[currentItemIndex] == null)
                //{
                //    if (currentItemIndex == 0)
                //        currentItemIndex = Inv.Count;
                //    else
                //        currentItemIndex--;

                //}
            }

            //UpdateSprite();
        }

    }

    /// <summary>
    /// Empties inventory of all items
    /// </summary>
    public void EmptyInventory()
    {
        foreach(InventorySlot slot in inv)
        {
            slot.SetItem(null);
        }
    }

    /// <summary>
    /// Returns information about the item currently selected
    /// </summary>
    /// <returns>The item within the current slot</returns>
    public Item GetCurrentItem()
    {
        return inv[currentItemIndex].GetItem();
    }

    /// <summary>
    /// Returns the index of the item currently selected
    /// </summary>
    /// <returns></returns>
    public int GetCurrentItemIndex()
    {
        return currentItemIndex;
    }

    /// <summary>
    /// Returns the Inventory Slot that is currently selected
    /// </summary>
    /// <returns>The slot at the index of currentItem</returns>
    public InventorySlot GetCurrentInvSlot()
    { 
        return inv[currentItemIndex]; 
    }

    /// <summary>
    /// Returns the inventory slot at the specified index
    /// </summary>
    /// <param name="index">What index of the inventory you would like to check</param>
    /// <returns>The item at index in the list</returns>
    public InventorySlot GetSlotAt(int index)
    {
        return inv[index];
    }

    public int GetIdOfItemAtIndex(int index)
    {
        return inv[index].GetItem().itemID;
    }

    public Item GetItemAtIndex(int index)
    {
        return inv[index].GetItem();
    }

    /// <summary>
    /// Returns the current maximum size of the inventory
    /// </summary>
    /// <returns></returns>
    public int GetInvSize()
    {
        return invSize;
    }

    /// <summary>
    /// Returns how many slots in the inventory are filled
    /// </summary>
    /// <returns>The number of slots in the inventory that are filled</returns>
    public int GetFilledSize()
    {
        return inv.Count;
    }

    /// <summary>
    /// Returns true if there are no items within the inventory.
    /// </summary>
    /// <returns>Whether or not the inventory is empty</returns>
    public bool IsEmpty()
    {
        return inv.Count == 0;
    }

    /// <summary>
    /// Is the inventory full?
    /// </summary>
    /// <returns>Whether or not the inventory is full</returns>
    public bool IsFull()
    {
        return invSize == fillCount;
    }

    /// <summary>
    /// Changes the inventory size for upgrades
    /// </summary>
    public void Upgrade(int newSize)
    {
        invSize = newSize;
    }

    /// <summary>
    /// Prints out what the array of inventory contains
    /// </summary>
    /// <returns>Every item and its Quantity within the array</returns>
    public virtual string InventoryContents()
    {
        string result;

        //result = "The first member of this list is: " + Inv[0].GetItem().ItemName;

        if (!IsEmpty())
        {
            result = $"This inventory [{fillCount}] contains: ";

            foreach(InventorySlot itemSlot in inv)
            {
                if (!itemSlot.IsEmptySlot())
                {
                    result += "[" + itemSlot.GetItem().itemName + " x" + itemSlot.Quantity + "] ";
                    if (itemSlot.GetItem().hasStars)
                    {
                        result += itemSlot.GetItem().starRating + "*]";
                    }
                    result += ", \n";
                }
            }
        }
        else
        {
            result = "There is nothing in this inventory";
        }


        return result;
    }
}
