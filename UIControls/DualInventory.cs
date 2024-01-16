using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the use of inventories when two must be shown on screen, such as with a shipping box or storage box
/// </summary>
public class DualInventory : MonoBehaviour
{
    public GameObject background;
    private Inventory _inv1, _inv2;
    private VisualInventory _visInv1, _visInv2;

    private ShippingBox_Scr _currentShippingBox;
    
    private const int ItemsPerRow = 6;
    private const int MaxItems = 36;
    
    private int _currentlySelectedIndex;
    private const float InvControlsCooldown = 0.3f;
    
    private float _timeSinceLastControl;
    private float _menuOpenedTime;
    
    private bool _showingDualInventories;
    private bool _onLeftSide;
    
    private enum InvMode
    {
        Shipping,
        Trade
    }
    
    private InvMode _inventoryMode;
    
    private void Start()
    {
        UIComponentReference.instance.GetDualInventories(out _visInv1, out _visInv2);
        _timeSinceLastControl = Time.realtimeSinceStartup;
        _currentlySelectedIndex = 0;
        _onLeftSide = true;
        
        HideDualInventory();
    }

    private void OnDisable()
    {
        _visInv1.OnIconHover -= HandleIconHover;
        _visInv1.OnIconClick -= HandleIconClick;
        
        _visInv2.OnIconHover -= HandleIconHover;
        _visInv2.OnIconClick -= HandleIconClick;
    }

    
    // Update is called once per frame
    private void Update()
    {
        if (!_showingDualInventories || Time.realtimeSinceStartup - _menuOpenedTime < InvControlsCooldown) return;
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIComponentReference.instance.GetHotbarInventory().UpdateVisual();

            if(_currentShippingBox != null) _currentShippingBox.HideMoneyText();
            HideDualInventory();
        }
        InventoryNavigation();
        
