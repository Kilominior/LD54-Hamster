using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;

    private void OnEnable()
    {
        BallController.OnHealthChangeEvent += DecreaseHealth;
    }

    private void OnDisable()
    {
        BallController.OnHealthChangeEvent -= DecreaseHealth;
    }

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime / 2;
        }
    }

    private void DecreaseHealth(float percentage)
    {
        healthImage.fillAmount = percentage;
    }
}
