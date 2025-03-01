using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : BallMountable, IBaseMechanism, ISubInteractable
{
    private static readonly string DefaultSortingLayerName = "Bottom";
    private static readonly string FireSortingLayerName = "Middle";

    public float shootDelay = 2f; // 发射延迟时间
    public Transform shootDirectionTransform; // 发射方向
    public float shootForce = 10f; // 发射力度

    public SpriteRenderer tubeSpriteRenderer;
    public SpriteRenderer baseSpriteRenderer;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected override void MountBall()
    {
        base.MountBall();
        SwitchCameraToTargetGroup();

        tubeSpriteRenderer.sortingLayerName = FireSortingLayerName;
        tubeSpriteRenderer.sortingOrder = 2;
        baseSpriteRenderer.sortingLayerName = FireSortingLayerName;
        baseSpriteRenderer.sortingOrder = 3;
    }

    protected override void DismountBall()
    {
        base.DismountBall();
        CameraManager.instance?.SwitchVCameraBack();
    }

    IEnumerator ShootAfterDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        Shoot();
    }

    private void Shoot()
    {
        // 发射方向
        Vector2 direction = shootDirectionTransform.position - transform.position;
        direction.Normalize();

        // 发射球，并启用控制
        audioSource.Play();
        DismountBall();
        ball.GetComponent<Rigidbody2D>().AddForce(direction * shootForce, ForceMode2D.Impulse);

        // 重置显示状态
        tubeSpriteRenderer.sortingLayerName = DefaultSortingLayerName;
        tubeSpriteRenderer.sortingOrder = 10;
        baseSpriteRenderer.sortingLayerName = DefaultSortingLayerName;
        baseSpriteRenderer.sortingOrder = 11;
    }

    private void SwitchCameraToTargetGroup()
    {
        if (CameraManager.instance != null)
        {
            if (CameraManager.instance.IsTargetGroupRegistered(this))
            {
                CameraManager.instance.SwitchVCameraTo(this);
            }
            else
            {
                // 实例化机制相机
                CameraManager.instance.RegisterTargetGroup(this, transform, shootDirectionTransform);
                CameraManager.instance.SwitchVCameraTo(this);
            }
        }
    }

    public void ExecuteSubInteract(MouseController player)
    {
        if(HasObjectMounted())
        {
            Shoot();
        }
    }
}
