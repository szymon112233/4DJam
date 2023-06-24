using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITime : MonoBehaviour
{
    public Text text;

    public string FormatText = "TimeLeft: {0}:{1}";
    

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format(FormatText, (int)GameManager.Instance.TimeLeft/60, (int)(GameManager.Instance.TimeLeft%60));
    }
}
