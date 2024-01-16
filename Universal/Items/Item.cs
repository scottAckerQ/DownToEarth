using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Monetization;

[Serializable]
public class Item
{
    private Sprite myPic; 

    public int itemID;
    public string itemName;
    public string itemDescription;

    public bool hasStars;
    public float starRating;

    public bool canStack;
    public bool isTool;

    public int sellPrice;
    public int storePrice;
    public bool canBeShipped;
    public ScaleManager_Scr.ObjectScales myScale;

    public enum ToolType
    {
        WCan,
        Hoe,
        Seeds,
        Axe,
        Hammer,
        Rod
    }
    public ToolType myToolType;

    public Item() 
    {
        itemID = 0;
        itemName = "Turnip";
        itemDescription = "Placeholder";

        isTool = false;

        canStack = false;
        canBeShipped = false;

        hasStars = false;
        starRating = 0;

        sellPrice = 0;
        storePrice = 0;

        myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);
        if (!myPic)
        {
            Debug.LogError($"Could not find item {itemName} image!");
        }
    }

    /// <summary>
    /// Constructor for an item that is not a tool
    /// </summary>
    /// <param name="myID">The ID of the item</param>
    /// <param name="myName">The name of the item</param>
    /// <param name="description">A description for the item</param>
    /// <param name="hasStarValue">Whether or not this item includes a star value</param>
    /// <param name="stackable">Whether or not this item can stack</param>
    /// <param name="sell">The price at which the player can sell this</param>
    /// <param name="store">The price that a store will sell this for</param>
    /// <param name="canBeShipped"> Whether or not the item can be put in the shipping bin</param>
    public Item(int myID, string myName, string description,
        bool hasStarValue,
        bool stackable, bool canBeShipped,
        int sell,int store, ScaleManager_Scr.ObjectScales scale = ScaleManager_Scr.ObjectScales.Human)
    {
        itemID = myID;
        itemName = myName;
        itemDescription = description;

        isTool = false;

        canStack = stackable;
        this.canBeShipped = canBeShipped;

        hasStars = hasStarValue;
        starRating = 0;

        sellPrice = sell;
        storePrice = store;

        myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);
        
        if (!isTool)
            myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);
        else
            myPic = Resources.Load<Sprite>("Sprites/Tools/" + itemName);
        if (!myPic)
        {
            Debug.LogError($"Could not find item {itemName} image!");
        }
    }
    
    /// <summary>
    /// Item constructor for tools
    /// </summary>
    /// <param name="myID">The ID of the item</param>
    /// <param name="myName">The name of the item</param>
    /// <param name="description">A description for the item</param>
    /// <param name="type">The tooltype that this item fits</param>
    /// <param name="stackable">Whether or not this item can stack</param>
    /// <param name="sell">The price at which the player can sell this</param>
    /// <param name="store">The price that a store will sell this for</param>
    public Item(int myID, string myName, string description,
        ToolType type, bool stackable, bool shippable,
        int sell, int store, ScaleManager_Scr.ObjectScales scale = ScaleManager_Scr.ObjectScales.Human)
    {
        itemID = myID;
        itemName = myName;
        itemDescription = description;
        myToolType = type;
        sellPrice = sell;
        storePrice = store;
        canStack = stackable;
        canBeShipped = shippable;
        isTool = true;
        myScale = scale;

        if (!isTool)
            myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);
        else
            myPic = Resources.Load<Sprite>("Sprites/Tools/" + itemName);
        if (!myPic)
        {
            Debug.LogError($"Could not find item {itemName} image!");
        }
    }

    /// <summary>
    /// A constructor to copy an item
    /// </summary>
    /// <param name="item"></param>
    public Item(Item item)
    {
        itemID = item.itemID;
        itemName = item.itemName;
        itemDescription = item.itemDescription;
        hasStars = item.hasStars;
        starRating = item.starRating;
        sellPrice = item.sellPrice;
        storePrice = item.storePrice;
        isTool = item.isTool;
        canStack = item.canStack;
        canBeShipped = item.canBeShipped;
        myScale = item.myScale;

        if(!isTool)
            myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);
        else
            myPic = Resources.Load<Sprite>("Sprites/Tools/" + itemName);
        if (!myPic)
        {
            Debug.LogError($"Could not find item {itemName} image!");
        }
    }

    public virtual void SetStars(float stars)
    {
        starRating = stars;
    }
    public virtual float GetStars()
    {
        return starRating;
    }

    public void UpdateMyPic()
    {
        if (!isTool)
            myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);
        else
            myPic = Resources.Load<Sprite>("Sprites/Tools/" + itemName);
        if (!myPic)
        {
            Debug.LogError($"Could not find item {itemName} image!");
        }
    }

    public Sprite MyPic()
    {
        return myPic;
    }

    public void PrintItemData()
    {
        Debug.Log($"{itemName} ({itemID}): {itemDescription}");
    }
}
