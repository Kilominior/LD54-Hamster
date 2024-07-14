using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using QFramework;
using static UnityEngine.InputSystem.InputAction;
using TMPro;

public class DirectionalRush : MonoBehaviour
{
    private static readonly string joystickSchemeName = "Joystick";
    private static readonly string keyboardSchemeName = "Keyboard&Mouse";

    private static readonly float DefaultTimeScale = 1.0f;
    private static readonly float SlowedDownTimeScale = 0.2f;

    // 玩家
    private MouseController player;


    // 冲撞是否正在冷却
    public bool isRushCooling;
    // 冲撞的冷却时间
    [SerializeField]
    private float rushCD = 0.5f;
    // 冲撞冷却倒计时
    private WaitForSeconds WaitForRushCD;

    // 当前的鼠标（如果有）
    private Mouse curMouse;

    // 为鼠标操作时的起始点和当前点
    private Vector3 mousePosInit;
    private Vector3 mousePosCur;

    // 当前的手柄（如果有）
    private Joystick curJoystick;

    // 方向向量
    private Vector3 _dirVector;
    private Vector3 dirVector
    {
        get { return _dirVector; }
        set
        {
            _dirVector = value;
            if (_dirVector.magnitude > dirLength)
            {
                _dirVector = _dirVector.normalized * dirLength;
            }
        }
    }

    // 每帧方向的变化量
    private Vector3 deltaDirVector;

    // 方向线条的最大长度
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
        WaitForRushCD = new WaitForSeconds(rushCD);

        Initialize();
    }

    private void Initialize()
    {
        EndAiming(true);
        isRushCooling = false;
    }

    void Update()
    {
        AimUpdate();
    }

    // 开始瞄准
    private void StartAiming()
    {
        if (isRushCooling) { return; }
        isAiming = true;
        dirRenderer.enabled = true;
        //GetMousePosition(out mousePosInit);
        dirVector = Vector2.zero;
        deltaDirVector = Vector2.zero;
        ExecuteTimeSlowDown();
    }

    // 结束瞄准
    private void EndAiming(bool canceled = false)
    {
        isAiming = false;
        RecoverTimeScale();
        dirRenderer.enabled = false;

        if (canceled) return;
        ExecuteRush();
    }

    private void ExecuteRush()
    {
        // 若长度过短判定为不冲撞
        if (dirVector.magnitude <= 0.01f) return;
        // 进行冲撞并启动冷却
        isRushCooling = true;
        player.Rush(dirVector / dirLength);
        dirVector = Vector3.zero;
        StartCoroutine(nameof(rushCoolDown));
    }

    private IEnumerator rushCoolDown()
    {
        yield return WaitForRushCD;
        isRushCooling = false;
    }

    //// 根据瞄准情况更新冲撞方向以及LineRenderer
    //private void AimUpdate()
    //{
    //    if (isAiming)
    //    {
    //        dirRenderer.enabled = true;
    //        dirBeginPos = player.transform.position;
    //        GetMousePosition(out mousePosCur);
    //        dirVector = mousePosInit - mousePosCur;
    //        if(dirVector.magnitude > dirLength)
    //        {
    //            dirVector = dirVector.normalized * dirLength;
    //        }
    //        dirEndPos = dirBeginPos + dirVector;

    //        dirRenderer.SetPosition(0, dirBeginPos);
    //        dirRenderer.SetPosition(1, dirEndPos);
    //    }
    //    else
    //    {
    //        dirRenderer.enabled = false;
    //    }
    //}

    private void AimUpdate()
    {
        if (!isAiming) return;
        // 更新当前瞄准线的起点和终点
        dirBeginPos = player.transform.position;
        dirEndPos = dirBeginPos + dirVector;

        dirRenderer.SetPosition(0, dirBeginPos);
        dirRenderer.SetPosition(1, dirEndPos);
    }

    public void OnAimPerformed(CallbackContext context)
    {
        if (!isAiming) return;
        // 计算并更新当前的方向向量
        deltaDirVector = context.ReadValue<Vector2>();
        dirVector += deltaDirVector / 5.0f;
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

    public void OnAimTriggerPerformed(CallbackContext context)
    {
        StartAiming();
    }

    public void OnAimTriggerCanceled(CallbackContext context)
    {
        EndAiming();
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
