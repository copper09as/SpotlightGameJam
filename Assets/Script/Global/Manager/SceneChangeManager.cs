using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScene("Battle");
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 异步加载新场景并卸载旧场景（无进度UI）
    /// </summary>
    public void LoadScene(string newScene)
    {
        StartCoroutine(LoadSceneCoroutine(newScene,LoadSceneMode.Single));
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
    }
}
