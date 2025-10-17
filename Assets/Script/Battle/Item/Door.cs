using Game.Battle.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Entity pedal;
    private Entity entity;
    // Start is called before the first frame update
    void Awake()
    {
            entity = GetComponent<Entity>();
    }
    private void Start()
    {
        //if (pedal == null || pedal.entityId != 10011)//������ʵ��Ϊ�ջ��߲���̤��
        //    Debug.LogError("������û��ƥ��̤��");
    }
    public void AddOpenTheDoor(Action action)
    {
        EventCenter.Instance.AddAction(pedal.name + "OpenTheDoor", action);
    }
}
