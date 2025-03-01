using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class TimeManager : SingletonMono<TimeManager>
{
    private static readonly float DefaultTimeScale = 1.0f;
    private static readonly float SlowedDownTimeScale = 0.2f;
    private static readonly float PauseTimeScale = 0;

    void Start()
    {
        Time.timeScale = DefaultTimeScale;
    }

    // 启动时缓
    public void ExecuteTimeSlowDown()
    {
        Time.timeScale = SlowedDownTimeScale;
        TypeEventSystem.Global.Send<TimeScaleSlowDownEvent>();
    }

    // 结束时缓
    public void RecoverTimeScale()
    {
        if(Time.timeScale == PauseTimeScale) return;
        Time.timeScale = DefaultTimeScale;
        TypeEventSystem.Global.Send<TimeScaleRecoverEvent>();
    }

    public void ExecutePause()
    {
        Time.timeScale = PauseTimeScale;
    }

    public void ExecuteContinue()
    {
        Time.timeScale = DefaultTimeScale;
    }
}
