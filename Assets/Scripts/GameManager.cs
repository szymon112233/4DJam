using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public CinemachineVirtualCamera characterVcam;
    
    public int BloodOnCharacter;
    public int BloodGivenAway;

    public float NightTotalDuration;
    public float TimeLeft;
    public bool DEBUGDISABLETIMER = false;

    public bool timeup = false;
    
    public static GameManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public bool TransferBlood(int blood)
    {
        if (BloodOnCharacter < blood)
        {
            return false;
        }

        BloodGivenAway += blood;
        BloodOnCharacter -= blood;

        return true;
    }

    public void SwitchCameraFollow(Transform transform)
    {
        characterVcam.Follow = transform;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        TimeLeft = NightTotalDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (!timeup && !DEBUGDISABLETIMER)
        {
            TimeLeft -= Time.deltaTime;    
        }
        if (TimeLeft < 0)
        {
            TimeUp();
        }
        
    }

    void TimeUp()
    {
        TimeLeft = 0;
        timeup = true;
    }
}
