using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackGroundRolling : MonoBehaviour
{
    [SerializeField]
    private List<Transform> backGrounds = new List<Transform>();
    [SerializeField]
    private float rollingSpeed= 1;
    [SerializeField]
    private float backGroundWidth = 1;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)//遍历所有一级子物体
        {
            backGrounds.Add(child);
        }
        SpriteRenderer spriteRenderer = backGrounds[0].GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            backGroundWidth = spriteRenderer.sprite.bounds.size.x * backGrounds[0].localScale.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i =0; i< backGrounds.Count;i++)
        {
            backGrounds[i].position -= new Vector3(rollingSpeed * Time.deltaTime, 0, 0);
        }
        if (backGrounds[0].position.x < -backGroundWidth)
            RecycleFirstBackground();
    }

    void RecycleFirstBackground()
    {
        Transform firstBg = backGrounds[0];
        Transform lastBg = backGrounds[backGrounds.Count - 1];

        // 将第一个背景移到最后一个背景的右边
        firstBg.position = lastBg.position + new Vector3(backGroundWidth, 0, 0);

        // 更新列表顺序
        backGrounds.RemoveAt(0);
        backGrounds.Add(firstBg);
    }
}
