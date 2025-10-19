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

    private string websiteUrl = "https://space.bilibili.com/80632239";

    void Awake()
    {
        // 输出显卡名称
        Debug.Log("当前显卡: " + SystemInfo.graphicsDeviceName);

        // 给按钮绑定方法
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        if (websiteButton != null)
            websiteButton.onClick.AddListener(OnWebsiteButtonClicked);
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
