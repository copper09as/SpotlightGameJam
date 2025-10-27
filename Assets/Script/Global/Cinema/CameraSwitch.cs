using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : SingleCaseMono<CameraSwitch>
{
    public CinemachineVirtualCamera[] virtualCameras;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
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
