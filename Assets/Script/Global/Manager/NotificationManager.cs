using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    [SerializeField] private float offset;
    [Header("Notification Settings")]
    public Transform notificationParent;
    public GameObject notificationPrefab;
    public float displayTime = 3f;
    public float fadeDuration = 0.5f;
    public float verticalSpacing = 30f; // 每条提示之间的间距

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="message">提示文本</param>
    /// <param name="color">文字颜色</param>
    /// <param name="icon">可选图标</param>
    public void ShowNotification(string message, Color? color = null, Sprite icon = null)
    {
        //AudioManager.Instance.PlaySFX(ResourceCenter.GetAudioPath("Notification"));
        GameObject go = Instantiate(notificationPrefab, notificationParent);

        var iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
        var text = go.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();

        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }

        if (text != null)
        {
            text.text = message;
            if (color.HasValue)
                text.color = color.Value;
        }
        float yOffset = notificationParent.childCount * verticalSpacing+offset;
        go.transform.localPosition = new Vector3(go.transform.localPosition.x, -yOffset, 0);
        StartCoroutine(FadeAndDestroy(go));
    }

    private IEnumerator FadeAndDestroy(GameObject go)
    {
        yield return new WaitForSeconds(displayTime);

        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        Destroy(go);

    }
}
