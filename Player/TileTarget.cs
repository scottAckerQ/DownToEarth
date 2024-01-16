/*****************************************************************************
// File Name: TileTarget.cs
// Author:
// Creation Date: 
//
// Brief Description: An object to keep track of what the player is currently looking at
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TileTarget : MonoBehaviour
{
    private List<GameObject> _myTargets;

    private bool _isMouseActive;
    private int _collisionCount;

    private float _lastMouseMovedTime;
    private Vector3 _currentMousePos;
    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        _lastMouseMovedTime = Time.realtimeSinceStartup;
        _myTargets = new List<GameObject>();
    }
    
    private void Update()
    {
        MouseControls();

        //if (!_isMouseActive) return;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ScaledObject_Scr scaledTileObject = col.gameObject.GetComponent<ScaledObject_Scr>();
        
        if (scaledTileObject != null)
        {
            if (scaledTileObject.MatchesSize(ScaleManager_Scr.Instance.currentSize))
            {
                _myTargets.Add(col.gameObject);
                _collisionCount++;
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col == null) return;
        if (!_myTargets.Contains(col.gameObject)) return;

        _myTargets.Remove(col.gameObject);
        _collisionCount--;
    }

    public bool TouchingObject()
    {
        return _collisionCount > 0; 
    }
    
    private void MouseControls()
    {
        //check if mouse is now moving after being inactive
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            _isMouseActive = true;
            _lastMouseMovedTime = Time.realtimeSinceStartup;
        }

        if (!_isMouseActive) return;
        if (Time.realtimeSinceStartup - _lastMouseMovedTime > .7f)
        {
            _isMouseActive = false;
            return;
        }

        _currentMousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void MouseTarget(Vector3 playerPos, float maxDistance)
    {
        float totalDist = Vector3.Distance(playerPos, _currentMousePos);
        if (totalDist > maxDistance) return;
        
        Vector2 mousePos2D = new Vector2(_currentMousePos.x, _currentMousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider == null) return;
        
        GameObject target = hit.collider.gameObject;
        transform.position = target.transform.position;
    }

    public bool MouseIsActive()
    {
        return _isMouseActive;
    }

    public List<GameObject> GetCurrentTargets()
    {
        return _myTargets;
    }

    public int GetCollisionCount()
    {
        return _collisionCount;
    }
}