        if (Input.GetButtonDown("Interact") && !Input.GetMouseButtonDown(0))
        {
            OnInteract();
        }
    }
    
    /// <summary>
    /// Controls for moving throughout the inventory with keyboard and mouse scroll
    /// </summary>
    private void InventoryNavigation()
    {
        if (Time.realtimeSinceStartup - _timeSinceLastControl < InvControlsCooldown) return;

        float xMove = Input.GetAxis("Horizontal");
        float yMove = Input.GetAxis("Vertical");
        float cycleXMove = Input.GetAxis("Cycle Items");

        if (xMove == 0 && yMove == 0 && cycleXMove == 0) return;

        if (xMove != 0)
        {
            _currentlySelectedIndex += (int) (1 * Mathf.Sign(xMove));
            if (_currentlySelectedIndex >= MaxItems * 2)
            {
                _currentlySelectedIndex = 0;
            }
            else if (_currentlySelectedIndex < 0)
            {
                _currentlySelectedIndex = MaxItems * 2 - 1;
            }

            _timeSinceLastControl = Time.realtimeSinceStartup;
        }
        else if (cycleXMove != 0)
        {
            _currentlySelectedIndex += (int) (1 * Mathf.Sign(xMove));
            if (_currentlySelectedIndex >= MaxItems * 2)
            {
                _currentlySelectedIndex = 0;
            }
            else if (_currentlySelectedIndex < 0)
            {
                _currentlySelectedIndex = MaxItems * 2 - 1;
            }

            _timeSinceLastControl = Time.realtimeSinceStartup;
        }

        if (yMove != 0)
        {
            _currentlySelectedIndex -= (int) (ItemsPerRow * 2 * Mathf.Sign(yMove));
            if (_currentlySelectedIndex < 0)
            {
                _currentlySelectedIndex += MaxItems * 2;
            }
            else if (_currentlySelectedIndex >= MaxItems * 2)
            {
                _currentlySelectedIndex -= MaxItems * 2;
            }

            _timeSinceLastControl = Time.realtimeSinceStartup;
        }


        ReflectInventorySpot();
        
    }
    
    /// <summary>
    /// Activates when the player presses the interact button. Moves an item over to the other inventory
    /// </summary>
    private void OnInteract()
    {
        int realIndex = FindSeparateIndex(_currentlySelectedIndex);
        switch (_inventoryMode)
        {
            case InvMode.Shipping:
                if (!_currentShippingBox) return;
                //select new icon
                if (_onLeftSide)
                {
                    if (realIndex >= _inv1.GetInvSize() ||  _inv2.IsFull()) return;
                    _currentShippingBox.ShipItem(realIndex);
                }
                else
                {
                    if (realIndex >= _inv2.GetInvSize() || _inv1.IsFull()) return;
                    _currentShippingBox.UnshipItem(realIndex);
                }
                UpdateVisualInventories();
                break;
            case InvMode.Trade:
                break;
            default:
                break;
        }
    }
    
    /// <summary>
    /// Updates the dual inventories to show the currently selected spot.
    /// </summary>
    private void ReflectInventorySpot()
    {
        int realIndex = FindSeparateIndex(_currentlySelectedIndex);
        
        //select new icon
        if (_onLeftSide)
        {
            _visInv1.SelectIcon(realIndex);
            _visInv2.DeselectIcon();
        }
        else
        {
            _visInv2.SelectIcon(realIndex); 
            _visInv1.DeselectIcon();
        }
    }
    
    public void ShowDualInventory()
    {
        background.SetActive(true);
        _timeSinceLastControl = Time.realtimeSinceStartup;
        _menuOpenedTime = Time.realtimeSinceStartup;
        _visInv1.Show();
        _visInv2.Show();
        
        GameController.Instance.inMenu = true;
        _showingDualInventories = true;
        UIComponentReference.instance.HideHUD();
    }
    
    private void HideDualInventory()
    {
        background.SetActive(false);
        _visInv1.Hide();
        _visInv2.Hide();

        GameController.Instance.inMenu = false;
        _showingDualInventories = false;
        UIComponentReference.instance.ShowHUD();
        _currentShippingBox = null;
    }

    /// <summary>
    /// Connect the two visual inventories to the respective inventories 
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    public void AssignDualInventories(Inventory one, Inventory two)
    {
        _visInv1.Assign(one, MaxItems, ItemsPerRow, 6);
        _inv1 = one;
        
        _visInv2.Assign(two, MaxItems, ItemsPerRow, 5);
        _inv2 = two;

        _visInv1.OnIconHover += HandleIconHover;
        _visInv1.OnIconClick += HandleIconClick;
        
        _visInv2.OnIconHover += HandleIconHover;
        _visInv2.OnIconClick += HandleIconClick;
        
        _currentlySelectedIndex = 0;
        _visInv1.SelectIcon(_currentlySelectedIndex);
        UpdateVisualInventories();
    }

    /// <summary>
    /// Update the visual for both inventories
    /// </summary>
    private void UpdateVisualInventories()
    {
        _visInv1.UpdateVisual();
        _visInv2.UpdateVisual();
    }

    /// <summary>
    /// Activates whenever the player's mouse hovers over one of the visual inventories
    /// </summary>
    /// <param name="num">The index number of the inventory</param>
    /// <param name="visualInventory">Which visual inventory the icon belongs to</param>
    private void HandleIconHover(int num, VisualInventory visualInventory)
    {
        _currentlySelectedIndex = FindFullIndex(num, visualInventory);
        ReflectInventorySpot();
    }
    
    /// <summary>
    /// Activates whenever the player clicks on one of the visual inventories
    /// </summary>
    /// <param name="num">The index number of the inventory</param>
    /// <param name="visualInventory">Which visual inventory the icon belongs to</param>
    private void HandleIconClick(int num, VisualInventory visualInventory)
    {
        _currentlySelectedIndex = FindFullIndex(num, visualInventory);
        ReflectInventorySpot();
        OnInteract();
    }

    /// <summary>
    /// Calculate the index of an inventory slot when both inventories are considered
    /// </summary>
    /// <param name="sIndex">The separated index of the inventory slots</param>
    /// <param name="inv"></param>
    /// <returns></returns>
    private int FindFullIndex(int sIndex, VisualInventory inv)
    {
        int fIndex = 0;
        int row = Mathf.FloorToInt(sIndex / ItemsPerRow);
        int column = Mathf.FloorToInt(sIndex % ItemsPerRow);
        
        //select new icon
        if (inv == _visInv1)
        {
            fIndex = row * ItemsPerRow * 2 + column;
            if (column > ItemsPerRow) fIndex -= ItemsPerRow;
        }
        else
        {
            
            fIndex = row * ItemsPerRow * 2+ column;
            if (column <= ItemsPerRow) fIndex += ItemsPerRow;
        }
        
        return fIndex;
    }
    
    private int FindSeparateIndex(int fullIndex)
    {
        int row = Mathf.FloorToInt(fullIndex / (ItemsPerRow * 2));

        //select new icon
        if (fullIndex % (ItemsPerRow * 2) < ItemsPerRow)
        {
            _onLeftSide = true;
            return fullIndex - ItemsPerRow * row;
        }
        else
        {
            _onLeftSide = false;
           return fullIndex - ItemsPerRow * row - ItemsPerRow;
        }
    }
    
    public void AssignShippingBox(ShippingBox_Scr sBox)
    {
        _currentShippingBox = sBox;
        _inventoryMode = InvMode.Shipping;
        _currentShippingBox.ShowMoneyText();
    }

    public bool IsActive()
    {
        return _showingDualInventories;
    }
}
