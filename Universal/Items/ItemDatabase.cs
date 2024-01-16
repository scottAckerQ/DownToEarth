/*****************************************************************************
// File Name: ItemDatabase.cs
// Author:
// Creation Date: 
//
// Brief Description:
*****************************************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;
    private List<Item> _items = new List<Item>();

    private void Awake()
    {
        BuildDatabase(); 
        if(!instance)
        {
            instance = this;
        }else
        { 
            Destroy(this);
        }
    }

    public Item GetItem(int id)
    {
        return _items.Find(item => item.itemID == id);
    }

    public Item GetItem(string itemName)
    {
        return _items.Find(item => item.itemName == itemName);
    }

    public string GetItemNameForLoad(int id)
    {
        //TODO: Find more efficient method
        string str =  _items.Find(item => item.itemID == id).itemName;
        str = str.Replace(" ", string.Empty);
        str = str.Replace("Seeds", string.Empty);
        return str;
    }

    private void BuildDatabase()
    {
        //ID, Name, Description, Sell price, Store price 
        TextAsset json = Resources.Load<TextAsset>("ItemDatabaseList");
        string jsonString = json.ToString(); 
        _items = JsonConvert.DeserializeObject<List<Item>>(jsonString);
         
        foreach (Item item in _items)
        {
            item.UpdateMyPic(); 
        }
    }

    public void GenerateExampleJSON()
    { 
        string jsonPath = $"{Application.persistentDataPath}/ItemDatabaseList.json";

        string json = JsonConvert.SerializeObject(_items, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
    }
}
