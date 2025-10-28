using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineBrain brain;
    public CinemachineVirtualCamera[] virtualCameras;
    public static CameraSwitch Instance;
    [SerializeField] private float switchRecoverDelay = 2f;

    private Coroutine recoverCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        brain = GetComponentInChildren<CinemachineBrain>();
        virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();

        
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    public void Switch(CinemachineVirtualCamera virtualCamera)
    {

        if (virtualCameras == null || virtualCameras.Length == 0)
            return;

        for (int i = 0; i < virtualCameras.Length; i++)
            virtualCameras[i].Priority = (virtualCamera == virtualCameras[i]) ? 10 : 0;

        EntityUIManager.Instance.isLoading = true;
        EntityUIManager.Instance.HideAllMenus();

        if (recoverCoroutine != null)
        {
            StopCoroutine(recoverCoroutine);
            recoverCoroutine = null;
        }

        recoverCoroutine = StartCoroutine(RecoverLoadingFlag());
    }


    private IEnumerator RecoverLoadingFlag()
    {
        yield return new WaitForSecondsRealtime(switchRecoverDelay);

        EntityUIManager.Instance.isLoading = false;
        recoverCoroutine = null;
    }


    public void SetTheCinameSize(float size)
    {
        if (virtualCameras == null || virtualCameras.Length == 0) return;

        var activeCam = virtualCameras[0].Priority == 10 ? virtualCameras[0] : virtualCameras[1];
        activeCam.m_Lens.OrthographicSize = size;
    }

    public float GetSwitchTime()
       => brain.m_DefaultBlend.m_Time;
}
