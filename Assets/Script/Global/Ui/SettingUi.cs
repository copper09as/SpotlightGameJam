
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUi : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Button closePanelBtn;
    [SerializeField] private Button exitGameBtn;
    [SerializeField] private GameObject settingUi;
    [SerializeField] private Button toStartSceneBtn;
    [SerializeField] private Button openSettingUi;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        bgmSlider.onValueChanged.AddListener(BgmSoundChange);
        seSlider.onValueChanged.AddListener(SeSoundChange);
        openSettingUi.onClick.AddListener(ShowSettingUi);
        closePanelBtn.onClick.AddListener(CloseSettingPanel);
        exitGameBtn.onClick.AddListener(ExitGame);
        toStartSceneBtn.onClick.AddListener(ToStartScene);
        settingUi.SetActive(false); // ³õÊ¼Òþ²Ø
    }

    private void ShowSettingUi()
    {
        settingUi.SetActive(true);
    }
    private void ToStartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "StartScene")
            return;
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

    private void CloseSettingPanel()
    {
        settingUi.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
