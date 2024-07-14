using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : BallMountable, IBaseMechanism
{
    public float shootDelay = 2f; // 发射延迟时间
    public Transform shootDirectionTransform; // 发射方向
    public float shootForce = 10f; // 发射力度

    public SpriteRenderer tubeSpriteRenderer;
    public SpriteRenderer baseSpriteRenderer;
    // public GameObject hamster; // 仓鼠
    // public GameObject ball; // 球
    // public GameObject father;

    // private bool isShooting = false;
    private bool canShoot = true;

    private AudioSource audioSource;

    private void Start()
    {
        // hamster = GameObject.FindGameObjectWithTag("Player");
        // ball = GameObject.FindGameObjectWithTag("Ball");
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
    }

    protected override void MountBall()
    {
        if (!canShoot) return;

        base.MountBall();

        tubeSpriteRenderer.sortingLayerName = "Middle";
        tubeSpriteRenderer.sortingOrder = 1;
        baseSpriteRenderer.sortingLayerName = "Middle";
        baseSpriteRenderer.sortingOrder = 2;
        // hamster.transform.position = transform.position;
        // ball.transform.position = transform.position;

        Debug.Log("准备发射");
        // isShooting = true;
        canShoot = false;

        StartCoroutine(ShootAfterDelay());
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Ball") && canShoot)
    //     {

    //         // 存储球的引用，以备发射时使用
    //         // ball = other.gameObject;

    //         // 吸入并禁用控制
    //         ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //         hamster.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //         ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    //         hamster.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    //         hamster.GetComponent<MouseController>().enabled = false;
    //         // father.SetActive(false);
    //         // ball.GetComponent<Rigidbody2D>().freezeRotation = true;
    //         // DisableRenderer();

    //         // 启动发射倒计时

    //     }
    // }

    IEnumerator ShootAfterDelay()
    {
        yield return new WaitForSeconds(shootDelay);

        // 发射方向
        Vector2 direction = shootDirectionTransform.position - transform.position;
        direction.Normalize();

        // 发射球，并启用控制
        audioSource.Play();
        DismountBall();
        // ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        // hamster.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        ball.GetComponent<Rigidbody2D>().AddForce(direction * shootForce, ForceMode2D.Impulse);
        // hamster.GetComponent<MouseController>().enabled = true;
        // isShooting = false;

        // father.SetActive(true);
        // EnableRenderer();
        // ball.GetComponent<Rigidbody2D>().freezeRotation = false;
        // hamster.GetComponent<Rigidbody2D>().AddForce(direction * shootForce, ForceMode2D.Impulse);

        // 清空存储的球的引用
        // ball = null;
        yield return new WaitForSeconds(shootDelay);
        tubeSpriteRenderer.sortingLayerName = "Bottom";
        tubeSpriteRenderer.sortingOrder = 10;
        baseSpriteRenderer.sortingLayerName = "Bottom";
        baseSpriteRenderer.sortingOrder = 11;
        canShoot = true;
    }

    // private void DisableRenderer()
    // {
    //     // 禁用 Hamster 的 MeshRenderer
    //     foreach (Transform child in hamster.transform)
    //     {
    //         MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();
    //         if (childMeshRenderer != null)
    //         {
    //             childMeshRenderer.enabled = false;
    //         }
    //     }

    //     // 禁用 Ball 的 SpriteRenderer
    //     if (ball != null)
    //     {
    //         SpriteRenderer sr = ball.GetComponent<SpriteRenderer>();
    //         if (sr != null)
    //             sr.enabled = false;
    //     }
    // }

    // private void EnableRenderer()
    // {
    //     // 启用 Hamster 的 MeshRenderer
    //     foreach (Transform child in hamster.transform)
    //     {
    //         MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();
    //         if (childMeshRenderer != null)
    //         {
    //             childMeshRenderer.enabled = true;
    //         }
    //     }

    //     // 启用 Ball 的 SpriteRenderer
    //     if (ball != null)
    //     {
    //         SpriteRenderer sr = ball.GetComponent<SpriteRenderer>();
    //         if (sr != null)
    //             sr.enabled = true;
    //     }
    // }
}
