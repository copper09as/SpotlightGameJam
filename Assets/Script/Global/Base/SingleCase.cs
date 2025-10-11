using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����ģʽ���� �����̳�monobehavior��
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingleCase<T> where T : class,new()
{
    static private T instanse;

    public static T Instance
    {
        get {
            if(instanse==null)
            { 
                //���÷����ȡ�޲�˽�еĹ��캯�� ���ڶ����ʵ����
                Type type = typeof(T);
                var info = type.GetConstructor( BindingFlags.Instance | BindingFlags.NonPublic,//˽�з���
                                     null,//û�а󶨶���
                                     Type.EmptyTypes,//û�в���
                                     null);//û�в������η�
                if (info != null)
                    instanse = info.Invoke(null) as T;
                else
                    Debug.LogError($"û�л�ȡ��{type.Name}���޲�˽�й��캯��");
            }
            return instanse;
        }
    }


}
