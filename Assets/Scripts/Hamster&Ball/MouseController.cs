using Spine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Spine.Unity;
using QFramework;

// 鼠/球状态
public enum PlayerState
{
    Ball,
    Hamster
}

public class MouseController : MonoBehaviour
{
    // 当前所在的状态
    public PlayerState currentState;

    // 当前持有的球壳（若有）
    public BallController ball;

    // 球壳预制体
    private static readonly string ballPrefabPath = "Prefab/Ball";
    private GameObject ballPrefab;

    // 鼠腿碰撞体，用于判定是否允许跳跃
    private MouseLegController mouseLeg;

    // 行动速度
    [SerializeField]
    private float moveSpeed = 10f;
    // 跳跃速度
    [SerializeField]
    private float JumpSpeed = 3.5f;
    // 冲撞速度
    [SerializeField]
    private float rushSpeed = 9.0f;

    // 移动的力
    private Vector2 moveForce;
    // 冲撞的力
    private Vector3 rushForce;
    // 跳跃的力
    private Vector3 JumpForce;
    // 当前计算出的将要施加给鼠鼠的总力
    private Vector3 force;

    // 输入系统
    private static readonly string ballAMName = "Player";
    private static readonly string hamsterAMName = "Hamster";
    private static readonly string UIAMName = "UI";
    private PlayerInput pi;
    private InputActionMap ballAM;
    private InputActionMap hamsterAM;
    private InputActionMap UIAM;

    // 因有Move输入而正在移动中
    private bool isMoving;

    // 当前可执行交互操作的对象
    private IInteractable interactObject;

    // 自身和球壳的刚体
    private Rigidbody2D rb;
    private Rigidbody2D brb;
    private SpriteRenderer sr;
    private SkeletonAnimation skeleton;
    private DirectionalRush dr;

    #region LifeSpan
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mouseLeg = transform.Find("PlaneCheck").GetComponent<MouseLegController>();
        skeleton = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        sr = GetComponent<SpriteRenderer>();
        dr = transform.Find("RushDir").GetComponent<DirectionalRush>();

        ActionBinding();
        StateUpdateTo(PlayerState.Ball);

