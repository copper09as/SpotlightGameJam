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

    readonly private string attackBeforeAudio = "Assets/Audio/Sfx/Dragoncharge.wav";

    readonly private List<string> wingAudios= new List<string>()
    {
        "Assets/Audio/Sfx/wing01.wav",
        "Assets/Audio/Sfx/wing02.wav",
        "Assets/Audio/Sfx/wing03.wav",
        "Assets/Audio/Sfx/wing04.wav",
    }; 
    public void FlyAudio()//动画事件，龙飞行的时候播放扇翅膀的声音
    {
        AudioManager.Instance.PlaySFX(wingAudios[UnityEngine.Random.Range(0, wingAudios.Count)]);
    }


    public void AttackBeforeAudio()//动画事件，龙攻击前摇的时候播放蓄力的声音
    {
        AudioManager.Instance.PlaySFX(attackBeforeAudio);
    }

    public void DeadAudio()//动画事件，龙死亡时的声音
    {
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/DragonDead.wav");
    }


}
