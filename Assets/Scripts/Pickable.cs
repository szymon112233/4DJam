using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Pickable : MonoBehaviour
{
    public SpriteRenderer renderer;
    public int Points;
    public List<Sprite> PossibleSplats;

    public void Awake()
    {
        renderer.sprite = PossibleSplats[Random.Range(0, PossibleSplats.Count)];
    }
}
