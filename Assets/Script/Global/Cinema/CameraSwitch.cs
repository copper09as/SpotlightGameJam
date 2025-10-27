using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public static CameraSwitch Instance;

    [SerializeField] private float switchRecoverDelay = 1f;

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
        if (Instance == this)
        {
            Instance = null;
        }
    }


    public void Switch(CinemachineVirtualCamera virtualCamera)
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].Priority = (virtualCamera == virtualCameras[i]) ? 10 : 0;
        }

        EntityUIManager.Instance.isLoading = true;
        EntityUIManager.Instance.HideAllMenus();


        StartCoroutine(RecoverLoadingFlag());
    }

    private IEnumerator RecoverLoadingFlag()
    {
        yield return new WaitForSecondsRealtime(switchRecoverDelay);
        EntityUIManager.Instance.isLoading = false;
    }

    public void SetTheCinameSize(float size)
    {
        var virtualCamera = virtualCameras[0].Priority == 10 ? virtualCameras[0] : virtualCameras[1];
        virtualCamera.m_Lens.OrthographicSize = size;
    }
}
