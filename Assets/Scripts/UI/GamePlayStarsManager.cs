using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayStarsManager : MonoBehaviour
{
    public Sprite[] stars;
    public LampController lamp1;
    public LampController lamp2;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (lamp1.IsLighted() && lamp2.IsLighted())
            image.sprite = stars[2];
        else if (!lamp1.IsLighted() && !lamp2.IsLighted())
            image.sprite = stars[0];
        else
            image.sprite = stars[1];
    }
}
