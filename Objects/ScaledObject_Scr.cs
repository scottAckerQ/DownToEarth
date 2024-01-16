using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledObject_Scr : MonoBehaviour
{
    public ScaleManager_Scr.ObjectScales myScale;
    protected Animator anim;
    // Start is called before the first frame update

    private void OnEnable()
    {
        
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
        ScaleManager_Scr.Instance.onScaleChange += UpdateSize;
    }

    private void OnDisable()
    {
        ScaleManager_Scr.Instance.onScaleChange -= UpdateSize;
    }

    protected void UpdateSize(ScaleManager_Scr.ObjectScales scaleToBecome)
    {
        if (anim == null) return;
        if (scaleToBecome == ScaleManager_Scr.ObjectScales.Human)
        {
            anim.SetBool("isAlienScale", false);
        }
        else
        {
            anim.SetBool("isAlienScale", true);
        }
    }

    public bool MatchesSize(ScaledObject_Scr otherObject)
    {
        return MatchesSize(otherObject.myScale);
    }

    public bool MatchesSize(ScaleManager_Scr.ObjectScales otherSize)
    {
        switch (otherSize)
        {
            case ScaleManager_Scr.ObjectScales.Human:
                if (myScale == ScaleManager_Scr.ObjectScales.Human || myScale == ScaleManager_Scr.ObjectScales.Hybrid)
                    return true;
                else
                    return false;
            case ScaleManager_Scr.ObjectScales.Alien:
                if (myScale == ScaleManager_Scr.ObjectScales.Alien || myScale == ScaleManager_Scr.ObjectScales.Hybrid)
                    return true;
                else
                    return false;
            case ScaleManager_Scr.ObjectScales.Hybrid:
                return true;
            default: return false;
        }
    }
}
