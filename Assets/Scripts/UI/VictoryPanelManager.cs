using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanelManager : MonoBehaviour
{
    public Sprite[] stars;
    public LampController lamp1;
    public LampController lamp2;
    public int score;

    private Image image;


    private void Start()
    {
        score = 0;
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (lamp1.IsLighted() && lamp2.IsLighted())
        {
            image.sprite = stars[2];
            score = 3;
        }
        else if (!lamp1.IsLighted() && !lamp2.IsLighted())
        {
            image.sprite = stars[0];
            score = 1;
        }
        else
        {
            image.sprite = stars[1];
            score = 2;
        }

    }
}
