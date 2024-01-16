/*****************************************************************************
// File Name: Inventory_Icon.cs
// Author: Scott Acker
// Creation Date: Holds data about the icons for items in the inventory
//
// Brief Description:
*****************************************************************************/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemImage;
    public Image background;
    public TMP_Text quantityText;

    [Header("Colors")]
    private Button _button;
    public Color defaultColor;
    public Color selectedColor;
    public Color unavailableColor;
    public Color unavailableSelectColor;

    [HideInInspector] public Vector2 position;
    [HideInInspector] public int myIndex;
    [HideInInspector] public bool isEmpty = true;
    [HideInInspector] public bool isSelected;

    private VisualInventory _myVisInv;
    
    protected virtual void Start()
    {
        position = transform.position;
        isEmpty = true;
        _button = GetComponent<Button>();
        _myVisInv = transform.GetComponentInParent<VisualInventory>();
    }

    public void Select(bool available = true)
    {
        if (!_button)
            _button = GetComponent<Button>();
        
        _button.interactable = available;
        isSelected = true;
        background.color = available switch
        {
            true => selectedColor,
            false => unavailableSelectColor
        };
    }

    //returns 
    public void UnSelect(bool available = true)
    {
        if (!_button)
            _button = GetComponent<Button>();
        
        _button.interactable = available;
        isSelected = false;
        background.color = available switch
        {
            true => defaultColor,
            false => unavailableColor
        };
    }

    public virtual void SetIconVisuals(InventorySlot slotData) //TODO Fix to apply all visual element
    {
        if(slotData == null)
        {
            itemImage.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
        }
        else
        {
            SetImage(slotData.GetItem().MyPic());
            SetCount(slotData.Quantity);
        }
    }

    public void SetImage(Sprite newImage) 
    {
        itemImage.sprite = newImage;
        itemImage.gameObject.SetActive(newImage != null);
    }

    public void SetCount(int newCount)
    {
        quantityText.text = "x" + newCount;
        quantityText.gameObject.SetActive(!(newCount == 0 || newCount == 1));
    }

    public void Hover()
    {
        if (_myVisInv == null) return;
        _myVisInv.OnIconHover?.Invoke(myIndex,_myVisInv);
    }

    public void Click()
    {
        if (_myVisInv == null) return;
        _myVisInv.OnIconClick?.Invoke(myIndex,_myVisInv);
    }
}
