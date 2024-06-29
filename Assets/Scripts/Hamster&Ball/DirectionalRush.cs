using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionalRush : MonoBehaviour
{
    // 玩家的位置
    private Transform playerTransform;

    // 当前的鼠标（如果有）
    private Mouse curMouse;

    // 为鼠标操作时的起始点和当前点
    private Vector3 mousePosInit;
    private Vector3 mousePosCur;

    // 当前的手柄（如果有）
    private Joystick curJoystick;

    // 方向向量
    private Vector3 dirVector;

    // 方向线条的起点和终点
    private Vector3 dirBeginPos;
    private Vector3 dirEndPos;

    // 当前正在瞄准中
    private bool isAiming;

    private LineRenderer dirRenderer;
    private Camera mainCamera;

    void Start()
    {
        playerTransform = transform.parent;
        dirRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        Initialize();
    }

    private void Initialize()
    {
        isAiming = false;
    }

    void Update()
    {
        DirUpdate();
    }

    private void DirUpdate()
    {
        curMouse = Mouse.current;
        if (curMouse != null)
        {
            if (curMouse.leftButton.wasPressedThisFrame)
            {
                isAiming = true;
                GetMousePosition(out mousePosInit);
            }
            if (curMouse.leftButton.wasReleasedThisFrame)
            {
                isAiming = false;
            }
        }
        if (isAiming)
        {
            dirRenderer.enabled = true;
            dirBeginPos = playerTransform.position;
            GetMousePosition(out mousePosCur);
            dirVector = mousePosCur - mousePosInit;
            dirEndPos = dirBeginPos - dirVector;

            dirRenderer.SetPosition(0, dirBeginPos);
            dirRenderer.SetPosition(1, dirEndPos);
        }
        else
        {
            dirRenderer.enabled = false;
        }
    }

    private void GetMousePosition(out Vector3 pos)
    {
        pos = curMouse.position.ReadValue();
        pos = mainCamera.ScreenToWorldPoint(pos);
        pos.z = 0;
    }

    internal void OnControlsUpdate(PlayerInput input)
    {
        Debug.Log("Controls update to " + input.currentControlScheme);
    }
}
