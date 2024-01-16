using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CharacterMaster_Scr : MonoBehaviour
{
    public static CharacterMaster_Scr Instance;

    public Transform NPCParent;
    
    //TODO: Create searchable list of all characters
    public Dictionary<string, CharacterData_Scr> allCharacters;
    public Dictionary<string, CharacterData_Scr> activeCharacters;
    public Dictionary<string, GameObject> activeCharacterObjects;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PopulateCharacterList();
        DontDestroyOnLoad(gameObject);
    }

    #region Startup
    public void PopulateCharacterList()
    {
        allCharacters = new Dictionary<string, CharacterData_Scr>();
        LoadCharacterData("Test");
        activeCharacters = new Dictionary<string, CharacterData_Scr>();
        activeCharacterObjects = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// Saves character data to allCharacterData list under specified key
    /// </summary>
    /// <param name="charName"></param>
    public void LoadCharacterData(string charName)
    {
        CharacterData_Scr newChar = CharacterData_Scr.LoadCharacter(charName); 
        allCharacters.Add(charName, newChar);
    }
    #endregion

    public CharacterData_Scr GetCharacter(string charName)
    {
        allCharacters.TryGetValue(charName, out CharacterData_Scr character); 
        
        if (character == null)
        {
            Debug.LogError("I could not find the character; sorry!");
            return null;
        }
        return character;
    }

    /// <summary>
    /// Add character of given name to scene as well as active character lists
    /// </summary>
    /// <param name="characterName">The name of the character to be spawned</param>
    public void SpawnCharacter(string characterName, Vector3 position)
    {
        
        
        GameObject charPrefab = GameObject.Instantiate
            (Resources.Load<GameObject>($"NPCPrefabs/{characterName}_characterObject"), NPCParent);
        charPrefab.transform.position = position;

        activeCharacters.Add(characterName, allCharacters[characterName]);
        activeCharacterObjects.Add(characterName, charPrefab); 
    }

    /// <summary>
    /// Removes character of given name from scene as well as active character lists
    /// </summary>
    /// <param name="characterName">The name of the character to be removed</param>
    public void DeSpawnCharacter(string characterName)
    {
        if (activeCharacters.Remove(characterName)) 
            Debug.Log($"Successfully removed {characterName} from active character list");
        else
            Debug.Log($"ERROR: Could not {characterName} from active character list");

        if (activeCharacterObjects.TryGetValue(characterName, out GameObject charObject))
        {
            charObject.SetActive(false);
            Destroy(charObject);
            Debug.Log($"Successfully removed {characterName} from active character object list");
        }
            
        else
            Debug.Log($"ERROR: Could not {characterName} from active character object list");
    }
    
    public void AddAffection(int delta, string charName)
    {
        CharacterData_Scr character = GetCharacter(charName);
        character.UpdateAffection(delta);
    }

    public void SaveCharacterData()
    {

    }

    public Dictionary<string, CharacterData_Scr> GetActiveCharacters()
    {
        return activeCharacters;
    }

    public bool IsCharacterActive(string charName)
    {
        return activeCharacters.ContainsKey(charName);
    }

    public GameObject GetCharacterInScene(string charName)
    {
        return activeCharacterObjects[charName] ;
    }

    public Dictionary<string, string> GetCharacterSchedule(string charName)
    {
        return GetCharacter(charName).GetSchedule();
    }

    public void GenerateExampleJSON()
    {
        CharacterData_Scr placeholder = new CharacterData_Scr(); 
        string jsonPath = $"{Application.persistentDataPath}/Character.json";

        string json = JsonConvert.SerializeObject(placeholder, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
    }
}
