using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterSeconds : MonoBehaviour
{
    public float disableTime = 5f; // 设置禁用的时间，单位为秒
    private float timer = 0f;

    void Start()
    {
        // 在Start方法中开始计时
        timer = 0f;
    }

    void Update()
    {
        // 更新计时器
        timer += Time.deltaTime;

        // 检查是否达到禁用时间
        if (timer >= disableTime)
        {
            // 达到时间后禁用GameObject
            gameObject.SetActive(false);
        }
    }
}
