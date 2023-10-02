using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public float MAXHP = 100f;      // 最大生命值
    public float HP;                // 当前生命值
    public Sprite[] ballSprites;
    // public Image healthImage;
    // public Image healthDelayImage;
    public PanelManager panelManager;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = ballSprites[0];
        HP = MAXHP;
    }

    private void Update()
    {
        // if (healthDelayImage.fillAmount > healthImage.fillAmount)
        // {
        //     healthDelayImage.fillAmount -= Time.deltaTime;
        // }
    }

    public void GetDamage(float amount)
    {
        HP -= amount;
        if (HP < 0)
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
        // healthImage.fillAmount = percentage;
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

    // private void BallBreak()
    // {
    //     panelManager.EnableVictoryPanel();
    //     this.gameObject.SetActive(false);
    // }
    IEnumerator BallBreak()
    {
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        panelManager.EnableVictoryPanel();
    }
}
