using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    public static float GetColliderWidth( BoxCollider2D collider )//获取这个碰撞箱世界空间宽度
    {
        return collider.size.x * collider.transform.lossyScale.x;
    }

}
