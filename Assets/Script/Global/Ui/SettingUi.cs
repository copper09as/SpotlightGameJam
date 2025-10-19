using System.Collections;
using Global.Data;
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
    [SerializeField] private CanvasGroup panelCanvasGroup; // 用于淡入淡出
    [SerializeField] private Button toStartSceneBtn;
    [SerializeField] private Button openSettingUi;
    [SerializeField] private Button mapBtn;

    [Header("Animation Settings")]
    [SerializeField] private Vector3 hiddenPos = new Vector3(0, 1000, 0); // 面板屏幕外位置
    [SerializeField] private Vector3 shownPos = new Vector3(0, 0, 0);     // 面板显示位置
    [SerializeField] private float slideDuration = 0.3f;                  // 动画时间

    private float currentScale = 1f;
    private Coroutine slideCoroutine;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

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
        openSettingUi.onClick.AddListener(ShowSettingUi);
        closePanelBtn.onClick.AddListener(CloseSettingPanel);
        exitGameBtn.onClick.AddListener(ExitGame);
        toStartSceneBtn.onClick.AddListener(ToStartScene);
        mapBtn.onClick.AddListener(ToMapScene);
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
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
        currentScale = Time.timeScale;
        settingUi.SetActive(true);

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideAndFade(settingUi.transform, hiddenPos, shownPos, 0f, 1f, slideDuration));
        Time.timeScale = 0f;

        EventBus.Publish(new Global.Events.OpenSettingUi());
    }

    private void CloseSettingPanel()
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideAndFade(settingUi.transform, shownPos, hiddenPos, 1f, 0f, slideDuration, true));
        Time.timeScale = currentScale;

        EventBus.Publish(new Global.Events.CloseSettingUi());
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
