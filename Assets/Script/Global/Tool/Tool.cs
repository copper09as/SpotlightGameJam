using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Tool
{
    public static float GetColliderWidth( Collider2D collider )//��ȡ�����ײ������ռ���
    {
        if (collider == null) return 0f;

        switch (collider)
        {
            case BoxCollider2D boxCollider:
                return boxCollider.size.x * boxCollider.transform.lossyScale.x;

            case CircleCollider2D circleCollider:
                return circleCollider.radius * 2f * circleCollider.transform.lossyScale.x;

            case CapsuleCollider2D capsuleCollider:
                // ������ײ���Ŀ��ȡ���ڷ���
                return capsuleCollider.direction == CapsuleDirection2D.Horizontal ?
                       capsuleCollider.size.y * capsuleCollider.transform.lossyScale.y :
                       capsuleCollider.size.x * capsuleCollider.transform.lossyScale.x;
            default:
                // ��������δ֪���͵���ײ����ʹ��ͨ�õı߽�򷽷�
                return collider.bounds.size.x;
        }
    }

    public static string GetLuaName(string path)
    {
        return Path.GetFileName(path);
    }
}
