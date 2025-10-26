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
    [SerializeField] private Button toStartSceneBtn;
    //[SerializeField] private Button openSettingUi;
    [SerializeField] private Button mapBtn;
    [SerializeField] private GameObject settingUIPrefab;
    [Header("Animation Settings")]
    [SerializeField] private Vector3 hiddenPos = new Vector3(0, 1000, 0); // 面板屏幕外位置
    [SerializeField] private Vector3 shownPos = new Vector3(0, 0, 0);     // 面板显示位置
    [SerializeField] private float slideDuration = 0.3f;                  // 动画时间

    [SerializeField] private TMP_InputField txtResolutionX;
    [SerializeField] private TMP_InputField txtResolutionY;
    [SerializeField] private Button confirmResolutionBtn;
    [SerializeField] private bool fullscreen = true;
    [SerializeField] private Toggle fullScreenTog;

    private float currentScale = 1f;
    private Coroutine slideCoroutine;
    private bool isOpen = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EventBus.Subscribe<Global.Events.OpenSettingUi>(OpenUiEve);
        // 初始化面板状态
        settingUi.transform.localPosition = hiddenPos;
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        // 绑定按钮事件
        bgmSlider.onValueChanged.AddListener(BgmSoundChange);
        seSlider.onValueChanged.AddListener(SeSoundChange);
       // openSettingUi.onClick.AddListener(ShowSettingUi);
        closePanelBtn.onClick.AddListener(CloseSettingPanel);
        exitGameBtn.onClick.AddListener(ExitGame);
        toStartSceneBtn.onClick.AddListener(ToStartScene);
        mapBtn.onClick.AddListener(ToMapScene);
        //confirmResolutionBtn.onClick.AddListener(ChangeResolution);
        //fullScreenTog.onValueChanged.AddListener(SetFullscreen);
        //txtResolutionX.onEndEdit.AddListener(_ => ValidateInput(txtResolutionX));
        //txtResolutionY.onEndEdit.AddListener(_ => ValidateInput(txtResolutionY));
    }
    private void OnEnable()
    {
        //txtResolutionX.text = Screen.width.ToString();
        //txtResolutionY.text = Screen.height.ToString();
        //fullScreenTog.isOn = Screen.fullScreen;
    }
    private void ValidateInput(TMP_InputField field)
    {
        string newText = "";
        foreach (char c in field.text)
        {
            if (char.IsDigit(c))
                newText += c;
        }
        if (newText != field.text)
            field.text = newText;
    }
    private void SetFullscreen(bool isFullscreen)
    {
        fullscreen = isFullscreen;
        Screen.SetResolution(Screen.width, Screen.height, fullscreen);
        GameConfig.Instance.SaveUserConfig(Screen.width, Screen.height, fullscreen);
    }
    private void ChangeResolution()
    {
        if (!int.TryParse(txtResolutionX.text, out int width))
        {
            txtResolutionX.text = string.Empty;
            return;
        }

        if (!int.TryParse(txtResolutionY.text, out int height))
        {
            txtResolutionY.text = string.Empty;
            return;
        }
        width = Mathf.Max(800, width);
        height = Mathf.Max(600, height);
        txtResolutionX.text = width.ToString();
        txtResolutionY.text = height.ToString();
        Screen.SetResolution(width, height, fullscreen);
        GameConfig.Instance.SaveUserConfig(width,height,fullscreen);
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
    }

    private void SeSoundChange(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
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
        catch(Exception ex)
        {
            NotificationManager.Instance.ShowNotification(ex.Message, "设置界面打开错误！");
            SceneChangeManager.Instance.LoadScene("StartScene");
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
