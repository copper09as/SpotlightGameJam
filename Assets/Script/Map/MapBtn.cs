using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MapBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button btn;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private GameObject frame;

    public event Action OnHover; // 鼠标悬停事件

    private CanvasGroup frameCanvasGroup;
    private Coroutine frameCoroutine = null;

    private void Awake()
    {
        if (frame != null)
        {
            // 添加 CanvasGroup 控制透明度
            frameCanvasGroup = frame.GetComponent<CanvasGroup>();
            if (frameCanvasGroup == null)
                frameCanvasGroup = frame.AddComponent<CanvasGroup>();

            frameCanvasGroup.alpha = 0f;
            frameCanvasGroup.interactable = false;
            frameCanvasGroup.blocksRaycasts = false;
            frame.SetActive(false); // 初始隐藏
        }
    }

    /// <summary>
    /// 初始化按钮
    /// </summary>
    public void Init(bool isUnlock, string sceneName, UnityAction action)
    {
        btn.interactable = isUnlock;
        nameTxt.text = sceneName;
        btn.onClick.AddListener(action);

        // frame 默认隐藏
        if (frameCanvasGroup != null)
        {
            frameCanvasGroup.alpha = 0f;
            frameCanvasGroup.interactable = false;
            frameCanvasGroup.blocksRaycasts = false;
            frame.SetActive(false);
        }
    }

    /// <summary>
    /// 鼠标进入
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!btn.interactable) return;

        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/MouseFlow (2).wav");

        FadeFrame(true); // 淡入
        OnHover?.Invoke(); // 通知 MapBtnGroup
    }

    /// <summary>
    /// 鼠标离开
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!btn.interactable) return;

        FadeFrame(false); // 淡出
    }

    /// <summary>
    /// 控制 frame 淡入淡出
    /// </summary>
    /// <param name="show">true 淡入，false 淡出</param>
    /// <param name="duration">持续时间</param>
    private void FadeFrame(bool show, float duration = 0.25f)
    {
        if (frameCoroutine != null)
        {
            StopCoroutine(frameCoroutine);
            frameCoroutine = null;
        }
        frameCoroutine = StartCoroutine(FadeFrameCoroutine(show, duration));
    }

    private IEnumerator FadeFrameCoroutine(bool show, float duration)
    {
        if (frameCanvasGroup == null) yield break;

        float startAlpha = frameCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;
        float t = 0f;

        if (show) frame.SetActive(true); // 确保淡入可见

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            frameCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        frameCanvasGroup.alpha = targetAlpha;

        if (!show)
            frame.SetActive(false);

        frameCoroutine = null;
    }
}
