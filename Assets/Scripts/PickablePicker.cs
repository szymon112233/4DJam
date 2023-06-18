using System;
using UnityEngine;

public class PickablePicker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Pickable pickable = other.GetComponent<Pickable>();

        if (pickable == null)
        {
            return;
        }

        GameManager.Instance.Points += pickable.Points;
        
        Destroy(other.gameObject);
    }
}
