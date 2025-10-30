using System;
using System.Collections;
using System.Collections.Generic;
using Global.Data;
using UnityEngine;
using UnityEngine.UI;

public class StartUiManager : MonoBehaviour
{
    [Header("Buttons")]

    [SerializeField] private Button startButton;
    [SerializeField] private Button websiteButton;
    
    [SerializeField] private Button openGiftPanelBtn;
    [SerializeField] private GameObject giftPanel;
    [Header("设置按钮")]
    [SerializeField] private Button OpenMenusButton;
    [Header("退出游戏按钮")]
    [SerializeField] private Button ExitBtn;
    [Header("制作人员按钮")]
    [SerializeField] private Button StaffBtn;
    [SerializeField] private GameObject staffPanel;
    private string websiteUrl = 
        "https://message.bilibili.com/?spm_id_from=333.1387.0.0#/whisper/mid80632239";

    void Awake()
    {
        // 输出显卡名称
        Debug.Log("当前显卡: " + SystemInfo.graphicsDeviceName);

        // 给按钮绑定方法
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        if (websiteButton != null)
            websiteButton.onClick.AddListener(OnWebsiteButtonClicked);
        OpenMenusButton.onClick.AddListener(OpenMenu);
        openGiftPanelBtn.onClick.AddListener(OpenGiftPanel);
        ExitBtn.onClick.AddListener(Exit);
        StaffBtn.onClick.AddListener(ShowStaff);
        //AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/GameStart (1).wav");
    }

    private void ShowStaff()
    {
         staffPanel.SetActive(true);
    }

    private void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OpenGiftPanel()
    {
        giftPanel.SetActive(true);
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/ButtonDown (1).wav");
    }

    private void OpenMenu()
    {
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/ButtonDown (1).wav");
        EventBus.Publish(new Global.Events.OpenSettingUi());
    }
    private void Start()
    {
        AudioManager.Instance.PlayBGM(StringResource.BattleBgmPath);
    }
    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/ButtonDown (1).wav");
        SceneChangeManager.Instance.LoadScene("Map");
    }

    private void OnWebsiteButtonClicked()
    {
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/ButtonDown (1).wav");
        if (!string.IsNullOrEmpty(websiteUrl))
        {
            Application.OpenURL(websiteUrl);
        }
    }
}
