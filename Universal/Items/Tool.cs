/*****************************************************************************
// File Name: Tool.cs
// Author:
// Creation Date: 2/25/2020
//
// Brief Description:
*****************************************************************************/

using UnityEngine;

public class Tool
{
    public Sprite myPic;

    public int itemID;
    public string itemName;
    public string itemDescription;

    public bool canStack;
    //public bool IsTool;

    public int sellPrice;
    public int storePrice;

    public enum ToolType
    {
        WCan,
        Hoe,
        Seeds,
        Axe,
        Hammer,
        Rod
    }
    public ToolType myTool;

    public Tool(int myID, string myName, string description,
        ToolType type, bool stackable,
        int sell, int store)
    {
        itemID = myID;
        itemName = myName;
        itemDescription = description;
        myTool = type;
        sellPrice = sell;
        storePrice = store;
        canStack = stackable;

        myPic = Resources.Load<Sprite>("Sprites/Tools/" + itemName);
    }

    public Tool(Tool tool)
    {
        itemID = tool.itemID;
        itemName = tool.itemName;
        itemDescription = tool.itemDescription;
        myTool = tool.myTool;
        sellPrice = tool.sellPrice;
        storePrice = tool.storePrice;
        canStack = tool.canStack;

        myPic = Resources.Load<Sprite>("Sprites/Items/" + itemName);

    }
}
