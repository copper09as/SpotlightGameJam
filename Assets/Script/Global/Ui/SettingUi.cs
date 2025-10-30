using System;
using System.Collections;
using Global.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUi : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Button closePanelBtn;
    [SerializeField] private Button exitGameBtn;
    [SerializeField] private GameObject settingUi;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private Button mapBtn;
    [Header("Animation Settings")]
    [SerializeField] private Vector3 hiddenPos = new Vector3(0, 1000, 0); // 面板屏幕外位置
    [SerializeField] private Vector3 shownPos = new Vector3(0, 0, 0);     // 面板显示位置
    [SerializeField] private float slideDuration = 0.3f;                  // 动画时间

    [Header("分辨率设置")]
    [SerializeField] private TMP_Dropdown dropDown;
    [SerializeField] private Toggle fullScreenTog;
    [SerializeField] private bool fullscreen = true;
    [SerializeField] private Toggle vSyncTog;
    [SerializeField] private Toggle frameLock;
    private readonly string[] resolutionOptions = { "1366X768", "1600X900", "1920X1080", "2560X1440" };

    private float currentScale = 1f;
    private Coroutine slideCoroutine;
    private bool isOpen = false;

    [Header("Slider UI")]
    [SerializeField] private Image bgmFillImage;
    [SerializeField] private Image seFillImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = new Color(1f, 0.8f, 0.2f);

    private Coroutine bgmFlashRoutine;
    private Coroutine seFlashRoutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EventBus.Subscribe<Global.Events.OpenSettingUi>(OpenUiEve);

        // 初始化UI状态
        settingUi.transform.localPosition = hiddenPos;
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        vSyncTog.isOn = QualitySettings.vSyncCount > 0;
        // 初始化状态
     
        
        // 按钮绑定
        bgmSlider.onValueChanged.AddListener(BgmSoundChange);
        seSlider.onValueChanged.AddListener(SeSoundChange);
        closePanelBtn.onClick.AddListener(CloseSettingPanel);
        exitGameBtn.onClick.AddListener(ExitGame);
        mapBtn.onClick.AddListener(ToMapScene);
        fullScreenTog.onValueChanged.AddListener(SetFullscreen);
        dropDown.onValueChanged.AddListener(ChangeResolution);
        frameLock.onValueChanged.AddListener(ChangeLock);
        vSyncTog.onValueChanged.AddListener(SetVSync);
        fullScreenTog.isOn = Screen.fullScreen;
        frameLock.isOn = PlayerPrefs.GetInt("FrameLock", 0) == 1;
        InitResolutionDropdown();
    }

    private void ChangeLock(bool arg0)
    {
        PlayerPrefs.SetInt("FrameLock",Convert.ToInt32(arg0));

        PlayerPrefs.Save();
        if(arg0)
        {
            Application.targetFrameRate = GameConfig.Instance.UserCD.TargetFrameRate;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }

    private void InitResolutionDropdown()
    {
        dropDown.ClearOptions();

        foreach (var res in resolutionOptions)
            dropDown.options.Add(new TMP_Dropdown.OptionData(res));

        // 匹配当前屏幕分辨率
        string currentRes = $"{Screen.width}X{Screen.height}";
        int index = Array.FindIndex(resolutionOptions, r => r == currentRes);
        if (index >= 0)
        {
            dropDown.value = index;
            dropDown.captionText.text = $"{resolutionOptions[index]}（已应用）";
        }
        else
        {
            dropDown.value = 0;
        }
        vSyncTog.isOn = QualitySettings.vSyncCount==1;
    }
    private void SetVSync(bool isOn)
    {
        
        GameConfig.Instance.SaveUserConfig(Screen.width, Screen.height, fullscreen,isOn);
    }
    private void SetFullscreen(bool isFullscreen)
    {
        fullscreen = isFullscreen;
        Screen.fullScreen = fullscreen;
        Debug.Log($"全屏状态: {fullscreen}");
        GameConfig.Instance.SaveUserConfig(Screen.width, Screen.height, fullscreen,vSyncTog.isOn);
    }

    private void ChangeResolution(int index)
    {
        string optionText = dropDown.options[index].text;
        if (string.IsNullOrEmpty(optionText) || !optionText.Contains("X"))
        {
            Debug.LogWarning($"无法解析分辨率: {optionText}");
            return;
        }

        string[] parts = optionText.ToLower().Split('x');
        if (parts.Length != 2)
        {
            Debug.LogWarning($"格式错误: {optionText}");
            return;
        }

        if (int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
        {
            Screen.SetResolution(width, height, fullscreen);
            Debug.Log($"分辨率已设置为: {width}x{height}, 全屏: {fullscreen}");

            dropDown.captionText.text = $"{width}X{height}";
            GameConfig.Instance.SaveUserConfig(width, height, fullscreen,vSyncTog.isOn);
        }
        else
        {
            Debug.LogWarning($"无法转换为数字: {optionText}");
        }
    }

    private void OpenUiEve(Global.Events.OpenSettingUi eve)
    {
        ShowSettingUi();
    }

    private void ToMapScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Map") return;

        SceneChangeManager.Instance.LoadScene("Map");
        CloseSettingPanel();
    }

    private void ToStartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "StartScene") return;

        SceneChangeManager.Instance.LoadScene("StartScene");
        CloseSettingPanel();
    }

    private void BgmSoundChange(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetBGMVolume(value);

        if (bgmFlashRoutine != null) StopCoroutine(bgmFlashRoutine);
        bgmFlashRoutine = StartCoroutine(FlashSlider(bgmFillImage));
    }

    private void SeSoundChange(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);

        if (seFlashRoutine != null) StopCoroutine(seFlashRoutine);
        seFlashRoutine = StartCoroutine(FlashSlider(seFillImage));
    }

    private IEnumerator FlashSlider(Image fill)
    {
        if (fill == null) yield break;
        fill.color = activeColor;
        float t = 0f;
        const float duration = 0.4f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            fill.color = Color.Lerp(activeColor, normalColor, t);
            yield return null;
        }
        fill.color = normalColor;
    }

    private void ShowSettingUi()
    {
        if (isOpen)
            return;

        isOpen = true;
        currentScale = Time.timeScale;
        try
        {
            settingUi.SetActive(true);
        }
        catch (Exception ex)
        {
            NotificationManager.Instance.ShowNotification(ex.Message, "设置界面打开错误！");
            SceneChangeManager.Instance.LoadScene("StartScene");
            throw ex;
        }

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideAndFade(settingUi.transform, hiddenPos, shownPos, 0f, 1f, slideDuration));
        Time.timeScale = 0f;
        EventBus.Publish(new Global.Events.OnOpenSettingUi());
    }

    private void CloseSettingPanel()
    {
        isOpen = false;
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideAndFade(settingUi.transform, shownPos, hiddenPos, 1f, 0f, slideDuration, true));
        Time.timeScale = currentScale;
        EventBus.Publish(new Global.Events.OnCloseSettingUi());
    }

    private IEnumerator SlideAndFade(Transform panel, Vector3 startPos, Vector3 endPos, float startAlpha, float endAlpha, float duration, bool disableAfter = false)
    {
        float t = 0f;

        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = startAlpha;
            panelCanvasGroup.interactable = startAlpha > 0f;
            panelCanvasGroup.blocksRaycasts = startAlpha > 0f;
        }

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            panel.localPosition = Vector3.Lerp(startPos, endPos, smoothT);
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, smoothT);

            yield return null;
        }

        panel.localPosition = endPos;
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = endAlpha;
            panelCanvasGroup.interactable = endAlpha > 0f;
            panelCanvasGroup.blocksRaycasts = endAlpha > 0f;
        }

        if (disableAfter)
            panel.gameObject.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
