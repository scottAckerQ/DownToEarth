#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

#endregion

public class VisualInventory : MonoBehaviour
{
    public Vector2 originPosition;
    public int numberOfRows = 1;
    private int _slotsPerRow;
    private int _visualSlotCount;
    public bool verticalInventory = false;

    public List<Transform> allRowsList;
    private List<InventoryIcon> _invSlots;
    [FormerlySerializedAs("invSlotPrefab")] public GameObject invIconPrefab;
    
    private int _currentPage = 1;
    private int _maxPages = 3;

    private Inventory _myItems;
    private int _selectedSlotIndex;
    
    public Action<int, VisualInventory> OnIconHover, OnIconClick;

    private void CreateSlots()
    {
        _invSlots = new List<InventoryIcon>();
        for (int i = 0; i < _visualSlotCount; i++)
        {
            int rowChosen = verticalInventory ? 0 : Mathf.FloorToInt(i / _slotsPerRow) ;

            GameObject newSlot = Instantiate(invIconPrefab, allRowsList[rowChosen]);
            InventoryIcon iconData = newSlot.GetComponent<InventoryIcon>();
            _invSlots.Add(iconData);
            iconData.myIndex = i;
        }
        UnselectAllIcons();
    }
    
    private void PlaceThings()
    {
        //for (int i = 0; i < _invSlots.Count; i++)
        //{
        //    RectTransform rect = _invSlots[i].gameObject.GetComponent<RectTransform>();

        //    if (i == 0)
        //    {
        //        rect.localPosition = originPosition;
        //    }
        //    else if (i % _slotsPerRow == 0)
        //    {
        //        RectTransform prevRect = _invSlots[i - _slotsPerRow].gameObject.GetComponent<RectTransform>();
        //        Vector2 abovePos = prevRect.localPosition;
        //        rect.localPosition = new Vector2(abovePos.x, abovePos.y - verticalSpacing);
        //    }
        //    else
        //    {
        //        RectTransform prevRect = _invSlots[i - 1].gameObject.GetComponent<RectTransform>();
        //        Vector2 prevPos = prevRect.localPosition;
        //        rect.localPosition = new Vector2(prevPos.x + horizontalSpacing, prevPos.y);
        //    }
        //}
    }

    /// <summary>
    /// Set the initial parameters for this Visual Inventory
    /// </summary>
    /// <param name="slotsOnAPage">How many inventory slots will appear at once</param>
    /// <param name="maxPages">How many pages there will be</param>
    public void SetSizes(int slotsPerRow, int slotsOnAPage, int maxPages)
    {
        _maxPages = maxPages;
        _slotsPerRow = slotsPerRow;
        _currentPage = 1;
    }

    public void SelectIcon(int index)
    {
        _invSlots[_selectedSlotIndex].UnSelect(_selectedSlotIndex < _myItems.GetInvSize());
        _invSlots[index].Select(index < _myItems.GetInvSize());
        _selectedSlotIndex = index;
    }

    /// <summary>
    /// Apply the unselected color of an icon to the previous inventory slot
    /// </summary>
    public void DeselectIcon()
    {
        _invSlots[_selectedSlotIndex].UnSelect(_selectedSlotIndex < _myItems.GetInvSize());
    }
    
    /// <summary>
    /// Apply the unselected color of an icon to the inventory slot
    /// </summary>
    /// <param name="index">The index of the slot</param>
    public void DeselectIcon(int index)
    {
        _invSlots[index].UnSelect(index < _myItems.GetInvSize());
    }

    public void UnselectAllIcons()
    {
        for (int i = 0; i < _invSlots.Count; i++)
        {
            _invSlots[i].UnSelect(i < _myItems.GetInvSize());
        }
    }

    public void BlockUnShippableItems()
    {
    }
    
    /// <summary>
    /// Reflects the addition and subtraction of items from the inventory. Called after any changes to the array
    /// </summary>
    public void UpdateVisual()
    {
        if (_myItems != null)
        {
            for (int i = 0; i < _invSlots.Count; i++)
            {
                InventoryIcon thisIcon = _invSlots[i];

                //deselect all as they are run through, leaving only the necessary one later
                
                thisIcon.SetIconVisuals(null);

                if (i >= _myItems.GetInvSize()) continue;
                InventorySlot thisSlot = _myItems.GetSlotAt(i);
                if (thisSlot.IsEmptySlot() ) continue;
                
                thisIcon.SetIconVisuals(thisSlot);

            }
        }
        else
        {
            Debug.LogError($"Could not find inventory for {gameObject.name}");
        }

        
        //Save for later: related to 
        ////deactivate subsequent pages
        ////if(slotsPerPage > realInventorySize)
        ////{
        ////    for(int i = realInventorySize; i < slotsPerPage; i++)
        ////    {
        ////        invSlots[i].gameObject.SetActive(false);
        ////    }
        ////}

        //int multiplier = currentPage;

        //int maxRange = (slotsPerPage * multiplier) - 1;
        //int minRange = maxRange - slotsPerPage + 1;

        ////check each and every spot within that page
        //for (int i = minRange; i < maxRange; i++)
        //{
        //    print("Selecting " + i);
        //    print("Current item index: " + myItems.GetCurrentItemIndex());
        //    //if(i > realInventorySize) //do nothing if i is outside of visual inventory
        //    //{
        //    //    return;
        //    //}


        //}
    }

    /// <summary>
    /// Connects this visual inventory to an actual inventory
    /// </summary>
    /// <param name="inv">The inventory that this will be connected to</param>
    /// <param name="totalCount">How many slots should appear on screen at one time</param>
    /// <param name="rowLimit">The maximum number of slots that can be shown in one row</param>
    public void Assign(Inventory inv, int totalCount, int rowLimit = 9, int rowCount = 1)
    {
        _myItems = inv;
        _currentPage = 1;
        _visualSlotCount = totalCount;
        _slotsPerRow = rowLimit;
        numberOfRows = rowCount;

        CreateSlots();
        PlaceThings();
        UpdateVisual();
    }

    /// <summary>
    /// Unhides this visual inventory
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides this visual inventory from view
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Swaps the position in the inventory of two items.
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    private void Swap(int one, int two)
    {
        _myItems.Swap(one, two);
        UpdateVisual();
    }

    /// <summary>
    /// Switches to a different page depending on which direction is input
    /// </summary>
    /// <param name="direction">Positive to go to the next page, negative for previous page</param>
    public void SwitchPages(int direction)
    {
        if (direction > 0)
        {
            _currentPage += 1;
            if (_currentPage > _maxPages) _currentPage = 1;
        }
        else
        {
            _currentPage -= 1;
            if (_currentPage < 0) _currentPage = _maxPages;
        }
        UpdateVisual();
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}