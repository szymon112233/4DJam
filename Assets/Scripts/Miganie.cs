using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Miganie : MonoBehaviour
{
    public List<Light2D> LightsToTwinkle;
    public float TwinkleSpeed = 0.1f;

    private Dictionary<Light2D, float> LightsPositions;


    public void EnableLights(bool isEnabled)
    {
        foreach (var light in LightsToTwinkle)
        {
            light.enabled = isEnabled;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LightsPositions = new Dictionary<Light2D, float>();
        for (int i = 0; i < LightsToTwinkle.Count; i++)
        {
            var Light = LightsToTwinkle[i];
            var StartPos = i * ((2 * Mathf.PI) / LightsToTwinkle.Count);
            LightsPositions.Add(Light, StartPos);
        }

        GetComponent<TopDownCarController>().OnPosses += () => { EnableLights(true); };
        GetComponent<TopDownCarController>().OnUnPosses += () => { EnableLights(false); };
        EnableLights(false);
    }

    // Update is called once per frame
    void Update()
    {
        var keys = new List<Light2D>(LightsPositions.Keys);
        foreach (Light2D key in keys)
        {
            var change = TwinkleSpeed * Time.deltaTime;
            change = LightsPositions[key] + change;
            LightsPositions[key] = change; 
            key.intensity = Mathf.Sin(LightsPositions[key]);
        }
    }
}