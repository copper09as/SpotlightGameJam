using System.Collections.Generic;
using UnityEngine;

public class BackGroundRolling : MonoBehaviour
{
    [SerializeField]
    private List<Transform> backGrounds = new List<Transform>();
    [SerializeField]
    private float rollingSpeed = 1;
    [SerializeField]
    private float backGroundWidth = 1;

    [Header("鼠标偏移参数")]
    [SerializeField]
    private float maxOffset = 2f; // 最大偏移量
    [SerializeField]
    private float followSpeed = 5f; // 平滑跟随速度

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;

        foreach (Transform child in transform)
        {
            backGrounds.Add(child);
        }

        SpriteRenderer spriteRenderer = backGrounds[0].GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            backGroundWidth = spriteRenderer.sprite.bounds.size.x * backGrounds[0].localScale.x * transform.localScale.x;
        }
    }

    void Update()
    {
        UpdateRolling();
        UpdateMouseOffset();
    }

    void UpdateRolling()
    {
        // 背景滚动
        for (int i = 0; i < backGrounds.Count; i++)
        {
            backGrounds[i].position -= new Vector3(rollingSpeed * Time.unscaledDeltaTime, 0, 0);
        }

        // 回收第一个背景
        if (backGrounds[0].position.x < -backGroundWidth)
            RecycleFirstBackground();
    }

    void RecycleFirstBackground()
    {
        Transform firstBg = backGrounds[0];
        Transform lastBg = backGrounds[backGrounds.Count - 1];
        firstBg.position = lastBg.position + new Vector3(backGroundWidth, 0, 0);
        backGrounds.RemoveAt(0);
        backGrounds.Add(firstBg);
    }

    void UpdateMouseOffset()
    {

        Vector2 mousePos = GameController.GetScreenPos();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Vector2 offsetNormalized = (mousePos - screenCenter) / screenCenter;
        offsetNormalized = Vector2.ClampMagnitude(offsetNormalized, 1f);

        Vector3 targetPosition = initialPosition + new Vector3(0, offsetNormalized.y * maxOffset, 0);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * followSpeed);
    }
}
