using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory 
{
    
    /// <summary> 
    /// The actual Inventory created 
    /// </summary>
    private List<InventorySlot> _inv;

    private readonly ItemDatabase _database;

    /// <summary> 
    /// The index of the current spot in the inventory 
    /// </summary>
    private int _currentItemIndex;

    /// <summary>  The sprite of the item currently held </summary>
    private Sprite _currentItemSprite;

    /// <summary>Sprite to be displayed if the inventory is empty</summary>
    private Sprite _emptyInvSprite;

    /// <summary> Where in the UI that the currently held item will be shown </summary>
    private Vector3 _itemUIPos;

    /// <summary> The sprite object to be changed to show the current item </summary>
    private Image _itemDisplay;
    private Text _itemCount;

    /// <summary> The inventory's maximum size </summary>
    private int _invSize;

    /// <summary> How many slots in the inventory are filled </summary>
    private int _fillCount;
    
    /// <summary> 
    /// Instantiates the inventory at size 15 </summary>
    public ItemInventory(Sprite emptySprite, Image itemBox, Text QuantityText, ItemDatabase myDatabase)
    {
        _inv = new List<InventorySlot>();
        _currentItemIndex = 0;
        _invSize = 15;

        
        _emptyInvSprite = emptySprite;
        _itemDisplay = itemBox;
        _itemCount = QuantityText;
        _database = myDatabase;

        FillInventory();
    }

    public ItemInventory(ItemDatabase myDatabase)
    {
        _inv = new List<InventorySlot>();
        _currentItemIndex = 0;
        _invSize = 15;
        _database = myDatabase;

        FillInventory();
    }

    //"fills" all inventory slots with empty/null slots to be filled later
    public void FillInventory()
    {
        for(int i = 0; i < _invSize;i++)
        {
            InventorySlot newSlot = new InventorySlot();
            _inv.Add(newSlot);
        }
    }

    /// <summary>
    /// Returns information about the item currently selected
    /// </summary>
    /// <returns></returns>
    public Item GetCurrentItem()
    {
        return _inv[_currentItemIndex].GetItem();
    }

    

    /// <summary>
    /// Returns the index of the item currently selected
    /// </summary>
    /// <returns></returns>
    public int GetCurrentItemIndex()
    {
        return _currentItemIndex;
    }

    public InventorySlot GetCurrentSlot()
    { return _inv[_currentItemIndex]; }

    /// <summary>
    /// Returns the inventory slot at the specified index
    /// </summary>
    /// <param name="index">What index of the inventory you would like to check</param>
    /// <returns></returns>
    public InventorySlot GetSlotAt(int index)
    {
        return _inv[index];
    }

    /// <summary>
    /// Returns the current maximum size of the inventory
    /// </summary>
    /// <returns></returns>
    public int GetInvSize()
    {
        return _invSize;
    }

    /// <summary>
    /// Returns how many slots in the inventory are filled
    /// </summary>
    /// <returns></returns>
    public int GetFilledSize()
    {
        return _inv.Count;
    }

    /// <summary>
    /// Switches the contents of two inventory slots in the same inventory
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    public void Swap(int one, int two)
    {
        InventorySlot temp = _inv[one];
        _inv[one].CopySlot(_inv[two]);
        _inv[two].CopySlot(temp);
    }

    /// <summary>
    /// Adds an item to the inventory if the inventory is not filled
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void AddItem(int itemID)
    {
        InventorySlot newSlot = new InventorySlot(); //Create a new invSlot with the item stored in it
        Item toAdd = _database.GetItem(itemID); //Grab itemID and find corresponding member of database

        newSlot.SetItem(toAdd);
        ++_fillCount;

        FirstEmpty(newSlot); //find a place for this item
        //UpdateSprite();
        
    }

    public void AddItem(int itemID, float starValue)
    {
        InventorySlot newSlot = new InventorySlot(); //Create a new invSlot with the item stored in it
        Item toAdd = _database.GetItem(itemID); //Grab itemID and find corresponding member of database
        toAdd.hasStars = true;

        toAdd.sellPrice = Mathf.RoundToInt(toAdd.sellPrice * 1.15f * starValue);

        toAdd.SetStars(starValue);
        newSlot.SetItem(toAdd);
        ++_fillCount;

        FirstEmpty(newSlot); //find a place for this item
        //UpdateSprite();
    }

    /// <summary>
    /// Finds the first empty slot in the array or the first matching slot and adds 
    /// </summary>
    /// <param name="item">The item to be added.</param>
    private void FirstEmpty(InventorySlot item)
    {
        //Check if the array is empty first
        if(IsEmpty())
        {
            item.Quantity++;
            _inv[0] = item;
            _currentItemIndex = 0;
        }
        //if not empty, find out which block this can be added to.
        else
        {
            bool found = false; //whether or not a place has been found

            for (int i = 0; i < _inv.Count; i++)
            {
                if (!found) //as long as the item has not yet been found, continue to check the inventory
                {
                    if(!_inv[i].IsEmptySlot())
                    {
                        //if this item's ID matches something else in the inventory, add to its Quantity
                        if (item.GetItem().itemID == _inv[i].GetItem().itemID /*&& Inv[i].Quantity < 99*/)
                        {
                            _inv[i].Quantity++;
                            found = true;
                        }
                    }
                    
                }
            }

            //if there is nothing that it can add Quantity to, just tack it onto the first empty slot
            if(!found && !IsFull())
            {
                for (int i = 0; i < _inv.Count; i++)
                {
                    if (!found) //as long as the item has not yet been found, continue to check the inventory
                    {
                        if (_inv[i].IsEmptySlot())
                        {
                            found = true;
                            _inv[i].CopySlot(item);
                            _inv[i].Quantity = 1;
                        }

                    }
                }
            }
            else if(IsFull())
            {
                Debug.Log("Full inventory");
            }
            
        }
        
    }

    /// <summary>
    /// Returns true if there are no items within the inventory.
    /// </summary>
    /// <returns>Whether or not the inventory is empty</returns>
    public bool IsEmpty()
    {
        return _inv.Count == 0;
    }
    
    /// <summary>
    /// Whether or not the inventory is full
    /// </summary>
    /// <returns></returns>
    private bool IsFull()
    {
        return _invSize == _fillCount;
    }

    /// <summary>
    /// Removes the currently-selected item from the inventory
    /// </summary>
    public void RemoveOneItem()
    {
        InventorySlot toBeRemoved = _inv[_currentItemIndex];

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
                _currentItemIndex = 0;
            }
        }


        //UpdateSprite();
    }

    public void RemoveItemStack()
    {
        InventorySlot toBeRemoved = _inv[_currentItemIndex];

        toBeRemoved.Quantity = 0;
        toBeRemoved.EraseSlot();

        if (!IsEmpty())
        {
            ShiftPosition(1);
        }
        else
        {
            _currentItemIndex = 0;
        }
        //UpdateSprite();
    }

    /// <summary>
    /// Removes the item at the specified index. If there is only one of that item left, the item is deleted from the inventory
    /// </summary>
    /// <param name="index">The index of the array that will be removed from the inventory</param>
    /// <param name="removeNum">The amount of the item that will be removed from the inventory</param>
    public void RemoveItem(int index, int removeNum)
    {
        InventorySlot toBeRemoved = _inv[index];

        if (removeNum > toBeRemoved.Quantity)
        {
            throw new Exception("There aren't enough items to get rid of.");
        }

        toBeRemoved.Quantity -= removeNum;

        if (toBeRemoved.Quantity == 0)
        {
            _inv[index].EraseSlot();
            if (!IsEmpty())
            {
                ShiftPosition(1);
            }
            else
            {
                _currentItemIndex = 0;
            }
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
            int rear = _inv.Count - 1;

            if (direction > 0)
            {
                if (_currentItemIndex == rear)//if you start at rear, make the next position the front
                {
                    _currentItemIndex = front;
                }
                else
                {
                    _currentItemIndex++;
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
                if (_currentItemIndex == 0)
                {
                    _currentItemIndex = rear;
                }
                else
                    _currentItemIndex--;

                //while (Inv[currentItemIndex] == null)
                //{
                //    if (currentItemIndex == 0)
                //        currentItemIndex = Inv.Count;
                //    else
                //        currentItemIndex--;

                //}
            }

            //UpdateSprite();
            Debug.Log("currentItemIndex: " + _currentItemIndex);
        }

    }

    /// <summary>
    /// Change which sprite is shown in the display window. No longer in use
    /// </summary>
    //public void UpdateSprite()
    //{
    //    if (!IsEmpty()) //if not null, change the pic to current item
    //    {
    //        itemDisplay.sprite = Inv[currentItemIndex].GetItem().myPic;
            

    //        itemCount.text = "x" + Inv[currentItemIndex].Quantity;
    //    }
    //    else //or just make it the blank picture
    //    {
    //        itemDisplay.sprite = emptyInvSprite;
    //    }

    //}

    /// <summary>
    /// Changes the inventory size for upgrades
    /// </summary>
    public void Upgrade(int newSize)
    {
        _invSize = newSize;
    }

    /// <summary>
    /// Prints out what the array of inventory contains
    /// </summary>
    /// <returns>Every item and its Quantity within the array</returns>
    public string PrintContents()
    {
        string result;

        //result = "The first member of this list is: " + Inv[0].GetItem().ItemName;

        if (!IsEmpty())
        {
            result = "This inventory contains: ";
            for (int i = 0; i < _inv.Count; i++)
            {
                if(!_inv[i].IsEmptySlot())
                {
                    result += "[" + _inv[i].GetItem().itemName + " x" + _inv[i].Quantity + "] ";
                    if (_inv[i].GetItem().hasStars)
                    {
                        result += "[" + _inv[i].GetItem().starRating + "*]";
                    }
                    result += ", ";
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
