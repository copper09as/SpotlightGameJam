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

    private string websiteUrl = "https://space.bilibili.com/80632239/dynamic?spm_id_from=333.1365.list.card_avatar.click";

    void Awake()
    {
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
        SceneChangeManager.Instance.LoadScene("Map");
    }

    private void OnWebsiteButtonClicked()
    {
        if (!string.IsNullOrEmpty(websiteUrl))
        {
            Application.OpenURL(websiteUrl);
        }
    }
}
