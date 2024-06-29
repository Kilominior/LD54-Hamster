using Spine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using Spine.Unity;

public class MouseController : MonoBehaviour
{
    // 鼠/球状态
    public enum PlayerState
    {
        Ball,
        Hamster
    }
    // 当前所在的状态
    public PlayerState currentState;

    public BallController ball;
    private static readonly string ballPrefabPath = "Prefab/Ball";
    private GameObject ballPrefab;

    private MouseLegController mouseLeg;
    // 行动速度
    [SerializeField]
    private float moveSpeed = 10f;
    // Y方向的冲撞速度
    [SerializeField]
    private float JumpSpeed = 150f;
    // Y方向的冲撞速度
    [SerializeField]
    private float rushSpeed = 200f;
    // X方向冲刺是否正在冷却
    private bool rushCooling;
    // X方向冲刺的冷却时间
    private float rushCD = 1.0f;          

    // 移动时的力
    private Vector2 moveForce;
    // X方向的冲撞力
    private Vector3 rushForceX;
    // Y方向的冲撞力
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
    // 判定是否允许二段跳
    public bool isDoubleJump;
    // 上一次按左/右的时间
    private double lastLeftTime;
    private double lastRightTime;
    // 判定双击的时间间隔
    public float doublePressTime = 0.2f;

    // 当前可执行交互操作的对象
    private IInteractable interactObject;
    private bool interactLegal;

    private Rigidbody2D rb;
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

        interactLegal = false;

        rushCooling = false;
        isDoubleJump = false;
        lastLeftTime = 0;
        lastRightTime = 0;
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
        //Movement();

        //DoublePress(KeyCode.A, KeyCode.LeftArrow);
        //DoublePress(KeyCode.D, KeyCode.RightArrow);

        //RushY();

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
        pi.onControlsChanged += dr.OnControlsUpdate;

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
        interactObject.ExecuteInteract(this);
        interactLegal = false;
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

    private void Jump()
    {
        // 保证在地面上起跳
        if (!mouseLeg.onGround) { return; }
        JumpForce = new(0, JumpSpeed);
        rb.AddForce(JumpForce, ForceMode2D.Force);
        AniPlayY("jump_left", "jump_right");
    }

    // 上下冲撞
    private void RushY()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (mouseLeg.onGround)
            {
                Debug.Log("Rush to Up");
                isDoubleJump = true;
                JumpForce = new(0, JumpSpeed);
                rb.AddForce(JumpForce, ForceMode2D.Force);
                AniPlayY("jump_left", "jump_right");
                return;
            }
            if (isDoubleJump)
            {
                Debug.Log("Double Rush to Up");
                isDoubleJump = false;
                JumpForce = new(0, JumpSpeed);
                rb.AddForce(JumpForce, ForceMode2D.Force);
                AniPlayY("jump_left", "jump_right");
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (mouseLeg.onGround) return;
            Debug.Log("Rush to Bottom");
            JumpForce = new(0, -JumpSpeed);
            rb.AddForce(JumpForce, ForceMode2D.Force);
            AniPlayY("strike_down_left", "strike_down_right");
        }
    }

    // 左右冲刺
    private void RushX(bool toRight)
    {
        if (rushCooling) return;
        Debug.Log("Rush to " + (toRight ? "Right" : "Left"));
        if (toRight) rushForceX = new(rushSpeed, 0);
        else rushForceX = new(-rushSpeed, 0);
        rb.AddForce(rushForceX, ForceMode2D.Force);
        AniPlayX(rushForceX.x, "strike_left", "strike_right");
        rushCooling = true;
        StartCoroutine(nameof(rushCoolDown));
    }

    // 双击左右键触发冲撞
    private void DoublePress(KeyCode key0, KeyCode key1)
    {
        if (Input.GetKeyDown(key0) || Input.GetKeyDown(key1))
        {
            if (key0 == KeyCode.A)
            {
                if (Time.realtimeSinceStartup - lastLeftTime < doublePressTime)
                {
                    RushX(false);
                }
                lastLeftTime = Time.realtimeSinceStartup;
            }
            if (key0 == KeyCode.D)
            {
                if (Time.realtimeSinceStartup - lastRightTime < doublePressTime)
                {
                    RushX(true);
                }
                lastRightTime = Time.realtimeSinceStartup;
            }
        }
    }

    private IEnumerator rushCoolDown()
    {
        yield return new WaitForSeconds(rushCD);
        rushCooling = false;
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
        // 接触可交互物体，判断是否允许进行交互
        if (collision.TryGetComponent(out IInteractable it))
        {
            interactObject = it;
            interactObject.EnableInteract();
            interactLegal = true;
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
