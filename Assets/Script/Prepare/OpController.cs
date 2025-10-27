using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.UI;

public class OpController : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("RawImage 渲染视频")]
    public CanvasGroup videoCanvasGroup;

    [Header("下一个场景名")]
    public string nextSceneName = "StartScene";

    [Header("淡出时间")]
    public float fadeDuration = 1.0f;

    private bool isSkippingOrEnding = false;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        if (GameController.Controller != null)
        {
            GameController.Controller.Main.Esc.started += OnEscPressed;
        }

        if (videoCanvasGroup != null)
        {
            videoCanvasGroup.alpha = 1f;
        }
    }

    void Update()
    {
        if (!isSkippingOrEnding && videoPlayer != null && videoPlayer.isPlaying)
        {
            if (videoPlayer.length - videoPlayer.time <= fadeDuration)
            {
                StartCoroutine(FadeOutAndLoadScene());
            }
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkippingOrEnding)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private void OnEscPressed(InputAction.CallbackContext ctx)
    {
        if (!isSkippingOrEnding)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        if (isSkippingOrEnding) yield break;
        isSkippingOrEnding = true;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        if (videoCanvasGroup != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                videoCanvasGroup.alpha = Mathf.Clamp01(1f - timer / fadeDuration);
                yield return null;
            }
            videoCanvasGroup.alpha = 0f;
        }

        SceneChangeManager.Instance.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (GameController.Controller != null)
        {
            GameController.Controller.Main.Esc.started -= OnEscPressed;
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
