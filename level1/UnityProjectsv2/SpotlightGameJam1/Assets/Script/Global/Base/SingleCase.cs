using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例模式基类 （不继承monobehavior）
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
                //利用反射获取无参私有的构造函数 用于对象的实例化
                Type type = typeof(T);
                var info = type.GetConstructor( BindingFlags.Instance | BindingFlags.NonPublic,//私有方法
                                     null,//没有绑定对象
                                     Type.EmptyTypes,//没有参数
                                     null);//没有参数修饰符
                if (info != null)
                    instanse = info.Invoke(null) as T;
                else
                    Debug.LogError($"没有获取到{type.Name}的无参私有构造函数");
            }
            return instanse;
        }
    }


}
