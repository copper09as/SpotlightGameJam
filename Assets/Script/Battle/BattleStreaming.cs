using System;
using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using Global.Data;
using Global.Data.BattleConfig;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class BattleStreaming : MonoBehaviour
{
    [Header("加载界面")]
    [SerializeField] private CanvasGroup panel;
    private TextMeshProUGUI tipText;
    private TextMeshProUGUI sceneNameText;

    [Header("漫画页系统")]
    [SerializeField] private CanvasGroup comicPanel;   // 放漫画图片的画布
    [SerializeField] private Image comicImage;         // 用于显示当前页图片
    [SerializeField] private Sprite[] comicPages;      // 所有漫画页
    private int currentPageIndex = 0;
    private bool isComicFinished = false;

    [SerializeField] private float fadeDuration = 2f; 

    private bool isLoading = false;

    private void Awake()
    {
        if (panel != null && panel.transform.childCount > 0)
        {
            var child1 = panel.transform.GetChild(1);
            tipText = child1.GetComponent<TextMeshProUGUI>();

            var child2 = panel.transform.GetChild(0);
            sceneNameText = child2.GetComponent<TextMeshProUGUI>();
        }
        var levelData = GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == BattleConfig.Instance.levelId);
        if (levelData != null)
        {
            if (tipText != null) tipText.text = levelData.noteString;
            if (sceneNameText != null) sceneNameText.text = levelData.SceneName;
        }
    }

    private void Start()
    {
        // 注册输入
        GameController.Controller.Main.Reset.started += ResetBattle;
        GameController.Controller.Main.LeftClick.started += OnLeftClick;

        if (comicPages != null && comicPages.Length > 0)
        {
            ShowComicPage(0);
        }
        else
        {
            StartCoroutine(LoadBattle());
        }
    }

    private void OnDestroy()
    {
        GameController.Controller.Main.Reset.started -= ResetBattle;
        GameController.Controller.Main.LeftClick.started -= OnLeftClick;
    }

    /// <summary>
    /// 鼠标左键逻辑
    /// </summary>
    private void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!isComicFinished)
        {
            NextComicPage();
        }
    }

    /// <summary>
    /// 播放下一页漫画
    /// </summary>
    private void NextComicPage()
    {
        if (comicPages == null || comicPages.Length == 0 || comicImage == null) return;

        currentPageIndex++;

        if (currentPageIndex < comicPages.Length)
        {
            comicImage.sprite = comicPages[currentPageIndex];
        }
        else
        {
            isComicFinished = true;
            StartCoroutine(FadeOutComicAndLoad());
        }
    }

    /// <summary>
    /// 显示某一页漫画
    /// </summary>
    private void ShowComicPage(int index)
    {
        if (comicPanel != null)
        {
            comicPanel.alpha = 1f;
            comicPanel.interactable = true;
            comicPanel.blocksRaycasts = true;
        }

        if (comicImage != null && comicPages != null && index >= 0 && index < comicPages.Length)
        {
            comicImage.sprite = comicPages[index];
        }
    }

    /// <summary>
    /// 淡出漫画面板后再加载
    /// </summary>
    private IEnumerator FadeOutComicAndLoad()
    {
        if (comicPanel != null)
        {
            float time = 0f;
            float startAlpha = comicPanel.alpha;
            while (time < 1f)
            {
                time += Time.unscaledDeltaTime;
                comicPanel.alpha = Mathf.Lerp(startAlpha, 0f, time / 1f);
                yield return null;
            }

            comicPanel.alpha = 0f;
            comicPanel.interactable = false;
            comicPanel.blocksRaycasts = false;
        }

        StartCoroutine(LoadBattle());
    }

    /// <summary>
    /// 战斗加载逻辑
    /// </summary>
    private IEnumerator LoadBattle()
    {
        if (isLoading) yield break;
        isLoading = true;
        SetPanelVisible(true);

        var entityManager = new EntityManager();
        EntityUIManager.Instance.entityManager = entityManager;
        foreach (var entity in FindObjectsOfType<Entity>(true))
        {
            entityManager.Register(entity);
            entity.entityStop = true;
        }

        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(FadeOutPanel());
        foreach (var entity in entityManager.GetAllEntities())
        {
            entity.entityStop =false;
        }
        isLoading = false;
        EntityUIManager.Instance.isLoading = false;
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

    private void ResetBattle(InputAction.CallbackContext context)
    {
        SceneChangeManager.Instance.ReloadCurrentScene();
    }
}
