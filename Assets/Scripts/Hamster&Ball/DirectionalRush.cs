using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using QFramework;

public class DirectionalRush : MonoBehaviour
{
    private static readonly string joystickSchemeName = "Joystick";
    private static readonly string keyboardSchemeName = "Keyboard&Mouse";

    private static readonly float DefaultTimeScale = 1.0f;
    private static readonly float SlowedDownTimeScale = 0.2f;

    // 玩家
    private MouseController player;

    // 当前的鼠标（如果有）
    private Mouse curMouse;

    // 为鼠标操作时的起始点和当前点
    private Vector3 mousePosInit;
    private Vector3 mousePosCur;

    // 当前的手柄（如果有）
    private Joystick curJoystick;

    // 方向向量
    private Vector3 dirVector;

    [SerializeField]
    private float dirLength = 3.0f;

    // 方向线条的起点和终点
    private Vector3 dirBeginPos;
    private Vector3 dirEndPos;

    // 当前正在瞄准中
    private bool isAiming;

    private LineRenderer dirRenderer;
    private Camera mainCamera;

    void Start()
    {
        player = transform.parent.GetComponent<MouseController>();
        dirRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        Initialize();
    }

    private void Initialize()
    {
        isAiming = false;
        RecoverTimeScale();
    }

    void Update()
    {
        CheckAim();
        AimUpdate();
    }

    // 检查是否存在瞄准的操作
    private void CheckAim()
    {
        // TODO: 目前仅支持鼠标，未来应引入更多输入方式
        // 可放弃现有方案，改用action，Performed和Canceled足以用于设置isAiming，
        // 且这样做不会和具体的某个输入方式强耦合
        if (curMouse != null)
        {
            if (curMouse.leftButton.wasPressedThisFrame)
            {
                StartAiming();
            }
            if (curMouse.leftButton.wasReleasedThisFrame)
            {
                EndAiming();
            }
        }
    }

    // 开始瞄准
    private void StartAiming()
    {
        isAiming = true;
        GetMousePosition(out mousePosInit);
        ExecuteTimeSlowDown();
    }

    // 结束瞄准
    private void EndAiming()
    {
        isAiming = false;
        RecoverTimeScale();

        // 进行冲撞
        player.Rush(dirVector / dirLength);
    }

    // 根据瞄准情况更新冲撞方向以及LineRenderer
    private void AimUpdate()
    {
        if (isAiming)
        {
            dirRenderer.enabled = true;
            dirBeginPos = player.transform.position;
            GetMousePosition(out mousePosCur);
            dirVector = mousePosInit - mousePosCur;
            if(dirVector.magnitude > dirLength)
            {
                dirVector = dirVector.normalized * dirLength;
            }
            dirEndPos = dirBeginPos + dirVector;

            dirRenderer.SetPosition(0, dirBeginPos);
            dirRenderer.SetPosition(1, dirEndPos);
        }
        else
        {
            dirRenderer.enabled = false;
        }
    }

    // 获取鼠标当前位置
    private void GetMousePosition(out Vector3 pos)
    {
        pos = curMouse.position.ReadValue();
        pos = mainCamera.ScreenToWorldPoint(pos);
        pos.z = 0;
    }

    // 启动时缓
    private void ExecuteTimeSlowDown()
    {
        Time.timeScale = SlowedDownTimeScale;
        TypeEventSystem.Global.Send<TimeScaleSlowDownEvent>();
    }

    // 结束时缓
    private void RecoverTimeScale()
    {
        Time.timeScale = DefaultTimeScale;
        TypeEventSystem.Global.Send<TimeScaleRecoverEvent>();
    }

    // 在当前控制器变化时调用，更新现在的操作设备
    public void OnControlsUpdate(PlayerInput input)
    {
        //Debug.Log("Controls update to " + input.currentControlScheme);
        if (input.currentControlScheme == keyboardSchemeName)
        {
            curMouse = input.GetDevice<Mouse>();
        }
        else if(input.currentControlScheme == joystickSchemeName)
        {
            curJoystick = input.GetDevice<Joystick>();
        }
    }
}
