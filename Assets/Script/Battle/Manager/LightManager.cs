using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public static LightManager Instance;
    [SerializeField] private Light2D gloalLight;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
