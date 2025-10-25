using System;
using System.Collections;
using Global.Data;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : SingleCaseMono<SceneChangeManager>
{
    [SerializeField] private string testScene;

    private void Start()
    {
        //LoadScene("Battle");
        LoadScene(testScene);
    }
    /// <summary>
    /// 异步加载新场景并卸载旧场景（无进度UI）
    /// </summary>
    private bool isLoading = false;
    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName, LoadSceneMode.Single);
    }
    public void LoadScene(string newScene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (isLoading)
        {
            Debug.LogWarning($"Scene {newScene} is already loading, skip duplicate request.");
            return;
        }
       
        isLoading = true;
       StartCoroutine(LoadSceneCoroutine(newScene, mode));

       
    }

    private IEnumerator LoadSceneCoroutine(string newScene,LoadSceneMode mode)
    {
        AsyncOperation loadOp = null;
        try
        {
            loadOp = SceneManager.LoadSceneAsync(newScene, mode);
            if (loadOp == null)
            {
                isLoading = false;
                NotificationManager.Instance.ShowNotification(newScene, "加载场景出现错误！" + newScene);
                ReloadCurrentScene();
                yield break;
            }

            loadOp.allowSceneActivation = true;
        }
        catch (Exception ex)
        {
            isLoading = false;
            NotificationManager.Instance.ShowNotification(ex.Message, "加载场景出现错误！" + newScene);
           
            yield break;
        }


        // 等待加载完成
        while (!loadOp.isDone)
        {
            yield return null;
        }
        Scene newLoadedScene = SceneManager.GetSceneByName(newScene);
        if (newLoadedScene.IsValid())
            SceneManager.SetActiveScene(newLoadedScene);
        isLoading = false;
        Time.timeScale = 1.0f;
    }
    public void LoadSceneWithDelay(string sceneName,float delay)
    {
        StartCoroutine(LoadSceneWithDelayCoroutine(delay, sceneName));
    }
    public void ReloadSceneWithDelay(float delay)
    {
        LoadSceneWithDelay(SceneManager.GetActiveScene().name, delay);
    }
    private IEnumerator LoadSceneWithDelayCoroutine(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(sceneName, LoadSceneMode.Single);
    }
}
