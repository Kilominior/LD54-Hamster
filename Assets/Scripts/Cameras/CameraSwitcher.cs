using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject fullCamera1;
    public GameObject fullCamera2;
    public GameObject mouseCamera;

    private float timer = 0f;
    public float switchFullCameraTime = 3f;
    public float targetTime = 6f;
    private bool isTimerRunning = true;
    private bool isSwitched = false;

    private void Start()
    {
        fullCamera2.SetActive(false);
        mouseCamera.SetActive(false);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;

            if (timer >= switchFullCameraTime && !isSwitched)
            {
                fullCamera1.SetActive(false);
                fullCamera2.SetActive(true);
                isSwitched = true;
            }
            else if (timer >= targetTime && isSwitched)
            {
                fullCamera2.SetActive(false);
                mouseCamera.SetActive(true);
                isTimerRunning = false;
            }
        }
    }
}
