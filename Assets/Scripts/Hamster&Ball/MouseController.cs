using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    private Rigidbody2D rb;
    private MouseLegController mouseLeg;
    public float moveSpeed = 0.6f;          // 行动速度
    public float rushSpeedY = 300f;         // Y方向的冲撞速度
    public float rushSpeedX = 600f;         // Y方向的冲撞速度
    public bool rushCooling;                // X方向冲刺是否正在冷却
    public float rushCDofX = 1.0f;          // X方向冲刺的冷却时间

    private float InputSpeedX;              // X方向的力
    private Vector3 rushForceX;             // X方向的冲撞力
    private Vector3 rushForceY;             // Y方向的冲撞力
    private Vector3 force;                  // 总力

    public bool isDoubleJump;                         // 判定是否允许二段跳
    private double lastLeftTime;                      // 上一次按左的时间
    private double lastRightTime;                     // 上一次按右的时间
    public float doublePressTime = 0.2f;              // 判定双击的时间间隔

    private SkeletonAnimation skeleton;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mouseLeg = transform.Find("PlaneCheck").GetComponent<MouseLegController>();
        skeleton = transform.GetChild(0).GetComponent<SkeletonAnimation>();
        sr = GetComponent<SpriteRenderer>();

        rushCooling = false;
        isDoubleJump = false;
        lastLeftTime = 0;
        lastRightTime = 0;
    }

    private void Update()
    {
        Movement();

        DoublePress(KeyCode.A, KeyCode.LeftArrow);
        DoublePress(KeyCode.D, KeyCode.RightArrow);

        RushY();

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

    // 左右移动
    private void Movement()
    {
        InputSpeedX = Input.GetAxis("Horizontal");
        force = new(InputSpeedX * moveSpeed, 0);
        rb.AddForce(force, ForceMode2D.Force);

        if (force != Vector3.zero)
        {
            // 动画
            if (skeleton.AnimationState.GetCurrent(1) == null
                || skeleton.AnimationState.GetCurrent(1).IsComplete)
                AniPlayX(force.x, "run_left", "run_right", 1, true);
        }
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
                rushForceY = new(0, rushSpeedY);
                rb.AddForce(rushForceY, ForceMode2D.Force);
                AniPlayY("jump_left", "jump_right");
                return;
            }
            if (isDoubleJump)
            {
                Debug.Log("Double Rush to Up");
                isDoubleJump = false;
                rushForceY = new(0, rushSpeedY);
                rb.AddForce(rushForceY, ForceMode2D.Force);
                AniPlayY("jump_left", "jump_right");
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (mouseLeg.onGround) return;
            Debug.Log("Rush to Bottom");
            rushForceY = new(0, -rushSpeedY);
            rb.AddForce(rushForceY, ForceMode2D.Force);
            AniPlayY("strike_down_left", "strike_down_right");
        }
    }

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

    // 左右冲刺
    private void RushX(bool toRight)
    {
        if (rushCooling) return;
        Debug.Log("Rush to " + (toRight ? "Right" : "Left"));
        if (toRight) rushForceX = new(rushSpeedX, 0);
        else rushForceX = new(-rushSpeedX, 0);
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
            if(key0 == KeyCode.A)
            {
                if (Time.realtimeSinceStartup - lastLeftTime < doublePressTime)
                {
                    RushX(false);
                }
                lastLeftTime = Time.realtimeSinceStartup;
            }
            if(key0 == KeyCode.D)
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
        yield return new WaitForSeconds(rushCDofX);
        rushCooling = false;
    }

}
