using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Dirt : ScaledTileObject_Scr, Interactable
{
    private DirtInitData _initData;

    [SerializeField][JsonProperty] private bool _tilled;
    [JsonProperty] private bool _hasCrop;
    [JsonProperty] private int _todaysWaterPoints;

    private Animator _anim;
    [JsonIgnore] public GameObject myCropObject;
    private Crop _myCrop;

    private void Awake()
    {
        _myCrop = myCropObject.GetComponent<Crop>();
        _myCrop.AssignDirt(this);

        _anim = GetComponent<Animator>();
        _todaysWaterPoints = 0;
    }

    public void Start()
    {
        name = $"Dirt #{transform.GetSiblingIndex()}";
    }

    public void InitializeDirt()
    {
         
    }

    public void Till()
    {
        if (!_tilled)
        {
            _tilled = true;
            UpdateAnimation();
        }
    }

    public void Plant(int cropId)
    {
        _myCrop.InitializeCrop(CropDatabase.instance.Crops[cropId]);
        HasCrop = true;
    }

    public void Water()
    {
        if (!_tilled) return;

        if (_todaysWaterPoints < 4)
        {
            _todaysWaterPoints++;
            UpdateAnimation();
        }
    }

    public void Dry()
    {
        if (_tilled)
        {
            UpdateAnimation();
            _todaysWaterPoints = 0;
        }
    }

    public void AdvanceDay()
    {
        if(_hasCrop)
        {
            _myCrop.AdvanceDay();
        }
    }

    public void DisconnectCrop()
    { 
        _hasCrop = false;
    }

    public int GetWaterPoints()
    {
        return _todaysWaterPoints;
    }

    public void UpdateInitDataForSaving()
    {
        if (_initData == null) InitData = new DirtInitData();
        _initData.SetInitDataToObject(this);
    }

    public void LoadInitData(DirtInitData dirtData)
    {
        if (_initData == null) InitData = new DirtInitData();
        _initData.CopyInitData(dirtData);
        ReflectInitData();
    }

    public void ReflectInitData()
    {
        name = InitData.InitName;
        _tilled = _initData.Tilled;
        _todaysWaterPoints = _initData.TodaysWaterPoints;
        name = _initData.InitName;
        HasCrop = _initData.HasCrop;
        //Debug.Log($"Save {name}, T: {_initData.Tilled}, WP:{_initData.TodaysWaterPoints}");

        //Debug.Log($"Field {name}, T: {_tilled}, WP:{_todaysWaterPoints}");
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        _anim.SetBool("Tilled", _tilled);
        _anim.SetInteger("WaterPoints", _todaysWaterPoints);
    }

    //Harvests crop
    public void Interact(GameObject player)
    {
        if (!PlayerInventoryController.Instance.InventoryHasSpace(_myCrop.ItemID)) return;
        if (!_myCrop.FullyGrown) return;

        Item itemData = ItemDatabase.instance.GetItem(_myCrop.ItemID);
        if(itemData.hasStars) itemData.starRating = _myCrop.FinalStarValue;
        PlayerInventoryController.Instance.AddToInventory(itemData);

        _hasCrop = false;
        _myCrop.ResetCrop();
    }

    public Crop MyCrop { get => _myCrop; set => _myCrop = value; }
    public bool Tilled { get => _tilled; set => _tilled = value; }
    public bool HasCrop { get => _hasCrop; set => _hasCrop = value; }
    public int TodaysWaterPoints { get => _todaysWaterPoints; set => _todaysWaterPoints = value; }
    
    public DirtInitData InitData
    {
        get => _initData; 
        set
        {
            _initData = value;
            //ReflectInitData(); 
        }
    }

    public ScaleManager_Scr.ObjectScales myScale 
    { get => ScaleManager_Scr.ObjectScales.Human;}

   public bool CanInteract(PlayerController player)
    {
        return true;
    }    

    public void GetConnectedTiles()
    {
        Vector3Int myPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        //TileBase myTile = TileScaleManager.Instance.tillableGroundTilemap_h.GetTile(myPosition);
    }
}
