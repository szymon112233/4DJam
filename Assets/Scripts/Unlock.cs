using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock : MonoBehaviour
{
    public List<Collider2D> CollidersToDisable;
    public GameObject UiCanvas;

    public int Price;
    

    public bool HasEnoughBlood(int blood)
    {
        return blood >= Price;
    }

    public void UnlockColliders()
    {
        foreach (var collider in CollidersToDisable)
        {
            collider.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UiCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UiCanvas.SetActive(false);
    }
}
