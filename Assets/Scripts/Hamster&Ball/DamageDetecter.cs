using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetecter : MonoBehaviour
{
    [Tooltip("伤害阈值，当撞击速度超过这一阈值时会计算伤害")]
    public float damageThreshold;
    [Tooltip("伤害倍率，最终的伤害会由速度值乘以此倍率产生")]
    [SerializeField]
    private float damageMultiplier = 1.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检查碰撞是否是由 Ball 的刚体引起的
        if (collision.collider.GetComponent<Rigidbody2D>() != null && collision.transform.CompareTag("Ball"))
        {
            // 冲撞速度
            float collisionSpeed = collision.relativeVelocity.magnitude;
            if (collisionSpeed > damageThreshold)
            {
                // 计算伤害值
                int damageAmount = Mathf.RoundToInt(collisionSpeed * damageMultiplier);

                // 执行伤害操作
                InflictDamage(damageAmount, collision.transform.GetComponent<BallController>());
            }
        }
    }

    private void InflictDamage(int damageAmount, BallController ball)
    {
        // 受伤方法
        Debug.Log("受到伤害：" + damageAmount);
        ball.GetDamage(damageAmount);
    }
}