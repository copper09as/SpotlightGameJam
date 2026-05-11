using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 继承了MonoBehaviour的单例模式基类
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
                //动态创建 动态挂载
                //在场景上创建一个空物体
                GameObject obj = new GameObject(typeof(T).Name);
                //动态挂载 单例模式脚本
                obj.AddComponent<T>();

                //过场景时候不移除对象 保证它在整个生命周期中存在
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
