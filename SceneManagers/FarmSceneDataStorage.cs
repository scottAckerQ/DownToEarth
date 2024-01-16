using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;

public class FarmSceneDataStorage : MonoBehaviour
{
    [HideInInspector] public static FarmSceneDataStorage Instance;
    [JsonProperty] private Dictionary<int, DirtInitData> _savedDirtDataList = new Dictionary<int, DirtInitData>();
    [JsonProperty] private Dictionary<int, CropInitData> _savedCropDataList = new Dictionary<int, CropInitData>();

    private FarmSceneSaveInitData farmSceneSave;
    private string farmSceneFilePath;

    private DayManager.FullDate _lastDateRecorded;

    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        farmSceneFilePath = $"{Application.persistentDataPath}/FarmScene.json";
    }

    public void SaveFarmData()
    {
        if (farmSceneSave == null)
        {
            farmSceneSave = new FarmSceneSaveInitData();
        }

        _lastDateRecorded = DayManager.Instance.GetCurrentDate();

        RecordDirtAndCropStatus();
        string json = farmSceneSave.SaveToString(); 
        File.WriteAllText(farmSceneFilePath, json);
    }

    public void LoadFarmScene()
    {
        if (!File.Exists(farmSceneFilePath))
        {
            Debug.LogError("Could not find file");
        }

        farmSceneSave = FarmSceneSaveInitData.Load(farmSceneFilePath);
        _savedDirtDataList = farmSceneSave.dirtStates; 
        _savedCropDataList = farmSceneSave.cropStates;
        SetCropsFromData();
    }

    //public void AddDirtData(Dirt newDirt)
    //{
    //    if(_allDirtObjects == null) _allDirtObjects = new Dictionary<int, Dirt>();
        
    //    _allDirtObjects.Add(_allDirtObjects.Count, newDirt);
    //}

    //public void ResetDirtData()
    //{
    //    _allDirtObjects.Clear();
    //}

    private void RecordDirtAndCropStatus()
    {
        _savedDirtDataList = new Dictionary<int, DirtInitData>();
        _savedCropDataList = new Dictionary<int, CropInitData>();

        //save each dirt and crop as members of this dictionary
        foreach(KeyValuePair<int, Dirt> entry in FarmSceneReference.Instance.GetAllDirt())
        {
            entry.Value.UpdateInitDataForSaving();
            entry.Value.MyCrop.UpdateInitDataForSaving();
            _savedDirtDataList.Add(entry.Key, entry.Value.InitData);
            _savedCropDataList.Add(entry.Key, entry.Value.MyCrop.InitData);
            
        }

        farmSceneSave.dirtStates = _savedDirtDataList;
        farmSceneSave.cropStates = _savedCropDataList;
    }

    public void SetCropsFromData()
    {
        int daysDifference = DayManager.Instance.DaysDifference(_lastDateRecorded) ;
        Dictionary<int, Dirt> _allDirtObjects = FarmSceneReference.Instance.GetAllDirt(); 

        if (_allDirtObjects == null || _allDirtObjects.Count == 0)
        {
            Debug.LogError("SA: All dirt objects were not correctly saved or loaded.");
            return;
        } 

        for (int i = 0; i < farmSceneSave.dirtStates.Count; i++)
        {
            _allDirtObjects[i].LoadInitData( _savedDirtDataList[i]);
            _allDirtObjects[i].MyCrop.LoadInitData(_savedCropDataList[i]);
            for (int f = 0; f < daysDifference; f++) { _allDirtObjects[i].AdvanceDay(); }
        }
    }

    public FarmSceneSaveInitData GetData()
    {
        return farmSceneSave;
    }

    public bool FarmSaveExists()
    {
        return farmSceneSave != null;
    }
}
