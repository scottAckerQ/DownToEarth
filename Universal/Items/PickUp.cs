/*****************************************************************************
// File Name: Tool.cs
// Author:
// Creation Date: 2/24/2020
//
// Brief Description: An item class. Stores an item's id which can be fed into the database to find it
Also, if an item can be ranked by stars, stores its star values
*****************************************************************************/

using UnityEngine;

public class PickUp : ScaledObject_Scr, Interactable
{
    public int id ;
    public bool isTool;
    public bool hasStars;
    public float starValue;

    private bool _active = true;

    private void Start()
    {
        myScale = ItemDatabase.instance.GetItem(id).myScale;
    }
    public void SetActive()
    {
        _active = true;
    }

    public void SetInactive()
    {
        _active = false;
    }

    public bool GetActive()
    {
        return _active;
    }

    public int GetID()
    { return id; }
    public void SetID(int num)
    { id = num;}

    public float GetStars()
    { return starValue; }
    public void SetStars(float num)
    { starValue = num; }

    public void Interact(GameObject player)
    {
        
    }


    public bool CanInteract(PlayerController player)
    {
        return true;
    }

}
