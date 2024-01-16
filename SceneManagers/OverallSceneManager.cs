using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class OverallSceneManager : MonoBehaviour
{
    public static OverallSceneManager Instance;
    private const string FarmSceneName = "FarmScene";
    private const string HouseSceneName = "PlayerHouseScene";
    private const string BarnSceneName = "BarnScene";

    private Vector2 FarmSpawnPos_FromHouse = new Vector3(0.6f, 26f);
    private Vector2 FarmSpawnPos_FromBarn = new Vector3(-62.43f, 28.6f);

    private Vector2 HouseSpawnPos_FromFarm = new Vector3(0, -3.63000011f);
    private Vector2 BarnSpawnPos_FromFarm = new Vector3(0, -19.38f);

    private Vector2 FarmSpawnPos_FromTown = new Vector3(-10, 50);


    private float _enteredZoneTime;
    private const float ZoneChangeCooldown = 1.5f;

    private Transform playerTransform;
    public enum Zone
    {
        FARM,
        HOUSE,
        BARN,
        PONDFARM,
        TOWN_EAST,
        TOWN_WEST
    }

    public Zone currentZone = Zone.FARM;

    public void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            this.currentZone = Zone.FARM;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    public IEnumerator LoadFarm(Door doorway)
    {
        if (CanChangeZones())
        {
            StartCoroutine(UIComponentReference.instance.BlackBoxFadeIn(.5f));
            yield return new WaitForSeconds(.5f);

            GameController.Instance.inSceneTransition = true;
            SceneManager.LoadScene(FarmSceneName);

            while (SceneManager.GetActiveScene().name != FarmSceneName)
            {
                yield return null;
            }

            Vector3 spawnPos = Vector3.zero;

            if (currentZone == Zone.HOUSE)
            {
                spawnPos = FarmSpawnPos_FromHouse;
            }
            else if (currentZone == Zone.BARN)
            {
                spawnPos = FarmSpawnPos_FromBarn;
            }

            _enteredZoneTime = Time.realtimeSinceStartup;
            playerTransform.position = spawnPos;

            FarmSceneDataStorage.Instance.LoadFarmScene();
            StartCoroutine(UIComponentReference.instance.BlackBoxFadeOut(.5f));
            yield return new WaitForSeconds(.5f);

            GameController.Instance.inSceneTransition = false;
            currentZone = Zone.FARM;

            Destroy(doorway.gameObject);
        }    
        
    }

    public IEnumerator LoadHouse()
    {
        if (CanChangeZones())
        {
            CheckToSaveFarm();

            StartCoroutine(UIComponentReference.instance.BlackBoxFadeIn(.5f));
            yield return new WaitForSeconds(.5f);

            GameController.Instance.inSceneTransition = true;
            SceneManager.LoadScene(HouseSceneName);
            currentZone = Zone.HOUSE;
            _enteredZoneTime = Time.realtimeSinceStartup;
            playerTransform.position = HouseSpawnPos_FromFarm;

            StartCoroutine(UIComponentReference.instance.BlackBoxFadeOut(.5f));
            yield return new WaitForSeconds(.5f);
            GameController.Instance.inSceneTransition = false;
        }
    }

    public IEnumerator LoadBarn()
    {
        if (CanChangeZones())
        {
            CheckToSaveFarm();

            StartCoroutine(UIComponentReference.instance.BlackBoxFadeIn(.5f));
            yield return new WaitForSeconds(.5f);

            GameController.Instance.inSceneTransition = true;
            SceneManager.LoadScene(BarnSceneName);
            currentZone = Zone.BARN;
            _enteredZoneTime = Time.realtimeSinceStartup;
            playerTransform.position = BarnSpawnPos_FromFarm;

            StartCoroutine(UIComponentReference.instance.BlackBoxFadeOut(.5f));
            yield return new WaitForSeconds(.5f);
            GameController.Instance.inSceneTransition = false;
        }
    }

    private void CheckToSaveFarm()
    {
        if (currentZone == Zone.FARM)
        {
            FarmSceneDataStorage.Instance.SaveFarmData();  
        }
    }

    private bool CanChangeZones()
    {
        return (Time.realtimeSinceStartup - _enteredZoneTime > ZoneChangeCooldown);
    }

    public Zone GetCurrentZone()
    {
        return currentZone;
    }
}
