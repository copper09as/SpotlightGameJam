using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DetectionManager
{

    // 2D���߼��
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        return hit.collider != null;
    }

    // 2D���߼�⣨��������Ϣ��
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, out RaycastHit2D hitInfo)
    {
        hitInfo = Physics2D.Raycast(origin, direction, distance, layerMask);
        return hitInfo.collider != null;
    }

    // �����ߵ�����
    public static GroundCheckResult MultiRayGroundCheck
        (
        Vector2 position,//��������λ��
        float width, //���Ŀ�ȷ�Χ�����˵Ľŵ׿�ȣ�
        int rayCount, //��������
        float distance, //���߳���
        LayerMask groundLayer//����㼶
        )
    {
        var result = new GroundCheckResult();
        int groundHits = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount > 1 ? (float)i / (rayCount - 1) : 0.5f;
            float x = Mathf.Lerp(-width / 2, width / 2, t);//���Բ�ֵ����
            Vector2 rayOrigin = position + new Vector2(x, 0);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, distance, groundLayer);

            if (hit.collider != null)
            {
                groundHits++;
                result.groundNormal = hit.normal;

                // ��Ե���
                if (i == 0) result.isLeftGrounded = true;
                if (i == rayCount - 1) result.isRightGrounded = true;
            }
        }

        result.isGrounded = groundHits > 0;
        result.isLeftEdge = !result.isLeftGrounded;
        result.isRightEdge = !result.isRightGrounded;

        return result;
    }

    // Բ�η�Χ���
    public static bool CircleCast(Vector2 center, float radius, LayerMask layerMask)
    {
        Collider2D hit = Physics2D.OverlapCircle(center, radius, layerMask);
        return hit != null;
    }

    //// ���߼�⣨�����ϰ��
    //public static bool LineOfSight(Vector2 from, Vector2 to, LayerMask obstacleLayer)
    //{
    //    Vector2 direction = to - from;
    //    float distance = direction.magnitude;

    //    RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, obstacleLayer);
    //    return hit.collider == null; // û���ϰ��ﷵ��true
    //}
}

// ���������ṹ
public struct GroundCheckResult
{
    public bool isGrounded;
    public bool isLeftEdge;//�Ƿ������Ե
    public bool isRightEdge;
    public bool isLeftGrounded;//����Ƿ��е���
    public bool isRightGrounded;
    public Vector2 groundNormal;//���淨����

    public bool IsOnAnyEdge => isLeftEdge || isRightEdge;//�Ƿ��ڱ�Ե
}