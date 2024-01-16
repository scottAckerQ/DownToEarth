using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class CharacterData_Scr 
{
    public string characterName;
    [JsonProperty] private bool _giftGivenToday, _talkedToday;
    [JsonProperty] private int _currentAffection;
    [JsonProperty] private List<string> eventsSeen;
    [JsonProperty] private List<int> _lovedItems, _likedItems, _dislikedItems, _hatedItems;
    [JsonProperty] private Dictionary<string, string> _schedule;

    public CharacterData_Scr()
    {
        characterName = "Placeholder sorta";
        _giftGivenToday = true;
        _talkedToday = true;
        _currentAffection = 0;
        _schedule = new Dictionary<string, string>
        {
            { "6:30", "Farmhouse But not" },
            { "8:00", "Pond North But not" }
        };

        _lovedItems = new List<int>() { 2001 };
        _likedItems = new List<int>() { 1011 };
        _dislikedItems = new List<int>() { 2001 };
        _hatedItems = new List<int>() { 2001 };
    }

    public bool TalkedTo()
    {
       return  _talkedToday;
    }

    public bool GiftGiven()
    {
        return  _giftGivenToday;
    }

    public void ResetDailies()
    {
        _giftGivenToday = false;
        _talkedToday = false;
    }
    
    public void UpdateAffection(int affectionDelta)
    {
        _currentAffection += affectionDelta;
    }
    
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// To be used at the start of the game, loading all characters who exist in the game
    /// </summary>
    /// <param name="jsonPath">The path of the JSON </param>
    /// <returns></returns>
    public static CharacterData_Scr LoadCharacter(string characterName)
    {
        string jsonPath = $"NPCData/{characterName}_CharData";
        TextAsset json = Resources.Load<TextAsset>(jsonPath);
        string jsonString = json.ToString(); 
        CharacterData_Scr data = JsonConvert.DeserializeObject<CharacterData_Scr>(jsonString);

        return data;
    }

    public int GetAffectionLevel()
    {
        return _currentAffection;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Dictionary of a characters schedule, represented by a float (time of day) and string (name of destination)</returns>
    public Dictionary<string, string> GetSchedule()
    {
        return _schedule;
    }

    public void GiveItem(int id)
    {
        int affectionDelta;
        if (_lovedItems.Contains(id))
        {
            affectionDelta = 500;
        }
        else if (_likedItems.Contains(id))
        {
            affectionDelta = 300;
        }
        else if (_dislikedItems.Contains(id))
        {
            affectionDelta = -150;
        }
        else if (_hatedItems.Contains(id))
        {
            affectionDelta = -350;
        }
        else
        {
            affectionDelta = 100;
        }

        _currentAffection += affectionDelta;
    }
    public void PrintCharacterData()
    {
        Debug.Log($"Printing data of character: {characterName}");

        string sched = $"Schedule x{_schedule.Count}:" ;
        foreach (string scheduleItem in _schedule.Values)
        {
            sched += $" {scheduleItem},";

        }

        Debug.Log(sched);

        string loved = $"loved items x{_lovedItems.Count}:";
        foreach (int item in _lovedItems)
        {
            loved += $" {item},";
        }
        Debug.Log(loved);
    }
}
