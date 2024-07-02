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
    // 冲撞是否正在冷却
    private bool isRushCooling;
    // 冲撞的冷却时间
    private float rushCD = 1.0f;

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
    private PlayerInput pi;
    private InputActionMap ballAM;
    private InputActionMap hamsterAM;

    // 因有Move输入而正在移动中
    private bool isMoving;

    // 当前可执行交互操作的对象
    private IInteractable interactObject;
    private bool interactLegal;

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
        interactLegal = false;
        isRushCooling = false;
        EventRegister();
    }

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<TimeScaleSlowDownEvent>(OnTimeSlowDown);
        TypeEventSystem.Global.Register<TimeScaleRecoverEvent>(OnTimeRecover);
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
    #endregion

    #region Input
    private void ActionBinding()
    {
        pi = GetComponent<PlayerInput>();

        ballAM = pi.actions.actionMaps[0];
        hamsterAM = pi.actions.actionMaps[1];

        ballAM["Move"].performed += OnMovePerformed;
        ballAM["Move"].canceled += OnMoveCanceled;
        ballAM["Jump"].performed += OnJumpPerformed;
        ballAM["Interact"].performed += OnInteractPerformed;

        hamsterAM["Move"].performed += OnMovePerformed;
        hamsterAM["Move"].canceled += OnMoveCanceled;
        hamsterAM["Jump"].performed += OnJumpPerformed;
        hamsterAM["Interact"].performed += OnInteractPerformed;
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

        if (targetState == PlayerState.Ball)
        {
            if (ballPrefab == null)
            {
                ballPrefab = Resources.Load<GameObject>(ballPrefabPath);
            }
            ball = Instantiate(ballPrefab, transform.position,
                Quaternion.identity).GetComponent<BallController>();
            brb = ball.GetComponent<Rigidbody2D>();
            pi.SwitchCurrentActionMap(ballAMName);
        }
        else
        {
            if (ball != null) Destroy(ball.gameObject);
            pi.SwitchCurrentActionMap(hamsterAMName);
        }
    }

    private void ExecuteInteract()
    {
        if (interactObject == null || !interactLegal) return;
        // 根据交互对象的情况决定是否允许再次交互
        interactLegal = interactObject.ExecuteInteract(this);
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

    // 时间减缓时进行插值
    private void OnTimeSlowDown(TimeScaleSlowDownEvent @event)
    {
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        if(brb != null)
        {
            brb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    // 时间恢复时停止插值
    private void OnTimeRecover(TimeScaleRecoverEvent @event)
    {
        rb.interpolation = RigidbodyInterpolation2D.None;
        if (brb != null)
        {
            brb.interpolation = RigidbodyInterpolation2D.None;
        }
    }

    // 冲撞
    public void Rush(Vector2 dir)
    {
        if(isRushCooling) { return; }
        rushForce = dir * rushSpeed;
        rb.AddForce(rushForce, ForceMode2D.Impulse);
        AniPlayX(rushForce.x, "strike_left", "strike_right");
        StartCoroutine(nameof(rushCoolDown));
    }

    //// 上下冲撞
    //private void RushY()
    //{
    //    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
    //    {
    //        if (mouseLeg.onGround)
    //        {
    //            Debug.Log("Rush to Up");
    //            isDoubleJump = true;
    //            JumpForce = new(0, JumpSpeed);
    //            rb.AddForce(JumpForce, ForceMode2D.Force);
    //            AniPlayY("jump_left", "jump_right");
    //            return;
    //        }
    //        if (isDoubleJump)
    //        {
    //            Debug.Log("Double Rush to Up");
    //            isDoubleJump = false;
    //            JumpForce = new(0, JumpSpeed);
    //            rb.AddForce(JumpForce, ForceMode2D.Force);
    //            AniPlayY("jump_left", "jump_right");
    //            return;
    //        }
    //    }
    //    if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
    //    {
    //        if (mouseLeg.onGround) return;
    //        Debug.Log("Rush to Bottom");
    //        JumpForce = new(0, -JumpSpeed);
    //        rb.AddForce(JumpForce, ForceMode2D.Force);
    //        AniPlayY("strike_down_left", "strike_down_right");
    //    }
    //}

    //// 左右冲刺
    //private void RushX(bool toRight)
    //{
    //    if (isRushCooling) return;
    //    Debug.Log("Rush to " + (toRight ? "Right" : "Left"));
    //    if (toRight) rushForce = new(rushSpeed, 0);
    //    else rushForce = new(-rushSpeed, 0);
    //    rb.AddForce(rushForce, ForceMode2D.Force);
    //    AniPlayX(rushForce.x, "strike_left", "strike_right");
    //    isRushCooling = true;
    //    StartCoroutine(nameof(rushCoolDown));
    //}

    //// 双击左右键触发冲撞
    //private void DoublePress(KeyCode key0, KeyCode key1)
    //{
    //    if (Input.GetKeyDown(key0) || Input.GetKeyDown(key1))
    //    {
    //        if (key0 == KeyCode.A)
    //        {
    //            if (Time.realtimeSinceStartup - lastLeftTime < doublePressTime)
    //            {
    //                RushX(false);
    //            }
    //            lastLeftTime = Time.realtimeSinceStartup;
    //        }
    //        if (key0 == KeyCode.D)
    //        {
    //            if (Time.realtimeSinceStartup - lastRightTime < doublePressTime)
    //            {
    //                RushX(true);
    //            }
    //            lastRightTime = Time.realtimeSinceStartup;
    //        }
    //    }
    //}

    private IEnumerator rushCoolDown()
    {
        yield return new WaitForSeconds(rushCD);
        isRushCooling = false;
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
            interactLegal = it.TryEnableInteract(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 离开可交互物体，终止可交互状态
        if (collision.TryGetComponent(out IInteractable it))
        {
            interactObject = it;
            interactObject.DisableInteract();
            interactLegal = false;
        }
    }
    #endregion
}
