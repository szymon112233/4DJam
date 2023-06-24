using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoints : MonoBehaviour
{

    public Text text;

    public string FormatText = "Points {0}";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format(FormatText, GameManager.Instance.BloodOnCharacter);
    }
}
