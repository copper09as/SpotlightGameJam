using DG.Tweening;
using Game.Battle.Entity;
using Global.Data;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using XLua;
using static UnityEngine.EventSystems.EventTrigger;

public static class XLuaCustomExport
{
    //在 LuaCallCSharp 列表中添加 DOTween 相关类型
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(DG.Tweening.DOTween),
        typeof(DG.Tweening.Tweener),
        typeof(DG.Tweening.Sequence),
        typeof(DG.Tweening.Tween),
        typeof(DG.Tweening.TweenExtensions),
        typeof(DG.Tweening.TweenSettingsExtensions),
        typeof(DG.Tweening.ShortcutExtensions),
        typeof(DG.Tweening.Core.DOGetter<>),
        typeof(DG.Tweening.Core.DOSetter<>),
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.LogBehaviour),
        // 枚举类型
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.LogBehaviour),
        typeof(DG.Tweening.AutoPlay),
        typeof(DG.Tweening.AxisConstraint),
        typeof(DG.Tweening.RotateMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.TweenType),
        typeof(DG.Tweening.UpdateType),

        // 添加 DG.Tweening 相关类型
        typeof(DG.Tweening.Core.DOGetter<UnityEngine.Vector2>),
        typeof(DG.Tweening.Core.DOSetter<UnityEngine.Vector2>),
        typeof(DG.Tweening.TweenCallback),
        
        // 如果需要其他类型，也可以添加
        typeof(DG.Tweening.Core.DOGetter<UnityEngine.Vector3>),
        typeof(DG.Tweening.Core.DOSetter<UnityEngine.Vector3>),
        typeof(DG.Tweening.Core.DOGetter<float>),
        typeof(DG.Tweening.Core.DOSetter<float>),
        
        // Unity 组件类型（扩展方法的目标）
        typeof(UnityEngine.Transform),
        typeof(UnityEngine.Rigidbody),
        typeof(UnityEngine.Rigidbody2D),
        typeof(UnityEngine.Material),
        typeof(UnityEngine.Camera),
        typeof(UnityEngine.Light),
        typeof(UnityEngine.AudioSource),
        
        // UI 组件类型
        typeof(UnityEngine.UI.Image),
        typeof(UnityEngine.UI.Text),
        typeof(UnityEngine.UI.Graphic),
        typeof(UnityEngine.CanvasGroup),
        typeof(UnityEngine.RectTransform),
    };
    [LuaCallCSharp]
    public static TMP_Text GetTMPText(Entity entity)
    {
        return entity.GetComponentInChildren<TMP_Text>(true);
    }
    
    [LuaCallCSharp]
    public static void ChangeColor(SpriteRenderer sr, Color color, float time,Action actionback)
    {
        sr.DOColor(color, time).OnComplete(

           () => { actionback?.Invoke(); }
        );
           
    }
    
}