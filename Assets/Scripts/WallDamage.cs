using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDamage : MonoBehaviour
{
    // 伤害阈值
    public float damageThreshold = 5f;
    // 速度阈值
    public float velocityThreshold = 2f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞是否是由刚体引起的
        if (collision.collider.GetComponent<Rigidbody2D>() != null)
        {
            // 冲撞速度
            float collisionSpeed = collision.relativeVelocity.magnitude;
            if (collisionSpeed > velocityThreshold && collisionSpeed > damageThreshold)
            {
                // 计算伤害值
                int damageAmount = Mathf.RoundToInt(collisionSpeed);

                // 执行伤害操作
                InflictDamage(damageAmount);
            }
        }
    }

    private void InflictDamage(int damageAmount)
    {
        // 受伤方法
        Debug.Log("受到伤害：" + damageAmount);
    }
}