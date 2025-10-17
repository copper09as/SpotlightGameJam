using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
[LuaCallCSharp]
public class EventCenter : SingleCaseMono<EventCenter>
{

    public Dictionary<string,Action> Actions = new Dictionary<string,Action>();

    public void AddAction(string name, Action action) => Actions[name] += action;
    
    public void RemoveAction(string name) => Actions.Remove(name); 

    public void ClearActions() => Actions.Clear();

    
}
