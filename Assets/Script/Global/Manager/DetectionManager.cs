using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DetectionManager
{

    // 2D射线检测
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        return hit.collider != null;
    }

    // 2D射线检测（带命中信息）
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask, out RaycastHit2D hitInfo)
    {
        hitInfo = Physics2D.Raycast(origin, direction, distance, layerMask);
        return hitInfo.collider != null;
    }

    // 多射线地面检测
    public static GroundCheckResult MultiRayGroundCheck
        (
        Vector2 position,//检测的中心位置
        float width, //检测的宽度范围（敌人的脚底宽度）
        int rayCount, //射线数量
        float distance, //射线长度
        LayerMask groundLayer//地面层级
        )
    {
        var result = new GroundCheckResult();
        int groundHits = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount > 1 ? (float)i / (rayCount - 1) : 0.5f;
            float x = Mathf.Lerp(-width / 2, width / 2, t);//线性插值函数
            Vector2 rayOrigin = position + new Vector2(x, 0);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, distance, groundLayer);

            if (hit.collider != null)
            {
                groundHits++;
                result.groundNormal = hit.normal;

                // 边缘检测
                if (i == 0) result.isLeftGrounded = true;
                if (i == rayCount - 1) result.isRightGrounded = true;
            }
        }

        result.isGrounded = groundHits > 0;
        result.isLeftEdge = !result.isLeftGrounded;
        result.isRightEdge = !result.isRightGrounded;

        return result;
    }

    // 圆形范围检测
    public static bool CircleCast(Vector2 center, float radius, LayerMask layerMask)
    {
        Collider2D hit = Physics2D.OverlapCircle(center, radius, layerMask);
        return hit != null;
    }

    //// 视线检测（考虑障碍物）
    //public static bool LineOfSight(Vector2 from, Vector2 to, LayerMask obstacleLayer)
    //{
    //    Vector2 direction = to - from;
    //    float distance = direction.magnitude;

    //    RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, obstacleLayer);
    //    return hit.collider == null; // 没有障碍物返回true
    //}
}

// 地面检测结果结构
public struct GroundCheckResult
{
    public bool isGrounded;
    public bool isLeftEdge;//是否在左边缘
    public bool isRightEdge;
    public bool isLeftGrounded;//左边是否有地面
    public bool isRightGrounded;
    public Vector2 groundNormal;//地面法向量

    public bool IsOnAnyEdge => isLeftEdge || isRightEdge;//是否在边缘
}