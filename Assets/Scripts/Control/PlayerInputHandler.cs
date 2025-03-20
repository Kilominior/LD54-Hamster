using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler: MonoBehaviour
{
    private MouseController mc;
    private DirectionalRush dr;

    private PlayerInput pi;
    private InputActionMap ballAM;
    private InputActionMap hamsterAM;
    private InputActionMap UIAM;

    private void Start()
    {
        pi = GetComponent<PlayerInput>();
        mc = GetComponent<MouseController>();
        dr = transform.Find("RushDir").GetComponent<DirectionalRush>();

        ControlManager.InitControlScheme(pi);
        ActionBinding();
    }
    private void OnDestroy()
    {
        ActionUnbinding();
    }

    private void ActionBinding()
    {
        ballAM = pi.actions.actionMaps[0];
        hamsterAM = pi.actions.actionMaps[1];
        UIAM = pi.actions.actionMaps[2];

        pi.onControlsChanged += OnControlsUpdate;

        ballAM["Move"].performed += OnMovePerformed;
        ballAM["Move"].canceled += OnMoveCanceled;
        ballAM["Jump"].performed += OnJumpPerformed;
        ballAM["Interact"].performed += OnInteractPerformed;
        ballAM["SubInteract"].performed += OnSubInteractPerformed;
        ballAM["AimTrigger"].performed += OnAimTriggerPerformed;
        ballAM["AimTrigger"].canceled += OnAimTriggerCanceled;
        ballAM["Aim"].performed += OnAimPerformed;
        ballAM["Pause"].performed += OnPausePerformed;

        hamsterAM["Move"].performed += OnMovePerformed;
        hamsterAM["Move"].canceled += OnMoveCanceled;
        hamsterAM["Jump"].performed += OnJumpPerformed;
        hamsterAM["Interact"].performed += OnInteractPerformed;
        hamsterAM["Pause"].performed += OnPausePerformed;

        UIAM["Pause"].performed += OnPausePerformed;
    }

    private void ActionUnbinding()
    {
        pi.onControlsChanged -= OnControlsUpdate;

        ballAM["Move"].performed -= OnMovePerformed;
        ballAM["Move"].canceled -= OnMoveCanceled;
        ballAM["Jump"].performed -= OnJumpPerformed;
        ballAM["Interact"].performed -= OnInteractPerformed;
        ballAM["SubInteract"].performed -= OnSubInteractPerformed;
        ballAM["AimTrigger"].performed -= OnAimTriggerPerformed;
        ballAM["AimTrigger"].canceled -= OnAimTriggerCanceled;
        ballAM["Aim"].performed -= OnAimPerformed;
        ballAM["Pause"].performed -= OnPausePerformed;

        hamsterAM["Move"].performed -= OnMovePerformed;
        hamsterAM["Move"].canceled -= OnMoveCanceled;
        hamsterAM["Jump"].performed -= OnJumpPerformed;
        hamsterAM["Interact"].performed -= OnInteractPerformed;
        hamsterAM["Pause"].performed -= OnPausePerformed;

        UIAM["Pause"].performed -= OnPausePerformed;
    }


// 对内使用的Input System操作绑定***********************************************************************************************************
// 全局控制相关
    private void OnControlsUpdate(PlayerInput pi)
    {
        ControlManager.OnControlsUpdate(pi);
    }

// 移动相关
    private void OnMovePerformed(CallbackContext context)
    {
        OnMovePerformed(context.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(CallbackContext context)
    {
        OnMoveCanceled();
    }

    private void OnJumpPerformed(CallbackContext context)
    {
        OnJumpPerformed();
    }

    private void OnInteractPerformed(CallbackContext context)
    {
        OnInteractPerformed();
    }

    private void OnSubInteractPerformed(CallbackContext context)
    {
        OnSubInteractPerformed();
    }

    private void OnPausePerformed(CallbackContext context)
    {
        OnPausePerformed();
    }


// 瞄准相关
    private void OnAimPerformed(CallbackContext context)
    {
        var vector2 = context.ReadValue<Vector2>();
        if (context.control.device is Mouse)
        {
            OnAimPerformedByDelta(vector2);
        }
        else
        {
            OnAimPerformedByPushAmount(vector2);
        }
    }

    private void OnAimTriggerPerformed(CallbackContext context)
    {
        OnAimTriggerPerformed();
    }

    private void OnAimTriggerCanceled(CallbackContext context)
    {
        OnAimTriggerCanceled();
    }


// 向外暴露的控制方法***********************************************************************************************************
// 移动相关
    public void OnMovePerformed(Vector2 delta)
    {
        mc.OnMove(delta);
    }

    public void OnMoveCanceled()
    {
        mc.OnMoveStop();
    }

    public void OnJumpPerformed()
    {
        mc.OnJump();
    }

    public void OnInteractPerformed()
    {
        mc.OnInteract();
    }

    public void OnSubInteractPerformed()
    {
        mc.OnSubInteract();
    }

    public void OnPausePerformed()
    {
        TypeEventSystem.Global.Send<GamePauseTriggeredEvent>();
    }


// 瞄准相关
    public void OnAimPerformedByDelta(Vector2 vector2)
    {
        dr.OnAimByDelta(vector2);
    }

    public void OnAimPerformedByPushAmount(Vector2 vector2)
    {
        dr.OnAimByPushAmount(vector2);
    }

    public void OnAimTriggerPerformed()
    {
        dr.OnAimStart();
    }

    public void OnAimTriggerCanceled()
    {
        dr.OnAimEnd();
    }
}
