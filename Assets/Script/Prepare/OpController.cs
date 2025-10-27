using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class OpController : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("下一个场景名")]
    public string nextSceneName = "StartScene";

    private bool isSkipped = false;

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
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkipped)
        {
            LoadNextScene();
        }
    }

    private void OnEscPressed(InputAction.CallbackContext ctx)
    {
        SkipOp();
    }

    void SkipOp()
    {
        if (isSkipped) return;

        isSkipped = true;

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneChangeManager.Instance.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        if (GameController.Controller != null)
        {
            GameController.Controller.Main.Esc.started -= OnEscPressed;
        }
    }
}
