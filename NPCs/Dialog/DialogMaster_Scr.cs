using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class DialogMaster_Scr : MonoBehaviour
{
    public static DialogMaster_Scr Instance;
    private CharacterData_Scr _currentCharacter;
    private DialogSet_Scr _currentDialogSet;
    private TextAsset _currentDialogJson;
    private List<DialogLine> _allLines;
    
    private GameObject _dialogHUD;
    private TMP_Text _dialogBoxText;
    
    private const float PrintSpeed = 0.05f;
    
    public bool inDialog, printingDialog;
    private string[] _currentDialogLines;
    private int _dialogIndex;

    private int _todayDialogIndex;

    private PlayerController _player;
    private const float chatCooldown = 1f;
    private float _timeofLastchat;

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

    private void Start()
    {
        _dialogHUD = UIComponentReference.instance.GetDialogBox();
        _dialogBoxText = _dialogHUD.GetComponentInChildren<TMP_Text>();

        _todayDialogIndex = 0;
        printingDialog = false;
        inDialog = false;
    }

    private void Update()
    {
        if (inDialog)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (printingDialog)
                {
                    printingDialog = false;
                    StopCoroutine(PrintDialog());
                    return;
                }
                else
                {
                    if (inDialog)
                    {
                        ContinueDialog();
                    }
                    else
                    {
                        _dialogIndex = -1;
                        ContinueDialog();
                    }
                }                
            }
        }
    }

    public void Activate(string characterName)
    {
        if (Time.realtimeSinceStartup - _timeofLastchat < chatCooldown) return;
        _currentCharacter = CharacterMaster_Scr.Instance.GetCharacter(characterName);
        _currentDialogSet = DialogSet_Scr.GetDialogSet(characterName);
        
        StartDialogProcess();
    }

    private void StartDialogProcess()
    {
        inDialog = true;
        PickDialog();
    }

    /// <summary>
    /// Sorts through dialog available in allLines and randomly selects a line of dialog that fits
    /// </summary>
    private void PickDialog()
    {
        _allLines = GetAffectionMatchingLines(_currentCharacter.GetAffectionLevel());

        if (_allLines.Count == 0) return;

        List<DialogLine> matchingLines = new List<DialogLine>();
        
        foreach (DialogLine line in _allLines)
        {
            if (_todayDialogIndex != 0 && line.firstLine) continue;
            if (_todayDialogIndex == 0 && !line.firstLine) continue;
            if (!line.requiredSeasons.Contains(DayManager.Instance._currentSeason.ToString())) continue;
            if (_currentCharacter == null) continue;

            matchingLines.Add(line);
        }

        int randomNum = Random.Range(0, matchingLines.Count - 1);
        _currentDialogLines = matchingLines[randomNum].dialogString;
        _todayDialogIndex++;
        _dialogIndex = 0;
        StartCoroutine(PrintDialog());
    }

    /// <summary>
    /// Increments Dialog Index, then either closes the chat window or prints the next line
    /// </summary>
    private void ContinueDialog()
    {
        _dialogIndex++;

        if (_dialogIndex < _currentDialogLines.Length - 1)
        {
            StartCoroutine(PrintDialog());
        }
        else
        {
            inDialog = false;
            CloseDialogBox();
        }
    }

    //UI and Such
    private void OpenDialogBox()
    {
        _dialogHUD.SetActive(true);
        inDialog = true;
    }

    private void CloseDialogBox()
    {
        _dialogHUD.SetActive(false);
        inDialog = false;
        _timeofLastchat = Time.realtimeSinceStartup;
    }
    
    /// <summary>
    /// Iterates through all characters in current dialog line, printing them out one by one.
    /// </summary>
    private IEnumerator PrintDialog()
    {
        string textToPrint = _currentDialogLines[_dialogIndex];
        printingDialog = true;
        
        OpenDialogBox();
        _dialogBoxText.text = "";
        
        int charIndex = 0;
        int totalCharacterCount = textToPrint.Length;

        while (charIndex < totalCharacterCount && printingDialog)
        {
            _dialogBoxText.text += textToPrint[charIndex];
            charIndex++;
            
            yield return new WaitForSeconds(PrintSpeed);
        }

        _dialogBoxText.text = textToPrint;
        printingDialog = false;
    }

    private List<DialogLine> GetAffectionMatchingLines(int affectionLevel)
    {
        Debug.Log(affectionLevel);
        int aLevel = affectionLevel / 1000; //make sure this just cuts off the decimals
        switch(aLevel)
        {
            case 0:
                return _currentDialogSet.aDialog0;
            case 1:
                return _currentDialogSet.aDialog1;
            case 2:
                return _currentDialogSet.aDialog2;
            case 3:
                return _currentDialogSet.aDialog3;
            case 4:
                return _currentDialogSet.aDialog4;
            case 5:
                return _currentDialogSet.aDialog5;
            default: return null;
        }
    }
}
