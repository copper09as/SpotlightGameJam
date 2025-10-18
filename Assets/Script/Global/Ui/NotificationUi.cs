using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Notification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button urlBtn; 

  private string openUrl = "https://space.bilibili.com/80632239/dynamic?spm_id_from=333.1365.list.card_avatar.click";

    public static int count;

    public void Init(string note, string title)
    {
        notificationText.text = note;
        titleText.text = title;

        closeBtn.onClick.AddListener(DestroySelf);
        restartBtn.onClick.AddListener(ForceRestart);
        urlBtn.onClick.AddListener(OpenUrl);

        count++;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void ForceRestart()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    private void OpenUrl()
    {
        if (!string.IsNullOrEmpty(openUrl))
        {
            Application.OpenURL(openUrl);
        }
    }

    private void OnDestroy()
    {
        count--;
    }
}
