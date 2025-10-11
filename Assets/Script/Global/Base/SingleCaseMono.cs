using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// �̳���MonoBehaviour�ĵ���ģʽ����
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleCaseMono<T>: MonoBehaviour where T: MonoBehaviour
{
    static private T instanse;

    public static T Instance
    {
        get
        {
            if (instanse == null)
            {
                //��̬���� ��̬����
                //�ڳ����ϴ���һ��������
                GameObject obj = new GameObject(typeof(T).Name);
                //��̬���� ����ģʽ�ű�
                obj.AddComponent<T>();

                //������ʱ���Ƴ����� ��֤�����������������д���
                DontDestroyOnLoad(obj);
            }
            return instanse;
        }
    }

    protected virtual void Awake()
    {
        instanse = this as T;
    }

}
