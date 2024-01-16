using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScaleManager_Scr : MonoBehaviour
{
    public static ScaleManager_Scr Instance;
    public GameObject environmentMapParent_h, environmentMapParent_a;
    public Tilemap tillableGroundTilemap_h, tillableGroundTilemap_a;
    public float cameraSize_h, cameraSize_a;
    public float playerSize_h, playerSize_a;

    public Action<ObjectScales> onScaleChange;
    [Serializable]
    public enum ObjectScales
    {
        Human, Alien, Hybrid
    }

    public ObjectScales currentPov;
    public ObjectScales currentSize;
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

    // Start is called before the first frame update
    void Start()
    {
        GoToHumanPov();
        
        currentPov = ObjectScales.Alien; 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            print("Switching scale");
            if (IsAlienPov()) GoToHumanPov();
            else GoToAlienPov();
        }
    }

    

    public void GoToHumanPov()
    {
        currentPov = ObjectScales.Human;
        SetTilePov(ObjectScales.Human);
        GrowPlayer();
    }

    public void GoToAlienPov()
    {
        currentPov = ObjectScales.Alien;
        SetTilePov(ObjectScales.Alien);
        ShrinkPlayer();
    }

    private void SetTilePov(ObjectScales newScale)
    {       
        Camera.main.orthographicSize = IsAlienPov() ? cameraSize_a : cameraSize_h;

        //environmentMapParent_a.SetActive(IsAlienPov());
        //environmentMapParent_h.SetActive(!IsAlienPov());

        if (onScaleChange == null) return;
        onScaleChange.Invoke(newScale);
    }

    private void GrowPlayer()
    {
        PlayerController.Instance.transform.localScale = Vector3.one * playerSize_h;
        SetTilePov(ObjectScales.Human);
        currentSize = ObjectScales.Human;
    }

    private void ShrinkPlayer()
    {
        PlayerController.Instance.transform.localScale = Vector3.one * playerSize_a;
        currentSize = ObjectScales.Alien;
    }
    public Tilemap[] GetHumanScaleTilemaps()
    {
        return environmentMapParent_h.GetComponentsInChildren<Tilemap>();
    }

    public Tilemap[] GetAlienScaleTilemaps()
    {
        return environmentMapParent_a.GetComponentsInChildren<Tilemap>();
    }

    public bool IsAlienPov()
    {
        return currentPov == ObjectScales.Alien;
    }
}
