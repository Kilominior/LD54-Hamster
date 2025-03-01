using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour, IBaseMechanism
{
    public enum MoveDirection
    {
        Right,
        Left
    }

    public enum VehicleState
    {
        Idle,   // 静止状态
        Moving, // 移动状态
        Stopped // 遭遇障碍物无法继续前进
    }

    public float moveForce = 10f; // 控制车辆移动的力大小
    public MoveDirection moveDirection;
    private Rigidbody2D rb;
    private VehicleState state;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = VehicleState.Idle;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        if (state == VehicleState.Moving)
        {
            Move();
        }
    }

    public void StartMove()
    {
        state = VehicleState.Moving;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Move()
    {
        if (moveDirection == MoveDirection.Left)
            rb.AddForce(Vector2.left * moveForce, ForceMode2D.Force);
        else
            rb.AddForce(Vector2.right * moveForce, ForceMode2D.Force);
    }


    public void StopMoving()
    {
        state = VehicleState.Stopped;
        rb.velocity = Vector2.zero;
        // rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
