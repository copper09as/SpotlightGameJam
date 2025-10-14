using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayManager
{
    /// <summary>
    /// ����2D���߼��
    /// </summary>
    /// <param name="origin">�������</param>
    /// <param name="direction">���߷���</param>
    /// <param name="distance">���߳���</param>
    /// <param name="layerMask">Ҫ���Ĳ㣩</param>
    /// <returns>�Ƿ�����</returns>
    public static bool RayHit2D(Vector2 origin, Vector2 direction, float distance,string layerName)
    {
        return Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask(layerName));
    }
}
