using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 控制 UI 激活或失活的流程（三级菜单版本）
/// </summary>
public class EntityUIManager : MonoBehaviour
{
    public static EntityUIManager Instance;

    [Header("菜单对象")]
    [SerializeField]private GameObject settingMenu;
    [SerializeField]private GameObject audioMenu;
    [SerializeField]private GameObject cameraMenu;

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
            Destroy(Instance);
        }
    }
   
    public void Init(GameObject setting, GameObject audio, GameObject camera)
    {
        settingMenu = setting;
        audioMenu = audio;
        cameraMenu = camera;
        SetAllMenuActive(false);
        GameController.Controller.Main.Esc.started += OnEscPressed;

    }

    private void SetAllMenuActive(bool active)
    {
        if (settingMenu != null) settingMenu.SetActive(active);
        if (audioMenu != null) audioMenu.SetActive(active);
        if (cameraMenu != null) cameraMenu.SetActive(active);
    }
    private void OnDestroy()
    {
        Instance = null;
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
        settingMenu.SetActive(true);
        isSettingMenuActive = true;
    }

    private void HideSettingMenu()
    {
        settingMenu.SetActive(false);
        isSettingMenuActive = false;
    }

    private void ShowAudioMenu()
    {
        audioMenu.SetActive(true);
        isAudioMenuActive = true;
    }

    private void HideAudioMenu()
    {
        audioMenu.SetActive(false);
        isAudioMenuActive = false;
    }

    private void ShowCameraMenu()
    {
        cameraMenu.SetActive(true);
        isCameraMenuActive = true;
    }

    private void HideCameraMenu()
    {
        cameraMenu.SetActive(false);
        isCameraMenuActive = false;
    }

    private void HideAllMenus()
    {
        HideSettingMenu();
        HideAudioMenu();
        HideCameraMenu();
    }
}
