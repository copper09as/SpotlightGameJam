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
    [SerializeField] private Button OpenMenusButton;
    [SerializeField] private Button openGiftPanelBtn;
    [SerializeField] private GameObject giftPanel;

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
    }

    private void OpenGiftPanel()
    {
        giftPanel.SetActive(true);
    }

    private void OpenMenu()
    {
        EventBus.Publish(new Global.Events.OpenSettingUi());
    }
    private void Start()
    {
        AudioManager.Instance.PlayBGM(StringResource.StartBgmPath);
    }
    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
        SceneChangeManager.Instance.LoadScene("Map");
    }

    private void OnWebsiteButtonClicked()
    {
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
        if (!string.IsNullOrEmpty(websiteUrl))
        {
            Application.OpenURL(websiteUrl);
        }
    }
}
