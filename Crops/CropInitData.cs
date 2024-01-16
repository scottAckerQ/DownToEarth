using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class CropInitData
{
    [JsonProperty] private string _cropName;
    [JsonProperty] private int _cropItemID;
    [JsonProperty] private int _currentStage;
    [JsonProperty] private int _daysInCurrentStage;///How many days this plant has been growing in its current stage
    [JsonProperty] private int _waterPointsInCurrentStage; //How much the crop has been watered in this stage
    [JsonProperty] private bool _fullyGrown;
    [JsonProperty] private bool _active;
    [JsonProperty] private bool _dead;
    [JsonProperty] private int _finalStarValue;

    [JsonIgnore] public int ItemID => _cropItemID;
    [JsonIgnore] public int CurrentStage => _currentStage;
    [JsonIgnore] public int CurrentDays => _daysInCurrentStage;
    [JsonIgnore] public int CurrentWater => _waterPointsInCurrentStage;
    [JsonIgnore] public bool FullyGrown => _fullyGrown;
    [JsonIgnore] public bool Dead => _dead;
    [JsonIgnore] public int FinalStarValue => _finalStarValue;
    [JsonIgnore] public int CropItemID { get => _cropItemID; }
    [JsonIgnore] public int DaysInCurrentStage { get => _daysInCurrentStage; }
    [JsonIgnore] public bool Active { get => _active; }
    [JsonIgnore] public string CropName { get => _cropName; set => _cropName = value; }

    public CropInitData()
    {
        
    }

    public void SetInitDataToObject(Crop cropObject)
    {
        _cropName = cropObject.CropName;
        _cropItemID = cropObject.ItemID;
        _currentStage = cropObject.CurrentStage;
        _daysInCurrentStage = cropObject.CurrentDays;
        _waterPointsInCurrentStage = cropObject.CurrentWater;
        _fullyGrown = cropObject.FullyGrown;
        _active = cropObject.Active;
        _dead = cropObject.Dead;
        _finalStarValue = cropObject.FinalStarValue;
    }

    public void CopyInitData(CropInitData data)
    {
        _cropName = data.CropName;
        _cropItemID = data.ItemID;
        _currentStage = data.CurrentStage;
        _daysInCurrentStage = data.CurrentDays;
        _waterPointsInCurrentStage = data.CurrentWater;
        _fullyGrown = data.FullyGrown;
        _active = data.Active;
        _dead = data.Dead;
        _finalStarValue = data.FinalStarValue;
    }
}
