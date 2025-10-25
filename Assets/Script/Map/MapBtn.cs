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

    [Header("悬停漂浮参数")]
    [SerializeField] private float hoverFloatAmplitude = 5f; // 漂浮高度
    [SerializeField] private float hoverFloatSpeed = 3f;     // 漂浮速度

    private CanvasGroup frameCanvasGroup;
    private Coroutine frameCoroutine = null;
    private Vector3 originalScale;

    [HideInInspector] public Vector3 basePosition; // MapBtnGroup 设置的基础位置
    private bool isHovered = false;
    public int id = -1;

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

    public void Init(bool isUnlock, string sceneName, UnityAction action,int id)
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
    [SerializeField] private float textHoverScale = 1.1f; // 悬停文字放大比例
    [SerializeField] private float textHoverSpeed = 3f;   // 文字漂浮速度
    [SerializeField] private float textHoverAmplitude = 3f; // 文字漂浮幅度
    [SerializeField] private float colorLerpSpeed = 5f;   // 颜色渐变速度
    [SerializeField] private float normalFloatAmplitude = 2f; // 正常状态漂浮幅度
    [SerializeField] private float normalFloatSpeed = 1f;     // 正常状态漂浮速度

    [Header("Frame旋转参数")]
    [SerializeField] private float frameRotateSpeed = 45f; // 每秒旋转角度

    private void Update()
    {
        Vector3 normalOffset = Vector3.up * Mathf.Sin(Time.time * normalFloatSpeed) * normalFloatAmplitude;
        Vector3 hoverOffset = Vector3.zero;
        if (isHovered)
        {
            hoverOffset = Vector3.up * Mathf.Sin(Time.time * hoverFloatSpeed) * hoverFloatAmplitude;
        }

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
        FadeFrame(false); // frame 收回
    }

    public void FadeFrame(bool show, float duration = 0.25f)
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

        Vector3 startScale = show ? originalScale * 1.5f : originalScale; // 从大缩小
        Vector3 endScale = show ? originalScale : originalScale * 0.5f;   // 收回缩小

        float t = 0f;
        frame.SetActive(true);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float lerp = Mathf.SmoothStep(0f, 1f, t);

            frameCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            frame.transform.localScale = Vector3.Lerp(startScale, endScale, lerp);

            yield return null;
        }

        frameCanvasGroup.alpha = targetAlpha;
        frame.transform.localScale = originalScale;

        if (!show)
            frame.SetActive(false);

        frameCoroutine = null;
    }
}
