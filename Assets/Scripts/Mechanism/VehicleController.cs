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

    public float moveForce = 10f; // 控制车辆移动的力大小
    public MoveDirection moveDirection;
    private Rigidbody2D rb;
    private bool isStarted;

    private void Start()
    {
        isStarted = false;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void FixedUpdate()
    {
        if (isStarted)
            Move();
    }

    public void StartMove()
    {
        Debug.Log("汽车开始移动");
        isStarted = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Move()
    {
        if (moveDirection == MoveDirection.Left)
            rb.AddForce(Vector2.left * moveForce, ForceMode2D.Force);
        else
            rb.AddForce(Vector2.right * moveForce, ForceMode2D.Force);
    }
}
