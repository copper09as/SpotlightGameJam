using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 公共mono模块
/// </summary>
public class MonoManager : SingleCaseMono<MonoManager>
{
    private Action UpdateAction;
    private Action FixedUpdateAction;
    private Action LateUpdateAction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 添加方法到公共mono模块update函数中
    /// </summary>
    /// <param name="action"></param>
    public void AddUpdateAction(Action action) =>  UpdateAction += action;
    /// <summary>
    /// 在公共mono模块update函数中删除该方法
    /// </summary>
    /// <param name="action"></param>
    public void DeleteUpdateAction(Action action) => UpdateAction -= action;

    // <summary>
    /// 添加方法到公共mono模块Fixedupdate函数中
    /// </summary>
    /// <param name="action"></param>
    public void AddFixedUpdateAction(Action action) => FixedUpdateAction += action;

    /// <summary>
    /// 在公共mono模块Fixedupdate函数中删除该方法
    /// </summary>
    /// <param name="action"></param>
    public void DeleteFixedUpdateAction(Action action) => FixedUpdateAction -= action;
    // <summary>
    /// 添加方法到公共mono模块Lateupdate函数中
    /// </summary>
    /// <param name="action"></param>
    public void AddLateFixedUpdateAction(Action action) => LateUpdateAction += action;
    /// <summary>
    /// 在公共mono模块Lateupdate函数中删除该方法
    /// </summary>
    /// <param name="action"></param>
    public void DeleteLateFixedUpdateAction(Action action) => LateUpdateAction -= action;

    // Update is called once per frame
    void Update()
    {
        UpdateAction?.Invoke();
    }
    private void FixedUpdate()
    {
        FixedUpdateAction?.Invoke();
    }
    private void LateUpdate()
    {
        LateUpdateAction?.Invoke();
    }
}
