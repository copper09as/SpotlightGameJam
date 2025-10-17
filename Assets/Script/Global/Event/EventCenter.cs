using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
[LuaCallCSharp]
public class EventCenter : SingleCaseMono<EventCenter>
{

    public Dictionary<string,Action> Actions = new Dictionary<string,Action>();

    public void AddAction(string name, Action action)
    {
        if (Actions.ContainsKey(name))
        {
            Actions[name] += action;
        }
        else
        {
            Actions[name] = action;
            Debug.Log($"注册{name}事件");
        }
    }

    public void RemoveAction(string name, Action action = null)
    {
        if (Actions.ContainsKey(name))
        {
            if (action != null)
            {
                Actions[name] -= action;
                // 如果删除后委托为空，移除整个键
                if (Actions[name] == null)
                {
                    Actions.Remove(name);
                }
            }
            else
            {
                Actions.Remove(name);
            }
        }
    }

    public void ClearActions() => Actions.Clear();

    public void SafeTrigger(string eventName)//事件的广播
    {
        Actions[eventName]?.Invoke();
    }
}
