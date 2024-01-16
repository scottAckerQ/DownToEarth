/*****************************************************************************
// File Name: PlayerInventoryController.cs
// Author:
// Creation Date: 2/7/2020
//
// Brief Description: Manages Player inventory, utilizing the Inventory class
*****************************************************************************/

using System.Collections;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [HideInInspector] public static PlayerInventoryController Instance;
    
    private PlayerInventory _myInventory;
    public int initialInvSize = 9;
    [HideInInspector]public int currentInvSize;
    private int _playerMoney;

    private PlayerController _pc;

    /// <summary> Where equipped tools will be shown </summary>
    public InventoryIcon toolBox;

    public bool usingTool;//whether or not a tool is being used

    [HideInInspector] public VisualInventory onScreenItemInv;
    [HideInInspector] public GameController gc;

    public GameObject heldItem;
    public bool holdingItem;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _pc = GetComponent<PlayerController>();
        gc = GameController.Instance;

        currentInvSize = initialInvSize; ;
        onScreenItemInv = UIComponentReference.instance.GetHotbarInventory();

        usingTool = false;

        //Create the inventory
        _myInventory = new PlayerInventory(currentInvSize);

        //sets up the on screen inventory
        onScreenItemInv.SetSizes(9, currentInvSize, 1);
        onScreenItemInv.Assign(_myInventory,9);
        toolBox = UIComponentReference.instance.GetToolBoxIcon();

        UpdateToolBoxVisual();
        UnholdItem();
    }

    private void Update()
    {
        if(!gc.inMenu)
        {
            InventoryControls();
        }
        
        if(Input.GetKeyDown(KeyCode.P)) //TEMP
        {
            print(_myInventory.InventoryContents());
        }
    }

    private void UpdateToolBoxVisual()
    {
        if (!_myInventory.HasItemEquipped())
        {
            toolBox.SetIconVisuals(null);

        }
        else
        {
            toolBox.SetIconVisuals(_myInventory.GetEquipSlot());
        }
    }

    public Inventory GetInventory()
    {
        return _myInventory;
    }

    /// <summary>
    /// Adds an object to the appropriate inventory space
    /// </summary>
    /// <param name="obj"></param>
    public void AddToInventory(GameObject obj)
    {
        PickUp objectPickUp = obj.GetComponent<PickUp>();
        bool isCrop = obj.TryGetComponent<Crop>(out Crop cropObject);

        if (!objectPickUp.GetActive()) return;
        if (!objectPickUp.hasStars)
        {
            _myInventory.AddItem(objectPickUp.id);
        }
        else
        {
            _myInventory.AddItem(objectPickUp.id, objectPickUp.starValue);
        }

        onScreenItemInv.UpdateVisual();

        Destroy(obj);
    }

    public void AddToInventory(Item newItem)
    {
        if(newItem.hasStars)
        {
            _myInventory.AddItem(newItem.itemID, newItem.starRating);
        }
        else
        {
            _myInventory.AddItem(newItem.itemID);
        }
        onScreenItemInv.UpdateVisual();
    }

    //used by shopkeepers
    public void AddToInventory(int itemID, int itemCount = 1)
    {
        _myInventory.AddItem(itemID, itemCount: itemCount);
        onScreenItemInv.UpdateVisual();
    }

    /// <summary>
    /// Controls which buttons change inventory positions as well as using tools
    /// </summary>
    private void InventoryControls()
    {
        if (gc.inMenu) return;
        if(Input.GetAxis("Cycle Items") != 0)
        {
            _myInventory.ShiftPosition(Input.GetAxis("Cycle Items"));
            onScreenItemInv.UpdateVisual();
            onScreenItemInv.SelectIcon(_myInventory.GetCurrentItemIndex());

            if (_myInventory.GetCurrentItem() != null && !_myInventory.GetCurrentItem().isTool) //TODO: A check to see if you're actually holding something
            {
                HoldItem(_myInventory.GetCurrentItem());
            }
            else
            {
                UnholdItem();
            }
        }

        if(Input.GetButtonDown("Equip"))
        {
            _myInventory.Equip();
            UpdateToolBoxVisual();
            onScreenItemInv.UpdateVisual();
        }

        if (!Input.GetButtonDown("Use Tool") || !_myInventory.HasItemEquipped()) return;
        
        UnholdItem();
        StartCoroutine(UseTool());
    }

    public void OpenBag()
    {

    }

    /// <summary>
    /// Puts the currently-held item in the shipping bin
    /// </summary>
    /// <param name="box"></param>
    public void ShipItem(ShippingBox_Scr box)
    {
        //box.Ship(_myInventory.GetCurrentItem(), _myInventory.GetCurrentInvSlot().quantity);
        print("Shipping item x" + _myInventory.GetCurrentInvSlot().Quantity);

        _myInventory.RemoveCurrentItemStack();
        onScreenItemInv.UpdateVisual();
    }

    //Passes the function of currently held tool onto whatever tiles are currently targeted
    private IEnumerator UseTool()
    {
        if(_pc.nowTargeting != null)
        {
            //TODO: Switch animation to tool swing
            usingTool = true;

            //toolInventory.GetCurrentItem().UseMe(_pc.nowTargeting);
            Item thisTool = _myInventory.GetEquippedTool();
            if (thisTool == null) yield break ;

            if (_pc.nowTargeting.GetComponent<Dirt>() != null)
            {
                Dirt thisDirt = _pc.nowTargeting.GetComponent<Dirt>();

                switch (thisTool.myToolType)
                {
                    case Item.ToolType.WCan:
                        if (_pc.nowTargeting.GetComponent<Dirt>() != null)
                            thisDirt.Water();
                        break;
                    case Item.ToolType.Hoe:
                            thisDirt.Till();
                        break;
                    case Item.ToolType.Seeds:
                        if (!thisDirt.Tilled || thisDirt.HasCrop) break;
                        if (_myInventory.GetEquippedToolCount() <= 0) break;
                        
                        int myID = thisTool.itemID;
                        _pc.nowTargeting.GetComponent<Dirt>().Plant(myID);
                        _myInventory.DecreaseEquippedItemCount(1); //GLITCH: Index out of bounds exception
                        UpdateToolBoxVisual();
                        
                        break;
                }
            }
            

            yield return new WaitForSeconds(.3f);
            usingTool = false;
        }
            
    }

    public void HoldItem(Item thisItem)
    {
        heldItem.GetComponent<SpriteRenderer>().sprite = thisItem.MyPic();
        holdingItem = true;
    }

    public void UnholdItem()
    {
        heldItem.GetComponent<SpriteRenderer>().sprite = null;
        holdingItem = false;
    }

    public bool InventoryHasSpace(GameObject obj)
    {
        PickUp objectPickUp = obj.GetComponent<PickUp>();
        return _myInventory.HasSpaceForThisItem(objectPickUp.GetID());
    }

    public bool InventoryHasSpace(int itemID)
    {
        return _myInventory.HasSpaceForThisItem(itemID);
    }

    public int PlayerMoney
    {
        get { return _playerMoney; }
        set
        {
            _playerMoney = value;
            Mathf.Clamp(_playerMoney, 0, 999999999);
            UIComponentReference.instance.UpdatePlayerMoney(_playerMoney);
        }
    }

}
