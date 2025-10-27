using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinallyPanel : MonoBehaviour
{
    [Header("漫画图片")]
    [SerializeField] private List<Sprite> images = new List<Sprite>();
    [SerializeField] private Image displayImage;
    [SerializeField] private float fadeDuration = 0.5f; 
    [SerializeField] private float finalFadeDuration = 1f;
    [SerializeField] private Image blackOverlay;

    private int currentIndex = 0;
    private bool isPlaying = true;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (images.Count == 0 || displayImage == null || blackOverlay == null)
        {
            Debug.LogError("请在 Inspector 中设置 images、displayImage 和 blackOverlay!");
            isPlaying = false;
            return;
        }

        displayImage.color = new Color(1f, 1f, 1f, 0f);
        blackOverlay.color = new Color(0f, 0f, 0f, 0f);

        // 显示第一张图片
        fadeCoroutine = StartCoroutine(FadeInImage(images[0]));
    }

    void Update()
    {
        if (!isPlaying) return;

        if (GameController.Controller.Main.LeftClick.triggered)
        {
            NextImage();
        }
    }

    private void NextImage()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        currentIndex++;

        if (currentIndex >= images.Count)
        {
            StartCoroutine(FadeToBlackAndLoadScene());
        }
        else
        {
            fadeCoroutine = StartCoroutine(FadeInImage(images[currentIndex]));
        }
    }

    private IEnumerator FadeInImage(Sprite newSprite)
    {
        displayImage.sprite = newSprite;
        float t = 0f;
        Color startColor = new Color(1f, 1f, 1f, 0f);
        Color endColor = new Color(1f, 1f, 1f, 1f);

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / fadeDuration;
            displayImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        displayImage.color = endColor;
    }

    private IEnumerator FadeToBlackAndLoadScene()
    {
        isPlaying = false;

        float t = 0f;
        Color startColor = blackOverlay.color;
        Color endColor = new Color(0f, 0f, 0f, 1f);

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / finalFadeDuration;
            blackOverlay.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        SceneChangeManager.Instance.LoadScene("StartScene");
    }
}
