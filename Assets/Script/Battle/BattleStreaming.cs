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

    private void Awake()
    {
        // 获取提示文字组件（panel 的第一个子节点）
        if (panel != null && panel.transform.childCount > 0)
        {
            var child = panel.transform.GetChild(1);
            tipText = child.GetComponent<TextMeshProUGUI>();
            var child2 = panel.transform.GetChild(0);
            sceneNameText = child2.GetComponent<TextMeshProUGUI>();
        }

        // 默认隐藏加载界面
        SetPanelVisible(false);
    }

    private void Start()
    {
        StartCoroutine(LoadBattle(BattleConfig.Instance.levelId));
    }

    /// <summary>
    /// 战斗加载逻辑（只控制加载面板与初始化）
    /// </summary>
    private IEnumerator LoadBattle(int levelId)
    {
        if (isLoading) yield break;
        isLoading = true;

        SetPanelVisible(true);

        var levelData = GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == levelId);
        if (tipText != null && levelData != null)
        {
            tipText.text = levelData.noteString;
        }
        if (sceneNameText != null && levelData != null)
        {
            sceneNameText.text = levelData.SceneName;
        }
        var entityManager = new EntityManager();
        foreach (var entity in FindObjectsOfType<Entity>(true))
        {
            entityManager.Register(entity);
            //entity.Init(entityManager);
        }
        yield return new WaitForSeconds(1f);

        SetPanelVisible(false);
        isLoading = false;
    }

    /// <summary>
    /// 控制加载界面显示与隐藏
    /// </summary>
    private void SetPanelVisible(bool visible)
    {
        if (panel == null) return;

        panel.alpha = visible ? 1 : 0;
        panel.interactable = visible;
        panel.blocksRaycasts = visible;
    }
}
