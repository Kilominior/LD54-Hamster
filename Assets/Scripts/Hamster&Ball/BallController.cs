using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

public class BallController : MonoBehaviour
{
    public delegate void HealthChangeEventHandler(float percentage);
    public static event HealthChangeEventHandler OnHealthChangeEvent;
    public float MAXHP = 100f;      // 最大生命值
    public float HP;                // 当前生命值
    public Sprite[] ballSprites;
    private SpriteRenderer sr;

    private AudioSource audioSource;

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = ballSprites[0];
        HP = MAXHP;
        audioSource = GetComponent<AudioSource>();
    }

    public void GetDamage(float amount)
    {
        HP -= amount * 0.9f;
        audioSource.Play();
        if (HP <= 0)
        {
            HP = 0;
            StartCoroutine(BallBreak());
            // BallBreak();
        }
        if (HP != 0)
            ChangeBallSprite();
        OnHealthChange(HP / MAXHP);
    }

    public void OnHealthChange(float percentage)
    {
        // 触发事件
        OnHealthChangeEvent?.Invoke(percentage);
    }

    private void ChangeBallSprite()
    {
        if (HP < 25)
            sr.sprite = ballSprites[4];
        else if (HP < 50)
            sr.sprite = ballSprites[3];
        else if (HP < 75)
            sr.sprite = ballSprites[2];
        else
            sr.sprite = ballSprites[1];
    }

    IEnumerator BallBreak()
    {
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);
        TypeEventSystem.Global.Send<GameWinEvent>();
    }
}
