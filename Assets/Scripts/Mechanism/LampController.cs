using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : MonoBehaviour
{
    public Sprite darkSprite;
    public Sprite lightSprite;

    private void Awake()
    {
        DisableLight();
    }

    public void EnableLight()
    {
        this.GetComponent<Light>().enabled = true;
    }

    public void DisableLight()
    {
        this.GetComponent<Light>().enabled = false;
    }
}
