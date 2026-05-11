using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;
    [SerializeField] private Light2D gloalLight;
    [SerializeField] private GameObject damageOnHeadObj;
    [SerializeField] private GameObject walkSmogObj;

    public void Awake()
    {

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public float GetGlobalLight()
    {
        return gloalLight.intensity;
    }
    private void OnDestroy()
    {
        if(Instance == this)
            Instance = null;
    }
    public void SetGlobalLight(float value)
    {
        gloalLight.intensity = value;
    }
    
}
