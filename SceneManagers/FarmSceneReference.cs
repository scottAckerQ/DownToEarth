using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class FarmSceneReference : MonoBehaviour
{
    public static FarmSceneReference Instance { get; private set; }
    [SerializeField]private GameObject _dirtParent;
    [SerializeField]private ShippingBox_Scr _shippingBin;
    private Dictionary<int, Dirt> _allDirtObjects = new Dictionary<int, Dirt>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        
        
    }
    public Dictionary<int, Dirt> GetAllDirt()
    {
        
        if(_allDirtObjects == null || _allDirtObjects.Count == 0)
        {
            int index = 0;
            foreach (Dirt d in _dirtParent.GetComponentsInChildren<Dirt>())
            {
                _allDirtObjects.Add(index, d);
                index++;
            }
        }
        return _allDirtObjects;
    }
}
