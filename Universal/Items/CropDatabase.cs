using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDatabase : MonoBehaviour
{
    public static CropDatabase instance;
    private Dictionary<int, CropInfo> _crops;

    public Dictionary<int, CropInfo> Crops { get => _crops; }

    private void Awake()
    {
        if (!instance)
        {
            InitializeDatabase();
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public class CropInfo
    {
        public readonly string cropName;
        public readonly int[] perfectWateringPoints; //Refers to how much water each crop needs for each stage
        public readonly int[] stageTimes;
        public readonly int numberOfStages; /// How many stages this plant has, not including fully grown
        public readonly GameController.Seasons growingSeason;
        public readonly int itemID;

        public CropInfo()
        {
            cropName = "PLACEHOLDER";
            perfectWateringPoints = new int[]{ 1, 0};
            stageTimes = new int[]{ 1, 0};
            numberOfStages = 2;
            growingSeason = GameController.Seasons.SPRING;
            itemID = 0;
        }

        public CropInfo(int id, string thisName, int[] pWater, int[] sTimes, GameController.Seasons season)
        {
            itemID = id;
            cropName = thisName;
            perfectWateringPoints = pWater;
            stageTimes = sTimes;
            numberOfStages = stageTimes.Length;
            growingSeason = season;
        }
    }

    private void InitializeDatabase()
    {
        _crops = new Dictionary<int, CropInfo>()
        {
            { 
                0001, new CropInfo( 2001,
                    "Turnip",
                    new int[] { 3, 3 }, //How much water this crop needs for each stage
                    new int[] { 2, 2 }, //How long this crop stays in each stage
                    GameController.Seasons.SPRING )
            },
            {
                0002, new CropInfo( 2002,
                    "Potato",
                    new int[] { 2, 2, 2 }, //How much water this crop needs for each stage
                    new int[] { 2, 1, 2 }, //How long this crop stays in each stage
                    GameController.Seasons.SPRING )
            }
        };
    }

}
