using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEngine;

public static class DetectionManager
{

    // 2D���߼��
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
        // ���ָ�����������ڵ���ײ��
        Collider2D col = Physics2D.OverlapBox(center, size, 0f, LayerMask.GetMask(tag));

        if (col == null)
            return null;

        // ���ʹ�� tag �ַ����ж϶����� Layer
        if (col.tag != tag)
            return null;

        return col.GetComponent<Entity>();
    }
    // 2D���߼�⣨��������Ϣ��
    public static bool Raycast2D(Vector2 origin, Vector2 direction, float distance, string layerMask, out RaycastHit2D hitInfo)
    {
        hitInfo = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask(layerMask));
        return hitInfo.collider != null;
    }
    public static bool Raycast2DNoLayer(Vector2 origin, Vector2 direction, float distance)
    {
        return Physics2D.Raycast(origin, direction, distance);
    }
    // �����ߵ�����
    public static GroundCheckResult MultiRayGroundCheck
        (
        //Vector2 position,//��������λ��
        //float width, //���Ŀ�ȷ�Χ�����˵Ľŵ׿�ȣ�
        Collider2D collider,//��ײ�壨������ײ�壬���Σ���Բ�Σ����ң�
        int rayCount, //��������
        float distance, //���߳���
        string groundLayer//����㼶
        )
    {
        Vector2 position = collider.transform.position;//��������λ��
        float width = Tool.GetColliderWidth( collider );//���Ŀ�ȷ�Χ����ײ��Ľŵ׿�ȣ�

        var result = new GroundCheckResult();
        int groundHits = 0;

        //Debug.Log("���ñ�Ե��⺯��");
        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount > 1 ? (float)i / (rayCount - 1) : 0.5f;
            float x = Mathf.Lerp(-width / 2, width / 2, t);//���Բ�ֵ����
            Vector2 rayOrigin = position + new Vector2(x, 0);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, distance, LayerMask.GetMask(groundLayer));

            Draw(hit.collider != null, rayOrigin, Vector2.down , distance);

            if (hit.collider != null)
            {
                groundHits++;
                result.groundNormal = hit.normal;

                // ��Ե���
                if (i == 0) result.isLeftGrounded = true;
                if (i == rayCount - 1) result.isRightGrounded = true;
            }
        }

        result.isGrounded = groundHits > 0;
        result.isLeftEdge = !result.isLeftGrounded;
        result.isRightEdge = !result.isRightGrounded;

        return result;
    }

    // Բ�η�Χ���
    public static bool CircleCast(Vector2 center, float radius, LayerMask layerMask)
    {
        Collider2D hit = Physics2D.OverlapCircle(center, radius, layerMask);
        return hit != null;
    }

    //// ���߼�⣨�����ϰ��
    //public static bool LineOfSight(Vector2 from, Vector2 to, LayerMask obstacleLayer)
    //{
    //    Vector2 direction = to - from;
    //    float distance = direction.magnitude;

    //    RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, obstacleLayer);
    //    return hit.collider == null; // û���ϰ��ﷵ��true
    //}


    private static void Draw(bool ishit,Vector2 position, Vector2 direction, float rayLength)
    {
       
        //���߻��Ʒ������
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

// ���������ṹ
public struct GroundCheckResult
{
    public bool isGrounded;
    public bool isLeftEdge;//�Ƿ������Ե
    public bool isRightEdge;
    public bool isLeftGrounded;//����Ƿ��е���
    public bool isRightGrounded;
    public Vector2 groundNormal;//���淨����

    public bool IsOnAnyEdge => isLeftEdge || isRightEdge;//�Ƿ��ڱ�Ե
}