using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Tool
{
    public static float GetColliderWidth( Collider2D collider )//获取这个碰撞箱世界空间宽度
    {
        if (collider == null) return 0f;

        switch (collider)
        {
            case BoxCollider2D boxCollider:
                return boxCollider.size.x * boxCollider.transform.lossyScale.x;

            case CircleCollider2D circleCollider:
                return circleCollider.radius * 2f * circleCollider.transform.lossyScale.x;

            case CapsuleCollider2D capsuleCollider:
                // 胶囊碰撞器的宽度取决于方向
                return capsuleCollider.direction == CapsuleDirection2D.Horizontal ?
                       capsuleCollider.size.y * capsuleCollider.transform.lossyScale.y :
                       capsuleCollider.size.x * capsuleCollider.transform.lossyScale.x;
            default:
                // 对于其他未知类型的碰撞器，使用通用的边界框方法
                return collider.bounds.size.x;
        }
    }

    public static string GetLuaName(string path)
    {
        return Path.GetFileName(path);
    }
}
