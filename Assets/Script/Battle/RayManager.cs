using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayManager : MonoBehaviour
{
    /// <summary>
    /// ����2D���߼��
    /// </summary>
    /// <param name="origin">�������</param>
    /// <param name="direction">���߷���</param>
    /// <param name="distance">���߳���</param>
    /// <param name="layerMask">Ҫ���Ĳ㣩</param>
    /// <returns>�Ƿ�����</returns>
    public static bool RayHit2D(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask)
    {
        return Physics2D.Raycast(origin, direction, distance, layerMask);
    }
    public static bool RayHit2D(Vector2 origin, Vector2 direction, float distance)
    {
        return Physics2D.Raycast(origin, direction, distance);
    }
}
