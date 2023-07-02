using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoints : MonoBehaviour
{

    public Text text;

    public string FormatText = "Points {0}";

    public bool OnCharacter;
    
    // Update is called once per frame
    void Update()
    {
        if (OnCharacter)
        {
            text.text = string.Format(FormatText, GameManager.Instance.BloodOnCharacter);
        }
        else
        {
            text.text = string.Format(FormatText, GameManager.Instance.BloodGivenAway);
        }
        
    }
}
