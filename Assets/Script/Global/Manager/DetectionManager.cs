using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEngine;

public static class DetectionManager
{

    // 2D射线检测
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, string layerMask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance,LayerMask.GetMask(layerMask));
        return hit.collider != null;
    }
    public static Entity Raycast2DByTag(Vector2 origin, Vector2 direction, float distance,string tag)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance);
        var col = hit.collider;
        if (col == null)
            return null;
        if (col.transform.tag != tag)
        {
            return null;
        }
        return col.GetComponent<Entity>();
    }
    public static Entity OverlapBoxByTag(Vector2 center, Vector2 size, string tag)
    {
        // 检测指定矩形区域内的碰撞体
        Collider2D col = Physics2D.OverlapBox(center, size, 0f, LayerMask.GetMask(tag));

        if (col == null)
            return null;

        // 如果使用 tag 字符串判断而不是 Layer
        if (col.tag != tag)
            return null;

        return col.GetComponent<Entity>();
    }
    // 2D射线检测（带命中信息）
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, string layerMask, out RaycastHit2D hitInfo)
    {
        hitInfo = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask(layerMask));
        return hitInfo.collider != null;
    }
    public static bool Raycast2DNoLayer(Vector2 origin, Vector2 direction, float distance)
    {
        return Physics2D.Raycast(origin, direction, distance);
    }
    // 多射线地面检测
    public static GroundCheckResult MultiRayGroundCheck
        (
        //Vector2 position,//检测的中心位置
        //float width, //检测的宽度范围（敌人的脚底宽度）
        Collider2D collider,//碰撞体（规则碰撞体，矩形，椭圆形，胶囊）
        int rayCount, //射线数量
        float distance, //射线长度
        string groundLayer//地面层级
        )
    {
        Vector2 position = collider.transform.position;//检测的中心位置
        float width = Tool.GetColliderWidth( collider );//检测的宽度范围（碰撞箱的脚底宽度）

        var result = new GroundCheckResult();
        int groundHits = 0;

        //Debug.Log("调用边缘检测函数");
        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount > 1 ? (float)i / (rayCount - 1) : 0.5f;
            float x = Mathf.Lerp(-width / 2, width / 2, t);//线性插值函数
            Vector2 rayOrigin = position + new Vector2(x, 0);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, distance, LayerMask.GetMask(groundLayer));

            Draw(hit.collider != null, rayOrigin, Vector2.down , distance);

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


    private static void Draw(bool ishit,Vector2 position, Vector2 direction, float rayLength)
    {
       
        //射线绘制方便调试
        if (ishit)
        {
            
            Debug.DrawRay(position, direction * rayLength, Color.green); 
        }
        else
        {
            Debug.DrawRay(position, direction * rayLength, Color.red);
        }
    }
   

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