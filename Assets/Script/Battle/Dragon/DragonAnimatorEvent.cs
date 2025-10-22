using DG.Tweening;
using Game.Battle.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class DragonAnimatorEvent : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Animator animator;


    public void Fly()//动画事件
    {
        animator.SetTrigger("Fly");
    }

    public void StopAttack()//动画事件
    {
        animator.SetTrigger("StopAttack");
    }

    public void DragonAttack(Vector3 position, float time)
    {
        entity.transform.DOMove(position, time).SetEase(Ease.OutCubic);
    }

    
}
