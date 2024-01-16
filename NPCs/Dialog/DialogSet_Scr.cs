using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A full collection of all dialog a character can say. Stored as a json
/// </summary>
[Serializable]
public class DialogSet_Scr 
{
    public DialogLine introduction;
    public List<DialogLine> allLines;
    public List<DialogLine> dayFirstLines;
    public List<DialogLine> aDialog0, aDialog1, aDialog2, aDialog3, aDialog4, aDialog5;

    public static DialogSet_Scr GetDialogSet(string charName)
    {
        string jsonPath = "NPCData/" + charName + "_CharDialog";
        return Load(jsonPath);
    }

    private static DialogSet_Scr Load(string jsonPath)
    {
        TextAsset json = Resources.Load<TextAsset>(jsonPath);
        string jsonString = json.ToString();
        DialogSet_Scr data = JsonUtility.FromJson<DialogSet_Scr>(jsonString);

        return data;
    }
}
