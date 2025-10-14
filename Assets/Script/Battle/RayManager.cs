using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayManager
{
    /// <summary>
    /// 发射2D射线检测
    /// </summary>
    /// <param name="origin">射线起点</param>
    /// <param name="direction">射线方向</param>
    /// <param name="distance">射线长度</param>
    /// <param name="layerMask">要检测的层）</param>
    /// <returns>是否命中</returns>
    public static bool RayHit2D(Vector2 origin, Vector2 direction, float distance,string layerName)
    {
        return Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask(layerName));
    }
}
