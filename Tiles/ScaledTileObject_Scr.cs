using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledTileObject_Scr : ScaledObject_Scr
{
    public GameObject objectSetToSpawnAtop;
    private Vector2 tileCoordinates;
    private bool hasSpawnedObjectItemOnTop;

    public List<Item> spawnableItems = new List<Item>();

}
