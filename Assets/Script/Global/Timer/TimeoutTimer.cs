using System;
using UnityEngine;

public class TimeoutTimer : MonoBehaviour
{
    private float duration;         // ����ʱʱ��
    private float currentTime;      // ��ǰ��ʱ
    private bool isRunning;         // �Ƿ�������
    private Action onTimeout;       // ��ʱ�ص�
    private bool loop;
    public Action<float> OnTick;

    /// <summary>
    /// ��ʼ����ʱ��
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
    /// ֹͣ��ʱ
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// ���ü�ʱ
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
    /// ��ȡʣ��ʱ��
    /// </summary>
    public float GetRemainingTime()
    {
        return Mathf.Max(duration - currentTime, 0f);
    }

    /// <summary>
    /// ��ȡ���ȣ�0~1��
    /// </summary>
    public float GetProgress()
    {
        return Mathf.Clamp01(currentTime / duration);
    }
}
