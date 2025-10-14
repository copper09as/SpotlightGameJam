using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : SingleCaseMono<SceneChangeManager>
{

    private void Start()
    {
        //LoadScene("Battle");
        LoadScene("Enemy");
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
        // 异步加载新场景
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newScene, mode);
        loadOp.allowSceneActivation = true; // 直接激活

        // 等待加载完成
        while (!loadOp.isDone)
        {
            yield return null;
        }
        Scene newLoadedScene = SceneManager.GetSceneByName(newScene);
        if (newLoadedScene.IsValid())
            SceneManager.SetActiveScene(newLoadedScene);
        isLoading = false;
    }
}
