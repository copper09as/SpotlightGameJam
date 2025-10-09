using System;
using System.Collections.Generic;
using UnityEngine;

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
        GameObject go = new GameObject("TimeoutTimer");
        go.transform.parent = transform; // 方便 hierarchy 管理
        TimeoutTimer timer = go.AddComponent<TimeoutTimer>();
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
            Destroy(timer.gameObject);
        }
    }
    public void ClearAllTimers()
    {
        foreach (var timer in timers)
            Destroy(timer.gameObject);
        timers.Clear();
    }
}
