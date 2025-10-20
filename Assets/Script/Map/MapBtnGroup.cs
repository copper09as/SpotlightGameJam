using System.Collections;
using System.Collections.Generic;
using Global.Data;
using Global.Data.BattleConfig;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MapBtnGroup : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private float spacing = 300f;   // 按钮之间的间距
    [SerializeField] private float centerScale = 1.5f; // 中心按钮放大倍数
    [SerializeField] private float sideScale = 1.0f;   // 旁边按钮缩小倍数
    [SerializeField] private float moveDuration = 0.25f; // 滑动动画时间
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private Button returnStartBtn;
    int unlockedIndex = 3;
    private readonly List<Button> mapSelectButtonGroup = new List<Button>();
    private int currentIndex = 0;
    private bool isMoving = false;
    private void Awake()
    {
        AudioManager.Instance.PlayBGM(StringResource.MapBgmPath);
        StartCoroutine(LoadMapButtonsCoroutine());
        returnStartBtn.onClick.AddListener(ReturnStartScene);
    }
    private void ReturnStartScene()
    {
        SceneChangeManager.Instance.LoadScene("StartScene");
    }
    private IEnumerator LoadMapButtonsCoroutine()
    {
       
        var levelDataList = GameConfig.Instance.LevtlDC.levelDataList;
        for (int i = 0; i < levelDataList.Count; i++)
        {
            var levelData = levelDataList[i];
            var sceneName = levelDataList[i].SceneName;
            CreateButton(levelData.Id, levelData.SpritePath,sceneName, i <= unlockedIndex);
            yield return null;
        }

        LayoutButtons(true); // 初始布局
        if (panelCanvasGroup != null)
        {
            StartCoroutine(FadeOutPanel());
        }
    }
    private IEnumerator FadeOutPanel(float duration = 0.3f)
    {
        float t = 0f;
        float startAlpha = panelCanvasGroup.alpha;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        panelCanvasGroup.alpha = 0f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
    }



    private void CreateButton(int id, string spritePath,string sceneName,bool isLock)
    {
        var btnInstance = Instantiate(buttonPrefab, transform);
        var mapBtn = btnInstance.GetComponent<MapBtn>();
        if (!string.IsNullOrEmpty(spritePath))
        {
            var image = btnInstance.GetComponent<Image>();
            image.sprite = ResManager.LoadSprite(StringResource.GetImagePath(spritePath));
        }

        int captureId = id;
        mapBtn.Init(isLock, sceneName, (() => EnterByIndex(captureId)));
        mapSelectButtonGroup.Add(mapBtn.GetComponent<Button>());
    }

    private void EnterByIndex(int id)
    {
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
        BattleConfig.Instance.levelId = id;
        var scenePath = GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == id).ScenePath;
        SceneChangeManager.Instance.LoadScene(scenePath);
    }

    /// <summary>
    /// 布局所有按钮位置和缩放
    /// </summary>
    private void LayoutButtons(bool isInit = false)
    {
        for (int i = 0; i < mapSelectButtonGroup.Count; i++)
        {
            int offset = i - currentIndex;

            if (!isInit)
            {
                if (offset > mapSelectButtonGroup.Count / 2) offset -= mapSelectButtonGroup.Count;
                if (offset < -mapSelectButtonGroup.Count / 2) offset += mapSelectButtonGroup.Count;
            }

            mapSelectButtonGroup[i].transform.localPosition = Vector3.right * offset * spacing;
            float scale = (offset == 0) ? centerScale : sideScale;
            mapSelectButtonGroup[i].transform.localScale = Vector3.one * scale;
        }
    }


    private void Update()
    {
        if (isMoving) return;

        float scroll = GameController.GetScrollDelta();
        if (scroll > 0f)
        {
            MoveToPrev();
        }
        else if (scroll < 0f)
        {
            MoveToNext();
        }
    }


    private void MoveToNext()
    {
        if (currentIndex >= mapSelectButtonGroup.Count - 1) return; // 到最右边了
        currentIndex++;
        StartCoroutine(AnimateLayout());
    }

    private void MoveToPrev()
    {
        if (currentIndex <= 0) return; // 到最左边了
        currentIndex--;
        StartCoroutine(AnimateLayout());
    }


    /// <summary>
    /// 平滑移动布局
    /// </summary>
    private IEnumerator AnimateLayout()
    {
        isMoving = true;
        float t = 0f;

        // 保存当前位置和缩放
        List<Vector3> startPos = new List<Vector3>();
        List<Vector3> startScale = new List<Vector3>();
        for (int i = 0; i < mapSelectButtonGroup.Count; i++)
        {
            startPos.Add(mapSelectButtonGroup[i].transform.localPosition);
            startScale.Add(mapSelectButtonGroup[i].transform.localScale);
        }

        // 计算目标位置和缩放
        List<Vector3> targetPos = new List<Vector3>();
        List<Vector3> targetScale = new List<Vector3>();
        for (int i = 0; i < mapSelectButtonGroup.Count; i++)
        {
            int offset = i - currentIndex;
            targetPos.Add(Vector3.right * offset * spacing);
            targetScale.Add(Vector3.one * (offset == 0 ? centerScale : sideScale));
        }

        // 平滑插值
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            float lerpT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));

            for (int i = 0; i < mapSelectButtonGroup.Count; i++)
            {
                mapSelectButtonGroup[i].transform.localPosition =
                    Vector3.Lerp(startPos[i], targetPos[i], lerpT);
                mapSelectButtonGroup[i].transform.localScale =
                    Vector3.Lerp(startScale[i], targetScale[i], lerpT);
            }

            yield return null;
        }

        // 确保最终位置和缩放精确
        for (int i = 0; i < mapSelectButtonGroup.Count; i++)
        {
            mapSelectButtonGroup[i].transform.localPosition = targetPos[i];
            mapSelectButtonGroup[i].transform.localScale = targetScale[i];
        }

        isMoving = false;
    }




}
