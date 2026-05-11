using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("缩放参数")]
    public float hoverScale = 1.1f;
    public float clickScale = 0.9f;
    public float duration = 0.15f;
    public bool isMainTitle = false;

    [Header("视觉效果参数")]
    public Color hoverColor = new Color(1f, 1f, 1f, 1f);
    public Color clickColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color disabledColor = new Color(1f, 1f, 1f, 0.4f);
    public bool enableGlow = true;
    public float glowIntensity = 0.15f;

    private Vector3 originalScale;
    private Button button;
    private Image targetImage;
    private Color originalColor;
    private Tween loopTween;
    private Tween colorTween;
    private Tween fadeTween;

    void Start()
    {
        button = GetComponent<Button>();
        targetImage = GetComponent<Image>();
        if (targetImage != null)
            originalColor = targetImage.color;

        originalScale = transform.localScale;

        // ✅ 主标题特殊动画（呼吸缩放 + 轻微旋转 + 渐变透明）
        if (isMainTitle)
        {
            // 呼吸缩放
            loopTween = transform.DOScale(originalScale * 1.1f, 1.2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            // 轻微旋转
            transform.DORotate(new Vector3(0, 0, 3f), 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            // ✅ 透明度渐变（淡入淡出）
            if (targetImage != null)
            {
                fadeTween = targetImage.DOFade(0.6f, 1.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                // 没有 Image，用 CanvasGroup 控制透明度
                CanvasGroup cg = GetComponent<CanvasGroup>();
                if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
                fadeTween = cg.DOFade(0.6f, 1.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        // 按钮状态监听
        button.onClick.AddListener(() => { CheckButtonState(); });
        CheckButtonState();
    }

    void CheckButtonState()
    {
        if (!button.interactable)
        {
            targetImage?.DOColor(disabledColor, 0.3f);
            transform.DOKill();
            fadeTween?.Kill();
        }
        else
        {
            targetImage?.DOColor(originalColor, 0.3f);
            if (isMainTitle && fadeTween != null && !fadeTween.IsPlaying())
                fadeTween.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable || isMainTitle) return;

        transform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutBack);
        if (targetImage != null)
        {
            colorTween?.Kill();
            colorTween = targetImage.DOColor(hoverColor + (enableGlow ? new Color(glowIntensity, glowIntensity, glowIntensity, 0) : Color.clear), duration);
        }

        if (enableGlow)
            transform.DOPunchScale(Vector3.one * 0.03f, 0.3f, 5, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable || isMainTitle) return;

        transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
        if (targetImage != null)
        {
            colorTween?.Kill();
            colorTween = targetImage.DOColor(originalColor, duration);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!button.interactable) return;
        if (loopTween != null && loopTween.IsPlaying()) return;

        transform.DOKill();
        transform.DOScale(originalScale * clickScale, duration * 0.5f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.OutBack);
            });

        if (targetImage != null)
        {
            colorTween?.Kill();
            targetImage.DOColor(clickColor, 0.05f)
                .OnComplete(() => targetImage.DOColor(originalColor, 0.2f));
        }
    }

    private void OnDestroy()
    {
        loopTween?.Kill();
        colorTween?.Kill();
        fadeTween?.Kill();
    }
}
