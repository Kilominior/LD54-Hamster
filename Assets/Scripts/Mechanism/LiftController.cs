using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : TriggerMechanism
{
    public GameObject[] points;
    public float moveSpeed;
    public bool isMoveOnce;

    private bool isMoving;
    private Transform destination;

    private void Start()
    {
        GetComponent<LineRenderer>().SetPosition(0, points[0].transform.position);
        GetComponent<LineRenderer>().SetPosition(1, points[1].transform.position);
    }

    protected override void ExecuteTrigger()
    {
        StartMove();
    }

    public void StartMove()
    {
        isMoving = true;

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
        isMoving = false;
    }

    private void Update()
    {
        if (isMoving)
            MoveLift();
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
            if (isMoveOnce)
                StopMove();
            else
                StartMove();
        }
    }
}
