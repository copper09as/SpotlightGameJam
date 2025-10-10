using System;
using System.Collections.Generic;
using Global.Data;
using Global.ObjectCreate;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimerCenter : MonoBehaviour
{
    public static TimerCenter Instance { get; private set; }

    private List<TimeoutTimer> timers = new List<TimeoutTimer>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public TimeoutTimer CreateTimer(float duration, Action onTimeout, Action<float> onTick = null,bool loop = false)
    {
        GameObject go = 
            ObjectPool.Instance.Get
            (StringResource.TimeoutTimerPath,StringResource.TimeoutTimerName,transform);
        TimeoutTimer timer = go.GetComponent<TimeoutTimer>();
        timer.OnTick = onTick;
        timer.StartTimer(duration, onTimeout,loop);
        timers.Add(timer);
        return timer;
    }

    public void RemoveTimer(TimeoutTimer timer)
    {
        if (timers.Contains(timer))
        {
            timers.Remove(timer);
            ObjectPool.Instance.Release(timer.gameObject);
        }
    }
    public void ClearAllTimers()
    {
        foreach (var timer in timers)
            ObjectPool.Instance.Release(timer.gameObject);
        timers.Clear();
    }
}
