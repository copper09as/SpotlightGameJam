using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tool
{
    public static float GetColliderWidth( BoxCollider2D collider )//��ȡ�����ײ������ռ���
    {
        return collider.size.x * collider.transform.lossyScale.x;
    }

}
