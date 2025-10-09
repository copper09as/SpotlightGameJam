using System;
using System.Collections.Generic;
using System.Diagnostics;
using XLua;
[LuaCallCSharp]
public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();
        _subscribers[type].Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
            _subscribers[type].Remove(handler);
    }

    public static void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            foreach (var handler in _subscribers[type])
                ((Action<T>)handler)?.Invoke(evt);
        }
        else
            UnityEngine.Debug.Log("未找到委托对象"+typeof(T));
    }
    public static void Clear()
    {
        _subscribers.Clear();
    }
}