        Initialize();
    }

    private void Initialize()
    {
        EventRegister();
    }

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<TimeScaleSlowDownEvent>(OnTimeSlowDown).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
        }
    }

    private void Update()
    {
        // 动画
        if (skeleton.AnimationState.GetCurrent(0).IsComplete)
        {
            skeleton.AnimationState.SetEmptyAnimation(1, 0.2f);
            if (GetComponent<SpriteRenderer>().flipX)
            {
                skeleton.AnimationState.SetAnimation(0, "idel_left", true);
            }
            else
            {
                skeleton.AnimationState.SetAnimation(0, "idel_right", true);
            }
        }
    }

    private void OnDestroy()
    {
        ActionUnbinding();
    }
    #endregion

    #region Input
    private void ActionBinding()
    {
        pi = GetComponent<PlayerInput>();

        ballAM = pi.actions.actionMaps[0];
        hamsterAM = pi.actions.actionMaps[1];
        UIAM = pi.actions.actionMaps[2];

        pi.onControlsChanged += OnControlsUpdate;
        ControlManager.Instance.InitControlScheme(pi);

        ballAM["Move"].performed += OnMovePerformed;
        ballAM["Move"].canceled += OnMoveCanceled;
        ballAM["Jump"].performed += OnJumpPerformed;
        ballAM["Interact"].performed += OnInteractPerformed;
        ballAM["SubInteract"].performed += OnSubInteractPerformed;
        ballAM["AimTrigger"].performed += dr.OnAimTriggerPerformed;
        ballAM["AimTrigger"].canceled += dr.OnAimTriggerCanceled;
        ballAM["Aim"].performed += dr.OnAimPerformed;
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
        ballAM["AimTrigger"].performed -= dr.OnAimTriggerPerformed;
        ballAM["AimTrigger"].canceled -= dr.OnAimTriggerCanceled;
        ballAM["Aim"].performed -= dr.OnAimPerformed;
        ballAM["Pause"].performed -= OnPausePerformed;

        hamsterAM["Move"].performed -= OnMovePerformed;
        hamsterAM["Move"].canceled -= OnMoveCanceled;
        hamsterAM["Jump"].performed -= OnJumpPerformed;
        hamsterAM["Interact"].performed -= OnInteractPerformed;
        hamsterAM["Pause"].performed -= OnPausePerformed;

        UIAM["Pause"].performed -= OnPausePerformed;
    }

    private void OnControlsUpdate(PlayerInput pi)
    {
        ControlManager.Instance.OnControlsUpdate(pi);
    }

    private void OnMovePerformed(CallbackContext context)
    {
        isMoving = true;
        moveForce = context.ReadValue<Vector2>();
        //Debug.Log("Is moving: " + moveForce);
    }

    private void OnMoveCanceled(CallbackContext context)
    {
        isMoving = false;
        //Debug.Log("Move canceled!");
    }

    private void OnJumpPerformed(CallbackContext context)
    {
        Jump();
        //Debug.Log("Jump performed!");
    }

    private void OnInteractPerformed(CallbackContext context)
    {
        ExecuteInteract();
        //Debug.Log("Interact performed!");
    }

    private void OnSubInteractPerformed(CallbackContext context)
    {
        ExecuteSubInteract();
        //Debug.Log("Sub Interact performed!");
    }

    private void OnPausePerformed(CallbackContext context)
    {
        TypeEventSystem.Global.Send<GamePauseTriggeredEvent>();
    }
    #endregion

    #region State & Interact
    /// <summary>
    /// 在鼠/球之间切换当前的状态
    /// </summary>
    public void StateSwitch()
    {
        if(currentState == PlayerState.Ball)
        {
            // 切换到鼠鼠状态
            StateUpdateTo(PlayerState.Hamster);
        }
        else
        {
            // 切换到鼠球状态
            StateUpdateTo(PlayerState.Ball);
        }
    }

    /// <summary>
    /// 将当前状态更新为目标状态
    /// </summary>
    /// <param name="targetState">目标状态</param>
    private void StateUpdateTo(PlayerState targetState)
    {
        currentState = targetState;

        switch(targetState)
        {
            case PlayerState.Ball:
                // 生成新的球，更新ActionMap为鼠球
                if (ballPrefab == null)
                {
                    ballPrefab = Resources.Load<GameObject>(ballPrefabPath);
                }
                ball = Instantiate(ballPrefab, transform.position,
                    Quaternion.identity).GetComponent<BallController>();
                brb = ball.GetComponent<Rigidbody2D>();
                pi.SwitchCurrentActionMap(ballAMName);
                break;
            case PlayerState.Hamster:
                // 销毁现有的球，更新ActionMap为鼠鼠
                if (ball != null) Destroy(ball.gameObject);
                pi.SwitchCurrentActionMap(hamsterAMName);
                break;
            default:
                break;
        }
    }

    private void ExecuteInteract()
    {
        if (interactObject == null) return;
        interactObject.ExecuteInteract(this);
    }

    private void ExecuteSubInteract()
    {
        if (interactObject == null) return;
        if (interactObject is ISubInteractable)
        {
            // 根据交互对象的情况决定是否允许再次交互
            (interactObject as ISubInteractable).ExecuteSubInteract(this);
        }
    }

    /// <summary>
    /// 打开UI时，更新ActionMap为UI
    /// </summary>
    public void SetActionMapToUI()
    {
        pi.SwitchCurrentActionMap(UIAMName);
    }

    /// <summary>
    /// 关闭UI后，恢复原有的控制
    /// </summary>
    public void RecoverActionMap()
    {
        switch (currentState)
        {
            case PlayerState.Ball:
                pi.SwitchCurrentActionMap(ballAMName);
                break;
            case PlayerState.Hamster:
                pi.SwitchCurrentActionMap(hamsterAMName);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Movement
    // 左右移动
    private void Move()
    {
        force = moveForce * moveSpeed;
        force.y = 0;
        rb.AddForce(force, ForceMode2D.Force);

        if (force != Vector3.zero)
        {
            // 动画
            if (skeleton.AnimationState.GetCurrent(1) == null
                || skeleton.AnimationState.GetCurrent(1).IsComplete)
                AniPlayX(force.x, "run_left", "run_right", 1, true);
        }
    }

    // 跳跃
    private void Jump()
    {
        // 保证在地面上起跳
        if (!mouseLeg.onGround) { return; }
        JumpForce = new(0, JumpSpeed);
        rb.AddForce(JumpForce, ForceMode2D.Impulse);
        AniPlayY("jump_left", "jump_right");
    }

    // 做出冲刺预备动作
    private void OnTimeSlowDown(TimeScaleSlowDownEvent @event)
    {
        AniPlayY("xuli_left", "xuli_right");
    }

    // 冲撞
    public void Rush(Vector2 dir)
    {
        rushForce = dir * rushSpeed;
        rb.AddForce(rushForce, ForceMode2D.Impulse);
        AniPlayX(rushForce.x, "strike_left", "strike_right");
    }
    #endregion

    #region Animation
    // X轴向动画播放方法
    private void AniPlayX(float forceX, string leftName, string rightName, int track = 1, bool loop = false)
    {
        if (forceX < 0)
        {
            if (!sr.flipX)
            {
                sr.flipX = true;
                //skeleton.AnimationState.SetAnimation(0, "turn", false);
            }
            skeleton.AnimationState.SetAnimation(track, leftName, loop);
        }
        else
        {
            if (sr.flipX)
            {
                sr.flipX = false;
                //skeleton.AnimationState.SetAnimation(0, "turn_back", false);
            }
            skeleton.AnimationState.SetAnimation(track, rightName, loop);
        }
    }

    // Y轴向动画播放方法
    private void AniPlayY(string leftName, string rightName, int track = 1, bool loop = false)
    {
        if (sr.flipX) skeleton.AnimationState.SetAnimation(track, leftName, loop);
        else skeleton.AnimationState.SetAnimation(track, rightName, loop);
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触可交互物体，尝试启动交互
        if (collision.TryGetComponent(out IInteractable it))
        {
            interactObject = it;
            it.EnableInteract(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 离开可交互物体，终止可交互状态
        if (collision.TryGetComponent(out IInteractable it))
        {
            it.DisableInteract();
            interactObject = null;
        }
    }
    #endregion
}
