using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class SaveLoadManager_Scr : MonoBehaviour
{
    public static SaveLoadManager_Scr Instance;
    private SaveFile saveFile;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SaveDataToFile()
    {
        saveFile.UpdateSaveData();

    }
}
