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
    /// �첽�����³�����ж�ؾɳ������޽���UI��
    /// </summary>
    public void LoadScene(string newScene)
    {
        StartCoroutine(LoadSceneCoroutine(newScene,LoadSceneMode.Single));
    }

    private IEnumerator LoadSceneCoroutine(string newScene,LoadSceneMode mode)
    {
        // �첽�����³���
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newScene, mode);
        loadOp.allowSceneActivation = true; // ֱ�Ӽ���

        // �ȴ��������
        while (!loadOp.isDone)
        {
            yield return null;
        }
        Scene newLoadedScene = SceneManager.GetSceneByName(newScene);
        if (newLoadedScene.IsValid())
            SceneManager.SetActiveScene(newLoadedScene);
    }
}
