using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDonor : MonoBehaviour
{
    public GameObject UiCanvas;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        UiCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UiCanvas.SetActive(false);
    }
}
