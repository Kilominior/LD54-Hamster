using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable
{
    private SpriteRenderer sr;

    // 鼠鼠本体
    private GameObject hamster;
    // 鼠鼠的刚体
    private Rigidbody2D hrb;
    // 鼠球，若初始其中就有球请挂载
    public GameObject ball;
    // 鼠球的刚体
    private Rigidbody2D brb;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;
    }

    public void EnableInteract()
    {
        sr.color = Color.green;
    }

    public void DisableInteract()
    {
        sr.color = Color.red;
    }

    public void ExecuteInteract(MouseController player)
    {
        hamster = player.gameObject;
        hrb = hamster.GetComponent<Rigidbody2D>();
        ball = player.ball.gameObject;
        brb = ball.GetComponent<Rigidbody2D>();

        //player.StateSwitch();
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.5f;
            sr.color = Color.cyan;
            hrb.interpolation = RigidbodyInterpolation2D.Interpolate;
            brb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
        else
        {
            Time.timeScale = 1.0f;
            sr.color = Color.blue;
            hrb.interpolation = RigidbodyInterpolation2D.None;
            brb.interpolation = RigidbodyInterpolation2D.None;
        }
    }
}
