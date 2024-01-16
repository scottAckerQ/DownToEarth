using System;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private CropInitData _initData;

    //public GameObject itemToGive;
    private int _cropItemID;
    private string _cropName;

    private Dirt _myDirt;
    public Animator anim;

    private int[] perfectWateringPoints; //Refers to how much water each crop needs for each stage
    protected int[] eachStageStars; //Array of how many stars were gained in each stage
    protected int numberOfStages; /// How many stages this plant has, not including fully grown
    protected int[] stageTimes;

    private int finalStarValue; //How many stars the crop has at the end
    private int _currentStage;
    private int _daysInCurrentStage;///How many days this plant has been growing in its current stage
    private int _waterPointsInCurrentStage; //How much the crop has been watered in this stage
    private bool _fullyGrown;
    private bool _active;

    private GameController.Seasons _growingSeason;
    private bool _dead;
    public virtual void Awake()
    {
        //default values
        numberOfStages = 1;
        eachStageStars = new int[1];
        stageTimes = new[]{1}; 
        perfectWateringPoints = new[] {1};

        _currentStage = 0;
        _dead = false;
        _fullyGrown = false;
        _active = false;

        _waterPointsInCurrentStage = 0;
        _daysInCurrentStage = 0;
    }

    public void InitializeCrop(CropDatabase.CropInfo info)
    {
        //make a new instance of init data in case it does not yet exist (during loading)
        if (_initData == null) _initData = new CropInitData(); 

        anim = GetComponent<Animator>();

        _cropName = info.cropName;
        _cropItemID = info.itemID;
        stageTimes = info.stageTimes;
        perfectWateringPoints = info.perfectWateringPoints;
        numberOfStages = info.numberOfStages;
        _growingSeason = info.growingSeason;


        _currentStage = 0;
        _dead = false;
        _fullyGrown = false;
        _active = true;
        _waterPointsInCurrentStage = 0;
        _daysInCurrentStage = 0;

        eachStageStars = new int[numberOfStages];

        
        Debug.Log($"{_cropName} has been planted");
        
        FindAnimationController();
        ReflectCurrentAnimation();
    }

    public void FindAnimationController()
    {
        anim.runtimeAnimatorController = 
            Resources.Load<AnimatorOverrideController>($"AnimationControllers/{_cropName}_AnimControl");
    }

    public void ReflectCurrentAnimation()
    {
        if (anim.runtimeAnimatorController)
        { 
            anim.SetInteger("Stage", CurrentStage);
            anim.SetBool("ReadyToHarvest", _fullyGrown);
            anim.SetBool("Empty", !_active);
        }
    }

    /// <summary>
    /// Adds a new star rating to the array of values.
    /// </summary>
    private void AddStageStars()
    {
        int newStarValue = 5;

        //calculate how many stars this one should earn
        int perfectPoints = perfectWateringPoints[_currentStage];
        //print(perfectPoints + "point for five stars");
        int averagePoints = stageTimes[_currentStage];
        //print(averagePoints + "point for 3 stars");


        float oneStarValue = perfectPoints - averagePoints;
        //print(oneStarValue + "point for 1 stars");


        int difference = Mathf.Abs(_waterPointsInCurrentStage - perfectPoints); //How far off this crop was from perfect
        //print(difference + " points off from perfect");


        if (difference == 0)
        {
            newStarValue = 5;
        }
        else
        {
            //print("dividing " + waterPointsInCurrentStage + " by " + oneStarValue );
            float newFloatValue = newStarValue - (_waterPointsInCurrentStage / oneStarValue);
            newStarValue = Mathf.RoundToInt(newFloatValue);
            
        }

        //print("Star value for stage " + currentStage + ": " + newStarValue);
        eachStageStars[_currentStage] = newStarValue;
    }

    /// <summary>
    /// Finds the average of all stored star values
    /// </summary>
    private void CalculateAverageStars()
    {
        int totalValue = 0;
        foreach(int item in eachStageStars)
        {
            totalValue += item;
        }

        totalValue = Mathf.RoundToInt(totalValue / eachStageStars.Length);

        finalStarValue = totalValue;
        //print("Final star value: " + finalStarValue);
    }

    //Called when a crop is planted 
    public void AssignDirt(Dirt thisPlot)
    {
        MyDirt = thisPlot;
    }
    
    /// <summary>
    /// Add a day to the number of days it has been growing and delete its watering. 
    /// If the plant was dried out this day, but was watered, restore it to a normal state.
    /// If the plant was not dried out but was watered, give it a day.
    /// If the plant was not watered and is not fully grown, it is now dried out.
    /// </summary>
    public void AdvanceDay()
    {
        _daysInCurrentStage++;
        _waterPointsInCurrentStage += MyDirt.GetWaterPoints();
        MyDirt.Dry();

        //if(waterPointsInCurrentStage > stageTimes[currentStage] * 2) //kill crop if its water level is double the minimum requirement
        //{
        //    KillCrop();
        //}

        if(!_fullyGrown && !_dead)
        {
            if (_daysInCurrentStage >= stageTimes[_currentStage]  //Crop must meet minimum days requirement to advance a stage
                && _waterPointsInCurrentStage >= stageTimes[_currentStage]) //minimum water requirement
            {
                AdvanceStage();
                Debug.Log("Now advancing a stage");
            }
            else
            {
                print("not ready to upgrade yet");
                print("This crop has " + (stageTimes[_currentStage] - _daysInCurrentStage) + " days " +
                    "and " + (stageTimes[_currentStage] - _waterPointsInCurrentStage) + "water points to go");
            }
        }
    }

    /// <summary>
    /// Change the current stage os the plant to the one that follows it, depending on the number of stages
    /// </summary>
    public void AdvanceStage()
    {
        if(!_fullyGrown)
        {
            //Add star value of current stage to array
            AddStageStars();

            _currentStage++;
            ReflectCurrentAnimation();

            //reset values
            _waterPointsInCurrentStage = 0;
            _daysInCurrentStage = 0;

            if(_currentStage == numberOfStages )
            {
                CalculateAverageStars();
                EndGrowth();
            }
        }
    }

    private void EndGrowth()
    {
        _fullyGrown = true;
    }

    public bool SameSeason(GameController.Seasons season)
    {
        return (season.Equals(_growingSeason));
    }

    public void Harvest()
    {
        //TODO: Add instructions for crops with multiple harvests
        ResetCrop();
    }

    public void KillCrop()
    {
        _dead = true;
        _active = false;
        //TODO: Set sprite to dead sprite
    }

    public void ResetCrop()
    {
        _active = false;
        _currentStage = 0;
        _fullyGrown = false;

        ReflectCurrentAnimation();
    }

    public void UpdateInitDataForSaving()
    {
        if (_initData == null) InitData = new CropInitData();
        _initData.SetInitDataToObject(this);
    }

    public void LoadInitData(CropInitData cropData)
    {
        if (_initData == null) InitData = new CropInitData();
        _initData.CopyInitData(cropData);
        ReflectInitData();
    }

    public void ReflectInitData()
    {
        _cropName = _initData.CropName;
        _cropItemID = _initData.ItemID;
        _currentStage = _initData.CurrentStage;
        _daysInCurrentStage = _initData.CurrentDays;
        _waterPointsInCurrentStage = _initData.CurrentWater;
        _fullyGrown = _initData.FullyGrown;
        _active = _initData.Active;
        _dead = _initData.Dead;

        FindAnimationController();
        ReflectCurrentAnimation();
    }

    public int GetItemID()
    { return _cropItemID; }

    public void PrintAttributes()
    {
        print("New Crop: " + _cropItemID);
        print("Number of stages: " + numberOfStages);

        string result = "Stage Times: [";
        for (int i = 0; i < stageTimes.Length; i++)
        {
            result += stageTimes[i] + " ";
        }
        result += "]";
        print(result);

        result = "Perfect Times: [";
        for (int i = 0; i < perfectWateringPoints.Length; i++)
        {
            result += perfectWateringPoints[i] + " ";
        }
        result += "]";
        print(result);
    }

    public CropInitData InitData
    {
        get => _initData; 
        set
        {
            _initData = value;
            ReflectInitData();
        }
    }

    public int ItemID { get => _cropItemID; set => _cropItemID = value; }
    public int CurrentStage { get => _currentStage; set => _currentStage = value; }
    public int CurrentDays { get => _daysInCurrentStage; set => _daysInCurrentStage = value; }
    public int CurrentWater { get => _waterPointsInCurrentStage; set => _waterPointsInCurrentStage = value; }
    public int FinalStarValue { get => finalStarValue; set => finalStarValue = value; }
    public bool FullyGrown { get => _fullyGrown; set => _fullyGrown = value; }
    public bool Active { get => _active; set => _active = value; }
    public bool Dead { get => _dead; set => _dead = value; }

    public GameController.Seasons GrowingSeason { get => _growingSeason; set => _growingSeason = value; }
    public Dirt MyDirt { get => _myDirt; set => _myDirt = value; }
    public string CropName { get => _cropName; set => _cropName = value; }
}
