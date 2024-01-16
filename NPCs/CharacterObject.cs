using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class CharacterObject : ScaledObject_Scr, Interactable
{
    public string characterName = "Test";
    private bool moving;
    private Vector3 currentDestination;
    private const float moveSpeed = 1.7f;

    private void Start()
    {
        Dictionary<string, string> sched = CharacterMaster_Scr.Instance.GetCharacterSchedule(characterName);
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!moving) return;
        Movement();
    }
    public string GetCharacterName()
    {
        return characterName;
    }

    public void Interact(GameObject player)
    {
        DialogMaster_Scr.Instance.Activate(characterName);
        CharacterMaster_Scr.Instance.AddAffection(50, characterName);
    }

    public void UpdateDestination(Vector3 newDestination)
    {
        moving = true;
        currentDestination = newDestination;
        anim.SetBool("WalkingForward", true);
    }

    private void Movement()
    {
        if(transform.position == currentDestination)
        {
            moving = false;
            anim.SetBool("WalkingForward", false);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, Time.deltaTime * moveSpeed);
        }
    }
}
