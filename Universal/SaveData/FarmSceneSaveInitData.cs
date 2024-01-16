/*****************************************************************************
// File Name: SceneSaver.cs
// Author:
// Creation Date: 
//
// Brief Description: Saves and loads scene exactly how it was when exited
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[Serializable]
public class FarmSceneSaveInitData
{
    public Dictionary<int, DirtInitData> dirtStates;
    public Dictionary<int, CropInitData> cropStates;

    public FarmSceneSaveInitData()
    {
        dirtStates = new Dictionary<int, DirtInitData>();
        cropStates = new Dictionary<int, CropInitData>(); 
    }

    public string SaveToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static FarmSceneSaveInitData Load(string jsonPath)
    {  
        
        string jsonString = File.ReadAllText(jsonPath).ToString(); 

        FarmSceneSaveInitData data = JsonConvert.DeserializeObject<FarmSceneSaveInitData>(jsonString);
        
        return data;
    }

}
