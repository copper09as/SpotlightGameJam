using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����monoģ��
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
    /// ��ӷ���������monoģ��update������
    /// </summary>
    /// <param name="action"></param>
    public void AddUpdateAction(Action action) =>  UpdateAction += action;
    /// <summary>
    /// �ڹ���monoģ��update������ɾ���÷���
    /// </summary>
    /// <param name="action"></param>
    public void DeleteUpdateAction(Action action) => UpdateAction -= action;

    // <summary>
    /// ��ӷ���������monoģ��Fixedupdate������
    /// </summary>
    /// <param name="action"></param>
    public void AddFixedUpdateAction(Action action) => FixedUpdateAction += action;

    /// <summary>
    /// �ڹ���monoģ��Fixedupdate������ɾ���÷���
    /// </summary>
    /// <param name="action"></param>
    public void DeleteFixedUpdateAction(Action action) => FixedUpdateAction -= action;
    // <summary>
    /// ��ӷ���������monoģ��Lateupdate������
    /// </summary>
    /// <param name="action"></param>
    public void AddLateFixedUpdateAction(Action action) => LateUpdateAction += action;
    /// <summary>
    /// �ڹ���monoģ��Lateupdate������ɾ���÷���
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
