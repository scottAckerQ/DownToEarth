using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [FormerlySerializedAs("Introduction")] public string[] introduction;
    private string[] _currentSpeech;
    private int _counter;
    public bool doneTalking;
    public Text dText;

    // Start is called before the first frame update
    private void Start()
    {
        introduction = new string[1];
        introduction[1] = "Hey there! I'm Jack.";

        _currentSpeech = introduction;
        
    }

    private void StartTalking()
    {
        dText.text = introduction[_counter];
    }

    private void Scroll()
    {
        _counter += 1;
        if(_counter == _currentSpeech.Length)
        {
            doneTalking = true;
        }
    }
    // Update is called once per frame
    private void Update()
    {
        
    }
}
