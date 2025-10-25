using System.Collections;
using System.Collections.Generic;
using Global.Data;
using Global.Data.BattleConfig;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MapBtnGroup : MonoBehaviour
{
    [Header("地图关卡按钮设置")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private float spacing = 300f;
    [SerializeField] private float centerScale = 1.5f;
    [SerializeField] private float sideScale = 1.0f;
    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private Button returnStartBtn;

    [Header("人物跳跃")]
    [SerializeField] private RectTransform mapCharacter;
    [SerializeField] private float jumpHeight = 120f;
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float charYOffset = 100f;
    [SerializeField] private float centerExtraHeight = 20f; 

    private readonly List<MapBtn> mapBtns = new List<MapBtn>();
    private readonly List<Button> mapSelectButtonGroup = new List<Button>();
    private int currentIndex = 3;
    private bool isMoving = false;
    private bool isJumping = false;
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float jumpDistanceScale = 1f;
    private Coroutine currentJumpCoroutine = null;
    private enum JumpSource { None, Scroll, Hover }
    private JumpSource currentJumpSource = JumpSource.None;
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

        // 先添加开头的虚假按钮
        for (int i = 0; i < 3; i++)
        {
            CreateFakeButton();
            yield return null;
        }

        for (int i = 0; i < levelDataList.Count; i++)
        {
            var levelData = levelDataList[i];
            CreateButton(levelData.Id, levelData.SpritePath, levelData.SceneName, i <= BattleConfig.Instance.userData.unLockLevel);
            yield return null;
        }

        // 添加结尾的虚假按钮
        for (int i = 0; i < 3; i++)
        {
            CreateFakeButton();
            yield return null;
        }

        LayoutButtons(true);

        if (panelCanvasGroup != null)
        {
            StartCoroutine(FadeOutPanel(1f));
        }
    }

    private void CreateFakeButton()
    {
        var btnInstance = Instantiate(buttonPrefab, transform);
        var mapBtn = btnInstance.GetComponent<MapBtn>();
        mapBtn.Init(false, "", null);
        mapSelectButtonGroup.Add(mapBtn.GetComponent<Button>());
        mapBtns.Add(mapBtn);
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

    private void CreateButton(int id, string spritePath, string sceneName, bool isUnlock)
    {
        var btnInstance = Instantiate(buttonPrefab, transform);
        var mapBtn = btnInstance.GetComponent<MapBtn>();

        if (!string.IsNullOrEmpty(spritePath))
        {
            var image = btnInstance.GetComponent<Image>();
            image.sprite = ResManager.LoadSprite(StringResource.GetImagePath(spritePath));
        }

        int captureId = id;
        mapBtn.Init(isUnlock, sceneName, () => EnterByIndex(captureId));
        mapSelectButtonGroup.Add(mapBtn.GetComponent<Button>());
        mapBtns.Add(mapBtn);

        var rect = mapBtn.GetComponent<RectTransform>();

        mapBtn.OnHover += () =>
        {
            if (!isUnlock) return;

            if (currentJumpSource == JumpSource.Scroll)
                return;

            if (currentJumpCoroutine != null)
            {
                StopCoroutine(currentJumpCoroutine);
                currentJumpCoroutine = null;
                isJumping = false;
            }

            currentJumpCoroutine = StartCoroutine(CharacterJumpTo(rect, captureId, JumpSource.Hover));
        };

    }


    private void EnterByIndex(int id)
    {
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
        BattleConfig.Instance.levelId = id;
        var scenePath = GameConfig.Instance.LevtlDC.levelDataList.Find(i => i.Id == id).ScenePath;
        SceneChangeManager.Instance.LoadScene(scenePath);
    }



    private void Update()
    {
        if (isMoving || isJumping) return;

        float scroll = GameController.GetScrollDelta();
        if (scroll > 0f) MoveToPrev();
        else if (scroll < 0f) MoveToNext();
    }

    private void MoveToNext()
    {
        int maxRealIndex = mapSelectButtonGroup.Count - 4; // 末尾真实按钮
        if (currentIndex >= maxRealIndex) return; // 已到末尾真实按钮，不移动

        int prevIndex = currentIndex;
        currentIndex++;
        StartCoroutine(AnimateLayout());

        if (currentJumpCoroutine != null)
        {
            StopCoroutine(currentJumpCoroutine);
            currentJumpCoroutine = null;
            isJumping = false;
        }

        currentJumpSource = JumpSource.Scroll;
        currentJumpCoroutine = StartCoroutine(CharacterJumpTo(mapSelectButtonGroup[currentIndex].GetComponent<RectTransform>(), prevIndex, JumpSource.Scroll));
    }

    private void MoveToPrev()
    {
        int minRealIndex = 3; // 开头真实按钮
        if (currentIndex <= minRealIndex) return; // 已到开头真实按钮，不移动

        int prevIndex = currentIndex;
        currentIndex--;
        StartCoroutine(AnimateLayout());

        if (currentJumpCoroutine != null)
        {
            StopCoroutine(currentJumpCoroutine);
            currentJumpCoroutine = null;
            isJumping = false;
        }

        currentJumpSource = JumpSource.Scroll;
        currentJumpCoroutine = StartCoroutine(CharacterJumpTo(mapSelectButtonGroup[currentIndex].GetComponent<RectTransform>(), prevIndex, JumpSource.Scroll));
    }



    private void LayoutButtons(bool isInit = false)
    {
        for (int i = 0; i < mapSelectButtonGroup.Count; i++)
        {
            int offset = i - currentIndex;
            Vector3 targetPos = Vector3.right * offset * spacing;
            mapSelectButtonGroup[i].transform.localPosition = targetPos;
            float scale = (offset == 0) ? centerScale : sideScale;
            mapSelectButtonGroup[i].transform.localScale = Vector3.one * scale;

            // 给 MapBtn 赋值 basePosition
            MapBtn mapBtn = mapSelectButtonGroup[i].GetComponent<MapBtn>();
            if (mapBtn != null)
            {
                mapBtn.basePosition = targetPos;
            }
        }
    }

    private IEnumerator AnimateLayout()
    {
        isMoving = true;
        float t = 0f;

        var buttons = new List<Button>(mapSelectButtonGroup);
        List<Vector3> startBasePos = new List<Vector3>();
        List<Vector3> targetBasePos = new List<Vector3>();
        List<Vector3> startScale = new List<Vector3>();
        List<Vector3> targetScale = new List<Vector3>();

        for (int i = 0; i < buttons.Count; i++)
        {
            MapBtn mapBtn = buttons[i].GetComponent<MapBtn>();
            startBasePos.Add(mapBtn != null ? mapBtn.basePosition : buttons[i].transform.localPosition);

            int offset = i - currentIndex;
            Vector3 targetPos = Vector3.right * offset * spacing;
            targetBasePos.Add(targetPos);

            startScale.Add(buttons[i].transform.localScale);
            targetScale.Add(Vector3.one * (offset == 0 ? centerScale : sideScale));
        }

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            float lerpT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));

            for (int i = 0; i < buttons.Count; i++)
            {
                MapBtn mapBtn = buttons[i].GetComponent<MapBtn>();
                if (mapBtn != null)
                {
                    // AnimateLayout 只修改 basePosition
                    mapBtn.basePosition = Vector3.Lerp(startBasePos[i], targetBasePos[i], lerpT);
                }

                buttons[i].transform.localScale = Vector3.Lerp(startScale[i], targetScale[i], lerpT);
            }

            yield return null;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            MapBtn mapBtn = buttons[i].GetComponent<MapBtn>();
            if (mapBtn != null)
                mapBtn.basePosition = targetBasePos[i];

            buttons[i].transform.localScale = targetScale[i];
        }

        isMoving = false;
    }


    private IEnumerator CharacterJumpTo(RectTransform targetButton, int fromIndex, JumpSource source)
    {
        if (mapCharacter == null || targetButton == null) yield break;
        isJumping = true;
        currentJumpSource = source;

        var animator = mapCharacter.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Jump");

        Vector3 startPos = mapCharacter.localPosition;
        Vector3 endPos;

        if (source == JumpSource.Hover)
        {
            // 悬停跳跃，角色跳到目标按钮上
            bool isCenter = targetButton.localScale.x > 1.2f;
            float extraHeight = isCenter ? centerExtraHeight : 0f;
            endPos = targetButton.localPosition
                     + Vector3.up * (charYOffset + extraHeight)
                     + Vector3.right * xOffset;
        }
        else
        {
            // 滚轮跳跃，角色原地跳跃
            endPos = startPos;
        }

        float effectiveJumpDuration = jumpDuration * Mathf.Clamp(jumpDistanceScale, 0.5f, 2f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / effectiveJumpDuration;
            float progress = Mathf.SmoothStep(0f, 1f, t);

            // 正弦曲线控制跳跃弧度
            float heightOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            Vector3 lerped = Vector3.Lerp(startPos, endPos, progress);
            mapCharacter.localPosition = new Vector3(lerped.x, lerped.y + heightOffset, lerped.z);

            yield return null;
        }

        mapCharacter.localPosition = endPos;
        isJumping = false;
        currentJumpCoroutine = null;
        currentJumpSource = JumpSource.None;
    }



}
