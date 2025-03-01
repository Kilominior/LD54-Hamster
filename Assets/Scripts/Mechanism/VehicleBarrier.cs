using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBarrier : MonoBehaviour
{
    [Tooltip("停止载具后将其transform父物体设置为该对象")]
    [SerializeField]
    private Transform AttachObjectTrans;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 汽车进入碰撞体后，永久停止其前进，并设置其父物体为目标对象
        if (collision.TryGetComponent<VehicleController>(out var vehicle))
        {
            vehicle.StopMoving();
            if(AttachObjectTrans != null)
            {
                vehicle.transform.SetParent(AttachObjectTrans.transform);
            }
        }
    }
}
