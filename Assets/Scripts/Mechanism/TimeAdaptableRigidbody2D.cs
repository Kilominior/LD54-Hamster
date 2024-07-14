using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 允许刚体在时缓时自动做出插值，并在结束后自动恢复离散检测.
/// </summary>
public class TimeAdaptableRigidbody2D : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.None;
        TypeEventSystem.Global.Register<TimeScaleSlowDownEvent>(OnTimeSlowDown).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<TimeScaleRecoverEvent>(OnTimeRecover).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnTimeRecover(TimeScaleRecoverEvent @event)
    {
        rb.interpolation = RigidbodyInterpolation2D.None;
    }

    private void OnTimeSlowDown(TimeScaleSlowDownEvent @event)
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }
}
