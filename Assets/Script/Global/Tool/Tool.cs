using Global.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class Tool
{
    public static float GetColliderWidth(this Collider2D collider )//获取这个碰撞箱世界空间宽度
    {
        if (collider == null) return 0f;

        switch (collider)
        {
            case BoxCollider2D boxCollider:
                return Mathf.Abs(boxCollider.size.x * boxCollider.transform.lossyScale.x);

            case CircleCollider2D circleCollider:
                return Mathf.Abs(circleCollider.radius * 2f * circleCollider.transform.lossyScale.x);

            case CapsuleCollider2D capsuleCollider:
                // 胶囊碰撞器的宽度取决于方向
                return capsuleCollider.direction == CapsuleDirection2D.Horizontal ?
                       Mathf.Abs((capsuleCollider.size.x + capsuleCollider.size.y) * capsuleCollider.transform.lossyScale.x) :
                       Mathf.Abs(capsuleCollider.size.x * capsuleCollider.transform.lossyScale.x);
            default:
                // 对于其他未知类型的碰撞器，使用通用的边界框方法
                return Mathf.Abs(collider.bounds.size.x);
        }
    }
    public static float GetColliderWidthLua(Collider2D collider)//获取这个碰撞箱世界空间宽度
    {
        if (collider == null) return 0f;

        switch (collider)
        {
            case BoxCollider2D boxCollider:
                return Mathf.Abs(boxCollider.size.x * boxCollider.transform.lossyScale.x);

            case CircleCollider2D circleCollider:
                return Mathf.Abs(circleCollider.radius * 2f * circleCollider.transform.lossyScale.x);

            case CapsuleCollider2D capsuleCollider:
                // 胶囊碰撞器的宽度取决于方向
                return capsuleCollider.direction == CapsuleDirection2D.Horizontal ?
                       Mathf.Abs((capsuleCollider.size.x + capsuleCollider.size.y) * capsuleCollider.transform.lossyScale.x) :
                       Mathf.Abs(capsuleCollider.size.x * capsuleCollider.transform.lossyScale.x);
            default:
                // 对于其他未知类型的碰撞器，使用通用的边界框方法
                return Mathf.Abs(collider.bounds.size.x);
        }
    }
    public static float GetColliderHeight(this Collider2D collider)
    {
        if (collider == null) return 0f;

        switch (collider)
        {
            case BoxCollider2D boxCollider:
                return Mathf.Abs(boxCollider.size.y * boxCollider.transform.lossyScale.y);

            case CircleCollider2D circleCollider:
                return Mathf.Abs(circleCollider.radius * 2f * circleCollider.transform.lossyScale.y);

            case CapsuleCollider2D capsuleCollider:
                if (capsuleCollider.direction == CapsuleDirection2D.Vertical)
                    return Mathf.Abs((capsuleCollider.size.y + capsuleCollider.size.x) * capsuleCollider.transform.lossyScale.y);
                else
                    return Mathf.Abs(capsuleCollider.size.y * capsuleCollider.transform.lossyScale.y);

            default:
                return Mathf.Abs(collider.bounds.size.y);
        }
    }
    public static float GetColliderHeightLua(Collider2D collider)
    {
        if (collider == null) return 0f;

        switch (collider)
        {
            case BoxCollider2D boxCollider:
                return Mathf.Abs(boxCollider.size.y * boxCollider.transform.lossyScale.y);

            case CircleCollider2D circleCollider:
                return Mathf.Abs(circleCollider.radius * 2f * circleCollider.transform.lossyScale.y);

            case CapsuleCollider2D capsuleCollider:
                if (capsuleCollider.direction == CapsuleDirection2D.Vertical)
                    return Mathf.Abs((capsuleCollider.size.y + capsuleCollider.size.x) * capsuleCollider.transform.lossyScale.y);
                else
                    return Mathf.Abs(capsuleCollider.size.y * capsuleCollider.transform.lossyScale.y);

            default:
                return Mathf.Abs(collider.bounds.size.y);
        }
    }
    public static string GetLuaName(string path)
    {
        return Path.GetFileName(path);
    }

    public static Vector2 ToVector2( Vector3 vector )
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 ToVector3(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y,0);
    }

    public static string GetRandomWords()
    {
        return GameConfig.Instance.WordsD.words[UnityEngine.Random.Range(0, GameConfig.Instance.WordsD.words.Count)];
    }

}
