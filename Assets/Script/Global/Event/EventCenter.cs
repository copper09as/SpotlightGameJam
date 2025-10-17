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
            Debug.Log($"ע��{name}�¼�");
        }
    }

    public void RemoveAction(string name, Action action = null)
    {
        if (Actions.ContainsKey(name))
        {
            if (action != null)
            {
                Actions[name] -= action;
                // ���ɾ����ί��Ϊ�գ��Ƴ�������
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

    public void SafeTrigger(string eventName)//�¼��Ĺ㲥
    {
        Actions[eventName]?.Invoke();
    }
}
