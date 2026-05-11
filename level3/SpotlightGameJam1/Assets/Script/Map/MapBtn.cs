using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class MapBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button btn;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private GameObject frame;

    public event Action OnHover; // 鼠标悬停事件

    [Header("悬停漂浮参数")]
    [SerializeField] private float hoverFloatAmplitude = 5f; // 漂浮高度
    [SerializeField] private float hoverFloatSpeed = 3f;     // 漂浮速度

    private CanvasGroup frameCanvasGroup;
    private Vector3 originalScale;

    [HideInInspector] public Vector3 basePosition;
    private bool isHovered = false;
    public int id = -1;

    private Tween frameTween; 

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

    public void Init(bool isUnlock, string sceneName, UnityAction action, int id)
    {
        btn.interactable = isUnlock;
        nameTxt.text = sceneName;
        btn.onClick.AddListener(action);
        this.id = id;
        if (frameCanvasGroup != null)
        {
            frameCanvasGroup.alpha = 0f;
            frame.SetActive(false);
        }
    }

    private Color normalColor = Color.gray;
    private Color highlightColor = Color.black;
    [SerializeField] private float textHoverScale = 1.1f;
    [SerializeField] private float textHoverSpeed = 3f;
    [SerializeField] private float textHoverAmplitude = 3f;
    [SerializeField] private float colorLerpSpeed = 5f;
    [SerializeField] private float normalFloatAmplitude = 2f;
    [SerializeField] private float normalFloatSpeed = 1f;

    [Header("Frame旋转参数")]
    [SerializeField] private float frameRotateSpeed = 45f;

    private void Update()
    {
        Vector3 normalOffset = Vector3.up * Mathf.Sin(Time.time * normalFloatSpeed) * normalFloatAmplitude;
        Vector3 hoverOffset = isHovered ? Vector3.up * Mathf.Sin(Time.time * hoverFloatSpeed) * hoverFloatAmplitude : Vector3.zero;
        transform.localPosition = basePosition + normalOffset + hoverOffset;

        if (nameTxt != null)
        {
            Color targetColor = isHovered ? highlightColor : normalColor;
            nameTxt.color = Color.Lerp(nameTxt.color, targetColor, Time.deltaTime * colorLerpSpeed);

            float floatOffset = Mathf.Sin(Time.time * textHoverSpeed) * textHoverAmplitude;
            float scale = isHovered ? textHoverScale : 1f;
            nameTxt.transform.localScale = Vector3.one * scale;
            nameTxt.transform.localPosition = new Vector3(nameTxt.transform.localPosition.x, floatOffset, nameTxt.transform.localPosition.z);
        }

        if (frame != null && frame.activeSelf)
        {
            frame.transform.Rotate(Vector3.forward, frameRotateSpeed * Time.deltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!btn.interactable) return;

        isHovered = true;
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/MouseFlow (2).wav");
        FadeFrame(true);
        OnHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!btn.interactable) return;

        isHovered = false;
        FadeFrame(false);
    }

    /// <summary>
    /// 使用 DOTween 淡入淡出 frame
    /// </summary>
    public void FadeFrame(bool show, float duration = 0.25f)
    {
        if (frameCanvasGroup == null || frame == null) return;

        // 停止旧动画
        frameTween?.Kill();

        if (show)
        {
            frame.SetActive(true);
            frame.transform.localScale = originalScale * 1.5f;
            frameCanvasGroup.alpha = 0f;

            // 同时控制缩放和透明度
            frameTween = DOTween.Sequence()
                .Join(frameCanvasGroup.DOFade(1f, duration))
                .Join(frame.transform.DOScale(originalScale, duration).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    frameCanvasGroup.alpha = 1f;
                    frame.transform.localScale = originalScale;
                });
        }
        else
        {
            frameTween = DOTween.Sequence()
                .Join(frameCanvasGroup.DOFade(0f, duration))
                .Join(frame.transform.DOScale(originalScale * 0.5f, duration).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    frame.SetActive(false);
                    frame.transform.localScale = originalScale;
                });
        }
    }
}
