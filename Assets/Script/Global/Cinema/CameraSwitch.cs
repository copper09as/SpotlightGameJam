using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public static CameraSwitch Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
    }
    private void OnDestroy()
    {
        if(Instance==this)
        {
            Instance = null;
        }
    }
    public void Switch(CinemachineVirtualCamera virtualCamera)//切换摄像机机位（固定两个）
    {
        
        for (int i = 0; i < virtualCameras.Length; i++)
        virtualCameras[i].Priority = virtualCamera == virtualCameras[i] ? 10 : 0;
        EntityUIManager.Instance.isLoading = true;
        EntityUIManager.Instance.HideAllMenus();
    }

    public void SetTheCinameSize(float size) 
    {
        var virtualCamera = virtualCameras[0].Priority == 10 ? virtualCameras[0] : virtualCameras[1];
        virtualCamera.m_Lens.OrthographicSize = size;
    }
    
}
