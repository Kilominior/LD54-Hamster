using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCheck : MonoBehaviour
{
    public delegate void PressurePlateEvent();
    public event PressurePlateEvent OnPlatePressed;
    public event PressurePlateEvent OnPlateReleased;
    private bool isPressed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 当有物体进入压力机关时触发事件
        if (!isPressed)
        {
            isPressed = true;

            // 触发按下事件
            if (OnPlatePressed != null)
            {
                OnPlatePressed();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 当物体离开压力机关时触发事件
        if (isPressed)
        {
            isPressed = false;

            // 触发释放事件
            if (OnPlateReleased != null)
            {
                OnPlateReleased();
            }
        }
    }
}
