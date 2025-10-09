using System;
using UnityEngine;

public class TimeoutTimer : MonoBehaviour
{
    private float duration;         // 倒计时时长
    private float currentTime;      // 当前计时
    private bool isRunning;         // 是否在运行
    private Action onTimeout;       // 超时回调
    private bool loop;
    public Action<float> OnTick;

    /// <summary>
    /// 初始化计时器
    /// </summary>
    public void StartTimer(float time, Action onTimeoutAction,bool loop)
    {
        duration = time;
        currentTime = 0f;
        onTimeout = onTimeoutAction;
        isRunning = true;
        this.loop = loop;
    }

    /// <summary>
    /// 停止计时
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// 重置计时
    /// </summary>
    public void ResetTimer()
    {
        currentTime = 0f;
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime += Time.deltaTime;
        float time = GetRemainingTime();
        OnTick?.Invoke(time);
        if (currentTime >= duration)
        {
            if(loop)
            {
                currentTime = 0f;
            }
            else
            {
                isRunning = false;
            }
            
            onTimeout?.Invoke();
        }
    }

    /// <summary>
    /// 获取剩余时间
    /// </summary>
    public float GetRemainingTime()
    {
        return Mathf.Max(duration - currentTime, 0f);
    }

    /// <summary>
    /// 获取进度（0~1）
    /// </summary>
    public float GetProgress()
    {
        return Mathf.Clamp01(currentTime / duration);
    }
}
