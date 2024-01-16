using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    public static Action<string> TimeChange;
    public static DayManager Instance;
    public Image colorOverlay;
    public Image darknessMask;
    [FormerlySerializedAs("overlayColors")] public Color[] dayColors;
    private int[] _changeHours = {3, 6, 8, 17, 20};

    private const int _realTimeIncrements = 3;

    [HideInInspector]public int _minute;
    [HideInInspector]public int _hour = 6;
    [HideInInspector]public int _day = 1;
    [HideInInspector]public int _year = 1;

    [HideInInspector]public GameController.Seasons _currentSeason;

    private PlayerController _player;

    private Inventory _savedShippingInventory;
    private bool _shippingSavedToday;
    private int _shippingValue;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            _hour = 6;
            _minute = 00;
            _currentSeason = GameController.Seasons.SPRING;
            _day = 1;
            _shippingSavedToday = false;
            _savedShippingInventory = new Inventory();
        }
    }

    private void Start()
    {
        UIComponentReference.instance.UpdateTime(_hour,_minute);
        UIComponentReference.instance.UpdateDate(_currentSeason,_day);

        StartCoroutine(TimePassage());

        _currentSeason = GameController.Seasons.SPRING;

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //_shipBox = GameObject.FindGameObjectWithTag("Shipping Box").GetComponent<ShippingBox_Scr>();
    }

    private IEnumerator TimePassage()
    {
        while(!GameController.Instance.inMenu)
        {
            yield return new WaitForSeconds(_realTimeIncrements); 
            _minute += 1;
            TimeChange.Invoke($"{_hour}:{_minute}0");

            if (_minute == 6)
            {
                _hour++;
                _minute = 0;
                

                darknessMask.enabled = IsNightHour();
                darknessMask.GetComponent<Mask>().enabled = IsNightHour();

                if (_hour == 24)
                {
                    AdvanceDay();
                }
            }

            for(int i = 0; i < _changeHours.Length; i++)
            {
                if (_hour == _changeHours[i])
                {
                    UpdateOverlayColor(dayColors[i], 60);
                    break;
                }
            }

            UIComponentReference.instance.UpdateTime(_hour, _minute);
        }
    }

    public void AdvanceDay()
    {
        if (_day == 30)
        {
            //change month to next month
            switch (_currentSeason)
            {
                case GameController.Seasons.SPRING:
                    _currentSeason = GameController.Seasons.SUMMER;
                    break;
                case GameController.Seasons.SUMMER:
                    _currentSeason = GameController.Seasons.FALL;
                    break;
                case GameController.Seasons.FALL:
                    _currentSeason = GameController.Seasons.WINTER;
                    break;
                case GameController.Seasons.WINTER:
                    _year++;
                    _currentSeason = GameController.Seasons.SPRING;
                    break;
            }
            _day = 0;
        }
        //make date move ahead by one
        _day++;

        UIComponentReference.instance.UpdateDate(_currentSeason,_day);

        //set time to 6AM
        _hour = 6;
        _minute = 0;
        
        UIComponentReference.instance.UpdateTime(_hour,_minute);

        GetDayProfits();

    }

    public void SaveShippingData()
    {
        ShippingBox_Scr _shipBox = GameObject.FindGameObjectWithTag("Shipping Box").GetComponent<ShippingBox_Scr>();
        
        if(_shipBox != null )
        {
            _shippingSavedToday = true;
            _savedShippingInventory = _shipBox.shippingInventory;
            _shippingValue = _shipBox._shippingValue;
        } 
    }

    public Inventory GetSavedShippingInventory()
    {
        return _savedShippingInventory;
    }

    public int GetSavedShippingValue()
    {
        return _shippingValue;
    }

    public bool HasSavedShippingToday()
    {
        return _shippingSavedToday;
    }

    /// <summary>
    /// Returns total profits for the day and resets count
    /// </summary>
    /// <returns></returns>
    public void GetDayProfits()
    {
        PlayerInventoryController.Instance.PlayerMoney += _shippingValue;
        _savedShippingInventory.EmptyInventory();
        _shippingValue = 0;
        _shippingSavedToday = false;
    }

    public FullDate GetCurrentDate()
    {
        FullDate date = new FullDate(_currentSeason, _day, _year);
        return date;
    }

    public static int DaysDifference(FullDate d1, FullDate d2)
    {
        return Mathf.Abs(DateToDays(d1) - DateToDays(d2));
    }
   
    /// <summary>
    /// Gives the difference in days between the current day and the provided date
    /// </summary>
    /// <param name="d1"></param>
    /// <returns></returns>
    public int DaysDifference(FullDate d1)
    {
        return Mathf.Abs(DateToDays(d1) - DateToDays(GetCurrentDate()));
    }

    private static int DateToDays(FullDate date)
    {
        int result = 0;

        result += date.year * 120;
        result += ((int)date.season * 30) + date.day;

        return result;
    }

    public struct FullDate
    {
        public int year;
        public int day;
        public GameController.Seasons season;

        public FullDate(GameController.Seasons Season, int Day, int Year )
        {
            season = Season;
            day = Day;
            year = Year;
        }

       
    }

    private IEnumerator UpdateOverlayColor(Color newColor, int timing)
    {
        float range = Vector4.Distance(colorOverlay.color, newColor);
        float deltaAmount = range / timing * Time.deltaTime;

        while (true)
        {
            colorOverlay.color = Vector4.MoveTowards(colorOverlay.color, newColor, deltaAmount);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool IsNightHour()
    {
        return _hour <= _changeHours[0] || _hour >= _changeHours[4] ;
    }
}
