using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance; 
    public PlayerInventory playerInventory;

    [HideInInspector] public bool inMenu, inSceneTransition;

    private int _playerMoney;

    public enum Seasons
    {
        SPRING,
        SUMMER,
        FALL,
        WINTER
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        inMenu = false;
        inSceneTransition = false;
    }  
    
    
}
