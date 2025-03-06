using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : TriggerMechanism, ISwitchMechanism
{
    private SpriteRenderer sr;
    public Sprite darkSprite;
    public Sprite lightSprite;

    public bool IsLighted()
    {
        if (GetComponent<SpriteRenderer>().sprite == lightSprite)
            return true;
        return false;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void ExecuteTrigger()
    {
        TurnOn();
    }

    public void TurnOn()
    {
        sr.sprite = lightSprite;
    }

    public void TurnOff()
    {
        sr.sprite = darkSprite;
    }
}
