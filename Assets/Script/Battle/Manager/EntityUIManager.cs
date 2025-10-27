using System;
using System.Collections;
using System.Collections.Generic;
using Global.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 控制 UI 激活或失活的流程（三级菜单版本）
/// 支持菜单为列表（多个 GameObject）
/// </summary>
public class EntityUIManager : MonoBehaviour
{
    public static EntityUIManager Instance;

    [Header("菜单对象")]
    [SerializeField] private List<GameObject> settingMenu;
    [SerializeField] private List<GameObject> audioMenu;
    [SerializeField] private List<GameObject> cameraMenu;

    [SerializeField] private Button reallSettingUi;
    [SerializeField] private bool autoInit;
    [SerializeField] private Button openMenuBtn;

    public EntityManager entityManager;
    public bool isLoading = true;
    private bool isSettingMenuActive = false;
    private bool isAudioMenuActive = false;
    private bool isCameraMenuActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(Instance.gameObject);
            return;
        }

        if (autoInit)
            Init();
    }

    public void CallNextFrame(Action action)
    {
        StartCoroutine(CallNextFrameCoroutine(action));
    }

    private IEnumerator CallNextFrameCoroutine(Action action)
    {
        yield return null;
        action?.Invoke();
    }

    public void Init()
    {
        isLoading = true;
        SetAllMenuActive(false);
        GameController.Controller.Main.Esc.started += OnEscPressed;
        openMenuBtn.onClick.AddListener(ShowSettingMenu);
        reallSettingUi.onClick.AddListener(OpenReallyUi);
    }

    private void OpenReallyUi()
    {
        EventBus.Publish(new Global.Events.OpenSettingUi());
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/ButtonDown (1).wav");
    }

    private void SetAllMenuActive(bool active)
    {
        SetMenuActive(settingMenu, active);
        SetMenuActive(audioMenu, active);
        SetMenuActive(cameraMenu, active);
    }

    private void OnDestroy()
    {
        Instance = null;
        if (GameController.Controller?.Main != null)
            GameController.Controller.Main.Esc.started -= OnEscPressed;
    }

    /// <summary>
    /// ESC键菜单切换逻辑：
    ///  - 没有菜单 → 打开 SettingMenu
    ///  - AudioMenu 打开 → 返回 SettingMenu
    ///  - CameraMenu 打开 → 返回 SettingMenu
    ///  - SettingMenu 打开 → 关闭所有菜单
    /// </summary>
    private void OnEscPressed(InputAction.CallbackContext ctx)
    {
        if (!isSettingMenuActive && !isAudioMenuActive && !isCameraMenuActive)
        {
            ShowSettingMenu();
        }
        else if (isAudioMenuActive || isCameraMenuActive)
        {
            HideAudioMenu();
            HideCameraMenu();
            ShowSettingMenu();
        }
        else if (isSettingMenuActive)
        {
            HideAllMenus();
        }
    }

    // ---------------- 公开接口 ----------------

    public void SwitchToAudioMenu()
    {
        HideSettingMenu();
        ShowAudioMenu();
    }

    public void SwitchToCameraMenu()
    {
        HideSettingMenu();
        ShowCameraMenu();
    }

    public void BackToSettingMenu()
    {
        HideAudioMenu();
        HideCameraMenu();
        ShowSettingMenu();
    }

    // ---------------- 显示隐藏逻辑 ----------------

    private void ShowSettingMenu()
    {
        if (isLoading) return;
        reallSettingUi.gameObject.SetActive(true);
        openMenuBtn.gameObject.SetActive(false);
        SetMenuActive(settingMenu, true);
        isSettingMenuActive = true;
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/ButtonDown (1).wav");

    }

    private void HideSettingMenu()
    {
        SetMenuActive(settingMenu, false);
        isSettingMenuActive = false;
    }

    private void ShowAudioMenu()
    {
        SetMenuActive(audioMenu, true);
        isAudioMenuActive = true;
    }

    private void HideAudioMenu()
    {
        SetMenuActive(audioMenu, false);
        isAudioMenuActive = false;
    }

    private void ShowCameraMenu()
    {
        SetMenuActive(cameraMenu, true);
        isCameraMenuActive = true;
    }

    private void HideCameraMenu()
    {
        SetMenuActive(cameraMenu, false);
        isCameraMenuActive = false;
    }

    public void HideAllMenus()
    {
        HideSettingMenu();
        HideAudioMenu();
        HideCameraMenu();
        reallSettingUi.gameObject.SetActive(false);
        openMenuBtn.gameObject.SetActive(true);
    }


    private void SetMenuActive(List<GameObject> menuList, bool active)
    {
        if (menuList == null) return;

        foreach (var go in menuList)
        {
            if (go != null)
                go.SetActive(active);
        }
    }
}
