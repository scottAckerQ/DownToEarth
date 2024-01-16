using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DirtInitData 
{
    [JsonProperty] private string _initName;
    [JsonProperty] private float _xPos;
    [JsonProperty] private float _yPos;
    [JsonProperty] private bool _tilled;
    [JsonProperty] private bool _hasCrop;

    [JsonProperty] private int _todaysWaterPoints;
    

    public DirtInitData()
    {
        
    }

    public void SetInitDataToObject(Dirt dirtObject)
    {
        _initName = $"Dirt #{dirtObject.transform.GetSiblingIndex()}";
        dirtObject.name = _initName;


        _xPos = dirtObject.transform.position.x;
        _yPos = dirtObject.transform.position.y;

        _tilled = dirtObject.Tilled;
        _hasCrop = dirtObject.HasCrop;
        _todaysWaterPoints = dirtObject.TodaysWaterPoints;
    }

    [JsonIgnore] public bool Tilled => _tilled;
    [JsonIgnore] public bool HasCrop => _hasCrop;
    [JsonIgnore] public int TodaysWaterPoints => _todaysWaterPoints;
    [JsonIgnore] public string InitName => _initName;
    [JsonIgnore] public Vector2 Position => new Vector2(_xPos, _yPos);

    public void CopyInitData(DirtInitData data)
    {
        _tilled = data.Tilled;
        _todaysWaterPoints = data.TodaysWaterPoints;
        _initName = data.InitName;
        _hasCrop = data.HasCrop;
        _xPos = data._xPos; 
        _yPos = data._yPos;
    }

    public void PrintInitData()
    {
        Debug.Log($"{_initName} init data \nT: {_tilled}\nWP: {_todaysWaterPoints}\nC: {_hasCrop}");
    }
}
