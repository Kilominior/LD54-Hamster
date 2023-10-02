using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftCheck : MonoBehaviour
{
    Transform parentTransform;
    LiftController liftController;

    private void Start()
    {
        parentTransform = transform.parent;
        if (parentTransform != null)
            liftController = parentTransform.GetComponent<LiftController>();
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ball"))
    //     {
    //         Debug.Log("角色进入升降梯");

    //         // 设置角色的父物体为电梯
    //         // other.transform.SetParent(parentTransform);
    //         liftController.StartMove();
    //     }
    // }
}
