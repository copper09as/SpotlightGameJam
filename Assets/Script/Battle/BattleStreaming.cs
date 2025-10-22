using System.Collections;
using Game.Battle.Entity;
using Global.Data;
using Global.Data.BattleConfig;
using TMPro;
using UnityEngine;

public class BattleStreaming : MonoBehaviour
{
    [SerializeField] private CanvasGroup panel;  // 加载界面
    private TextMeshProUGUI tipText;
    private TextMeshProUGUI sceneNameText;
    private bool isLoading = false;

    [SerializeField] private float fadeDuration = 0.5f; // ✅ 淡出时间

    private void Awake()
    {
        if (panel != null && panel.transform.childCount > 0)
        {
            var child1 = panel.transform.GetChild(1);
            tipText = child1.GetComponent<TextMeshProUGUI>();

            var child2 = panel.transform.GetChild(0);
            sceneNameText = child2.GetComponent<TextMeshProUGUI>();
        }

        SetPanelVisible(false, instant: true);
    }

    private void Start()
    {
        StartCoroutine(LoadBattle(BattleConfig.Instance.levelId));
    }

    /// <summary>
    /// 战斗加载逻辑
    /// </summary>
    private IEnumerator LoadBattle(int levelId)
    {
        if (isLoading) yield break;
        isLoading = true;

        SetPanelVisible(true);

        var levelData = GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == levelId);
        if (levelData != null)
        {
            if (tipText != null) tipText.text = levelData.noteString;
            if (sceneNameText != null) sceneNameText.text = levelData.SceneName;
        }

        var entityManager = new EntityManager();
        foreach (var entity in FindObjectsOfType<Entity>(true))
        {
            entityManager.Register(entity);
        }

        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(FadeOutPanel());
        isLoading = false;
    }

    /// <summary>
    /// 控制加载界面显示与隐藏
    /// </summary>
    private void SetPanelVisible(bool visible, bool instant = false)
    {
        if (panel == null) return;

        if (instant)
        {
            panel.alpha = visible ? 1 : 0;
        }

        panel.interactable = visible;
        panel.blocksRaycasts = visible;
    }

    /// <summary>
    /// 淡出加载界面
    /// </summary>
    private IEnumerator FadeOutPanel()
    {
        if (panel == null) yield break;

        float startAlpha = panel.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }

        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
}
