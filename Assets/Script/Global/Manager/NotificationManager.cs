using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    [Header("Notification Settings")]
    public Transform notificationParent;
    public GameObject notificationPrefab;

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
    public void ShowNotification(string message, string title,Color? color = null)
    {
        if (Notification.count > 3)
            return;
        // 清除旧的提示
        foreach (Transform child in notificationParent)
        {
            Destroy(child.gameObject);
        }

        GameObject go = Instantiate(notificationPrefab, notificationParent);
        var notification = go.GetComponent<Notification>();
        notification.Init(message,title);

    }


}
