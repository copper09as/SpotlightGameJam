using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : SingleCaseMono<CameraSwitch>
{
    public CinemachineVirtualCamera[] virtualCameras;
    private int currentCameraIndex = 0;
    // Start is called before the first frame update
    private void Start()
    {
        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
    }
    public void Switch()//切换摄像机机位（固定两个）
    {
        currentCameraIndex++;
        currentCameraIndex %= virtualCameras.Length;
        for (int i = 0; i < virtualCameras.Length; i++)
        virtualCameras[i].Priority = currentCameraIndex == i ? 10 : 0;

        EntityUIManager.Instance.HideAllMenus();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Switch();
        }
    }
}
