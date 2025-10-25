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
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = frame.transform.localScale;

        if (frame != null)
        {
            frameCanvasGroup = frame.GetComponent<CanvasGroup>();
            if (frameCanvasGroup == null)
                frameCanvasGroup = frame.AddComponent<CanvasGroup>();

            frameCanvasGroup.alpha = 0f;
            frameCanvasGroup.interactable = false;
            frameCanvasGroup.blocksRaycasts = false;
            frame.SetActive(false);
        }
    }

    public void Init(bool isUnlock, string sceneName, UnityAction action)
    {
        btn.interactable = isUnlock;
        nameTxt.text = sceneName;
        btn.onClick.AddListener(action);

        if (frameCanvasGroup != null)
        {
            frameCanvasGroup.alpha = 0f;
            frameCanvasGroup.interactable = false;
            frameCanvasGroup.blocksRaycasts = false;
            frame.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!btn.interactable) return;

        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/MouseFlow (2).wav");
        FadeFrame(true); // 淡入 + 缩放弹跳
        OnHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!btn.interactable) return;
        FadeFrame(false);
    }

    private void FadeFrame(bool show, float duration = 0.25f)
    {
        if (frameCoroutine != null)
            StopCoroutine(frameCoroutine);

        frameCoroutine = StartCoroutine(FadeFrameCoroutine(show, duration));
    }
    private IEnumerator FadeFrameCoroutine(bool show, float duration)
    {
        if (frameCanvasGroup == null) yield break;

        float startAlpha = frameCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;

        Vector3 startScale = show ? originalScale * 1.5f : originalScale; // 从1.5倍缩小到原始
        Vector3 endScale = show ? originalScale : originalScale * 0.5f; // 淡出时缩小到0.5倍
        float t = 0f;

        frame.SetActive(true);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float lerp = Mathf.SmoothStep(0f, 1f, t);

            // alpha
            frameCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);

            // 从外往里弹效果
            frame.transform.localScale = Vector3.Lerp(startScale, endScale, lerp);

            yield return null;
        }

        frameCanvasGroup.alpha = targetAlpha;
        frame.transform.localScale = originalScale;

        if (!show) frame.SetActive(false);

        frameCoroutine = null;
    }

    // 中心按钮高亮
    public void SetHighlight(bool highlight)
    {
        if (frameCanvasGroup == null) return;
        frameCanvasGroup.alpha = highlight ? 1f : 0f;
        frame.SetActive(highlight);
    }
}
