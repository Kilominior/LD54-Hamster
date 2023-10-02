using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : MonoBehaviour
{
    public GameObject[] points;
    public float moveSpeed;
    public GameObject player;
    public GameObject pressurePlate;

    private bool isPlayerOnLift = false;
    private Transform destination;
    private ObjectCheck objectCheck;

    private void Start()
    {
        if (pressurePlate != null)
        {
            objectCheck = pressurePlate.GetComponent<ObjectCheck>();
            // 订阅按下事件
            objectCheck.OnPlatePressed += StartMove;

            // 订阅释放事件
            // objectCheck.OnPlateReleased += OnPlateReleased;
        }
        else
        {
            Debug.LogError("ObjectCheck component not found on this object.");
        }
    }

    public void StartMove()
    {
        isPlayerOnLift = true;

        // 设置电梯的目的地
        if (transform.position == points[0].transform.position)
        {
            SetDestination(points[1].transform);
        }
        else if (transform.position == points[1].transform.position)
        {
            SetDestination(points[0].transform);
        }
    }

    public void StopMove()
    {
        isPlayerOnLift = false;
    }

    private void Update()
    {
        if (isPlayerOnLift)
        {
            MoveLift();
        }
    }

    private void SetDestination(Transform destination)
    {
        this.destination = destination;
    }

    private void MoveLift()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, destination.position, step);

        if (transform.position == destination.position)
        {
            StartMove();
            // isPlayerOnElevator = false;

            // 恢复
            // player.transform.SetParent(null);
        }
    }
}
