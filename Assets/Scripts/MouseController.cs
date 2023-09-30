using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 0.6f;          // 行动速度
    public float rushSpeedY = 200f;         // Y方向的冲撞速度
    public float rushSpeedX = 300f;         // Y方向的冲撞速度

    private float InputSpeedX;              // X方向的力
    private Vector3 rushForceX;             // X方向的冲撞力
    private Vector3 rushForceY;             // Y方向的冲撞力
    private Vector3 force;                  // 总力

    private double lastLeftTime;                      // 上一次按左的时间
    private double lastRightTime;                      // 上一次按右的时间
    public float doublePressTime = 0.2f;               // 判定双击的时间间隔

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastLeftTime = 0;
        lastRightTime = 0;
    }

    private void Update()
    {
        Movement();

        DoublePress(KeyCode.A, KeyCode.LeftArrow);
        DoublePress(KeyCode.D, KeyCode.RightArrow);

        RushY();
    }

    // 左右移动
    private void Movement()
    {
        InputSpeedX = Input.GetAxisRaw("Horizontal");
        force = new(InputSpeedX * moveSpeed, 0);
        rb.AddForce(force, ForceMode2D.Force);

        if (force != Vector3.zero)
        {/*
            if (skeleton.AnimationState.GetCurrent(1) == null
                || skeleton.AnimationState.GetCurrent(1).IsComplete)
            {
                if (force.x < 0)
                {
                    if (!GetComponent<SpriteRenderer>().flipX)
                    {
                        GetComponent<SpriteRenderer>().flipX = true;
                        skeleton.AnimationState.SetAnimation(0, "turn", false);
                    }
                    skeleton.AnimationState.SetAnimation(1, "animation2", true);
                }
                else
                {
                    if (GetComponent<SpriteRenderer>().flipX)
                    {
                        GetComponent<SpriteRenderer>().flipX = false;
                        skeleton.AnimationState.SetAnimation(0, "turn_back", false);
                    }
                    skeleton.AnimationState.SetAnimation(1, "animation", true);
                }
            }*/
        }
    }

    // 上下冲撞
    private void RushY()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Rush to Up");
            rushForceY = new(0, rushSpeedY);
            rb.AddForce(rushForceY, ForceMode2D.Force);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Rush to Bottom");
            rushForceY = new(0, -rushSpeedY);
            rb.AddForce(rushForceY, ForceMode2D.Force);
        }
    }

    // 左右冲刺
    private void RushX(bool toRight)
    {
        Debug.Log("Rush to " + (toRight ? "Right" : "Left"));
        if (toRight) rushForceX = new(rushSpeedX, 0);
        else rushForceX = new(-rushSpeedX, 0);
        rb.AddForce(rushForceX, ForceMode2D.Force);
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

}
