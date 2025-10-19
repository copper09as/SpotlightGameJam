using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

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
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.LogBehaviour)
    };
}