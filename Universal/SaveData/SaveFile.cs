using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFile 
{
    [JsonProperty] private FarmSceneSaveInitData _farmData;
    [JsonProperty] private Inventory _playerInventoryData;
    [JsonProperty] private Dictionary<string, CharacterData_Scr> NPCData;
    [JsonProperty] private int _hour, _minute;
    [JsonProperty] private DayManager.FullDate  date; 
    [JsonProperty] private bool initialized = false;
    
    public SaveFile()
    {
        _farmData = FarmSceneDataStorage.Instance.GetData();
        _playerInventoryData = new PlayerInventory(15);
        NPCData = CharacterMaster_Scr.Instance.allCharacters;
        initialized = true;

        date = new DayManager.FullDate();
        date.day = 1; ;
        date.year = 1;
        date.season = GameController.Seasons.SPRING;
        _hour = 6; _minute = 0;
    }

    public void SaveFarmToFile(FarmSceneSaveInitData data)
    {
        _farmData = data; 
    }

    public void UpdateSaveData()
    {
        _farmData = FarmSceneDataStorage.Instance.GetData();
        _playerInventoryData = PlayerInventoryController.Instance.GetInventory();
        date = DayManager.Instance.GetCurrentDate();
        _hour = DayManager.Instance._hour;
        _minute = DayManager.Instance._minute;
        NPCData = CharacterMaster_Scr.Instance.allCharacters;
    }

    /// <summary>
    /// Save data for one specific character
    /// </summary>
    /// <param name="data">Pass the data of this specific character in</param>
    public void SaveCharacterData(CharacterData_Scr data)
    {
        if(!initialized) 
        { 
            Debug.LogWarning("This save file was not properly initialized. Aborting SaveFile.SaveCharacterData()");
        }
        NPCData[data.characterName] = data;
    }
}
