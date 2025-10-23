using Game.Battle.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XLua;
using static UnityEngine.UI.Image;

[LuaCallCSharp]
public static class DetectionManager
{

    // 2D射线检测
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, string layerMask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance,LayerMask.GetMask(layerMask));
        return hit.collider != null;
    }
    public static Entity Raycast2DByTag(Vector2 origin, Vector2 direction, float distance, string tag)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return hit.collider.GetComponent<Entity>();
            }
        }
        return null;
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
    public static bool Raycast2DOutHit(Vector2 origin, Vector2 direction, float distance, string layerMask, out RaycastHit2D hitInfo)
    {
        hitInfo = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask(layerMask));
        return hitInfo.collider != null;
    }
    
    public static bool Raycast2DNoLayer(Vector2 origin, Vector2 direction, float distance)
    {
        return Physics2D.Raycast(origin, direction, distance);
    }

    public static bool Raycast2DNoLayerOutHit(Vector2 origin, Vector2 direction, float distance, out RaycastHit2D hitInfo)
    {
        hitInfo = Physics2D.Raycast(origin, direction, distance);
        //Draw(hitInfo.collider != null, origin, direction, distance);
        /*if (hitInfo.collider != null)
        {
            // 确实碰撞到了物体
            Debug.Log("碰撞到: " + hitInfo.collider.gameObject.name);
        }*/


        return hitInfo.collider != null;
    }
    // 多射线地面检测
    public static GroundCheckResult MultiRayGroundCheck//只检测脚底
        (
        //Vector2 position,//检测的中心位置
        //float width, //检测的宽度范围（敌人的宽度）
        Collider2D collider,//碰撞体（规则碰撞体，矩形，椭圆形，胶囊）
        int rayCount, //脚底射线数量
        float distance, //射线长度(超出碰撞体的部分)
        string groundLayer//地面层级
        )
    {
        Vector2 position = collider.transform.position;//检测的中心位置
        float width = collider.GetColliderWidth();//检测的宽度范围（碰撞箱的宽度）
        float height = collider.GetColliderHeight();//检测的宽度范围（碰撞箱的高度）

        var result = new GroundCheckResult();
        int groundHits = 0;

        //Debug.Log("调用边缘检测函数");
        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount > 1 ? (float)i / (rayCount - 1) : 0.5f;
            float x = Mathf.Lerp(-width / 2, width / 2, t);//线性插值函数
            Vector2 rayOrigin = position + new Vector2(x, 0);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, (height/2) + distance, LayerMask.GetMask(groundLayer));


            Draw(hit.collider != null, rayOrigin, Vector2.down , (height / 2) + distance);

            if (hit.collider != null)
            {
                groundHits++;
                result.groundNormal = hit.normal;

                // 边缘检测
                if (i == 0) result.isLeftGrounded = true;
                if (i == rayCount - 1) result.isRightGrounded = true;
            }
        }

        //Draw(Raycast2DoutHit(position, Vector2.left, (width / 2) + distance, groundLayer,out RaycastHit2D hitleft),
        //    position, Vector2.left, (width / 2) + distance);//检测两边
        //Draw(Raycast2DoutHit(position, Vector2.right, (width / 2) + distance, groundLayer, out RaycastHit2D hitright),
        //    position, Vector2.right, (width / 2) + distance); ;

        result.isGrounded = groundHits > 0;
        result.isLeftEdge = !result.isLeftGrounded;
        result.isRightEdge = !result.isRightGrounded;
        return result;
    }

    // 圆形范围检测
    public static bool CircleCast(Vector2 center, float radius, string layerMask)
    {
        Collider2D hit = Physics2D.OverlapCircle(center, radius, LayerMask.GetMask(layerMask));
        return hit != null;
    }
    public static bool CircleCastOutHit(Vector2 center, float radius, string layerMask, out Collider2D hit)
    {
        hit = Physics2D.OverlapCircle(center, radius, LayerMask.GetMask(layerMask));
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


    public static void Draw(bool ishit,Vector2 position, Vector2 direction, float rayLength)
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