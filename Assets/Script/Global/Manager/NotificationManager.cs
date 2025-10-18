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
    /// ��ʾ��ʾ
    /// </summary>
    /// <param name="message">��ʾ�ı�</param>
    /// <param name="color">������ɫ</param>
    /// <param name="icon">��ѡͼ��</param>
    public void ShowNotification(string message, string title,Color? color = null)
    {
        if (Notification.count > 3)
            return;
        // ����ɵ���ʾ
        foreach (Transform child in notificationParent)
        {
            Destroy(child.gameObject);
        }

        GameObject go = Instantiate(notificationPrefab, notificationParent);
        var notification = go.GetComponent<Notification>();
        notification.Init(message,title);

    }


}
