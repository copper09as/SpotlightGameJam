using DG.Tweening;
using Game.Battle.Entity;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using XLua;
[LuaCallCSharp]
public static class XLuaCustomExport
{
    //在 LuaCallCSharp 列表中添加 DOTween 相关类型
    public static List<Type> LuaCallCSharp = new List<Type>()
    {
        typeof(DG.Tweening.DOTween),
        typeof(DG.Tweening.Tweener),
        typeof(DG.Tweening.Sequence),
        typeof(DG.Tweening.Tween),
        typeof(DG.Tweening.TweenExtensions),
        typeof(DG.Tweening.TweenSettingsExtensions),
        typeof(DG.Tweening.ShortcutExtensions),
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.LogBehaviour)
    };
    public static TMP_Text GetTMPText(Entity entity)
    {
        return entity.GetComponentInChildren<TMP_Text>(true);
    }

    public static void TypeWriter(TMP_Text _Text,string words,float time)
    {
        DOTween.To(() => string.Empty,
            currentText => _Text.text = currentText,
             words,
             time);
    }
}