using UnityEngine;

public class MouseTipsGroup : MonoBehaviour
{
    public CanvasGroup group;        // 拖父物体上的 CanvasGroup
    public float fadeSpeed = 2f;     // 渐变速度
    public float idleDelay = 0.5f;   // 滚轮停止多久后才显示（仅在没滚过时有效）
    private float idleTimer = 0f;    // 计时器
    private bool fadingOut = true;   // 是否正在淡出
    private bool hasScrolled = false; // 是否滚动过滚轮

    void Start()
    {
        if (group == null)
            group = GetComponent<CanvasGroup>();

        group.alpha = 0f;
        fadingOut = true;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // 如果滚轮滚动过一次 → 永远隐藏
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            hasScrolled = true;
            FadeOut();
        }

        // 如果从未滚动过滚轮，才允许显示
        if (!hasScrolled)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDelay)
            {
                FadeIn();
            }
        }

        // 执行渐变
        if (fadingOut)
        {
            group.alpha -= Time.deltaTime * fadeSpeed;
            if (group.alpha < 0f)
                group.alpha = 0f;
        }
        else
        {
            group.alpha += Time.deltaTime * fadeSpeed;
            if (group.alpha > 1f)
                group.alpha = 1f;
        }
    }

    public void FadeOut()
    {
        fadingOut = true;
    }

    public void FadeIn()
    {
        fadingOut = false;
    }
}
