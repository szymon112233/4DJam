using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public CinemachineVirtualCamera characterVcam;
    public UIGameSummaryPanel gameOverPanel;
    
    public int BloodOnCharacter;
    public int BloodGivenAway;
    public int BloodSpent;
    public int HumansKilled;

    public float NightTotalDuration;
    public Vector2 ZoomRange;
    public AnimationCurve CameraZoomCurve;
    
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
    
    public bool PayWithBlood(int blood)
    {
        if (BloodOnCharacter < blood)
        {
            return false;
        }
        
        BloodOnCharacter -= blood;
        BloodSpent += blood;
        return true;
    }

    public void SwitchCameraFollow(Transform transform)
    {
        characterVcam.Follow = transform;
    }

    public void UpdateCameraZoom(float NormalizedZoom)
    {
        characterVcam.m_Lens.OrthographicSize = Mathf.Lerp(ZoomRange.x, ZoomRange.y, CameraZoomCurve.Evaluate(NormalizedZoom));
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
        
        gameOverPanel.Show();
    }

    public void RestartGame()
    {
        BloodOnCharacter = 0;
        BloodGivenAway = 0;
        BloodSpent = 0;
        HumansKilled = 0;
        SceneManager.LoadScene(0);
    }
}
