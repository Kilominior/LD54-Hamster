using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChange : MonoBehaviour
{
    public GameObject fullSceneCamera;
    public GameObject mouseCamera;

    private float timer = 0f;
    public float targetTime = 5f;
    private bool isTimerRunning = true;

    private void Start()
    {
        fullSceneCamera = GameObject.Find("Full Scene Camera");
        fullSceneCamera.SetActive(true);
        mouseCamera.SetActive(false);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;

            if (timer >= targetTime)
            {
                Debug.Log("Timer reached 5 seconds. Stopping...");
                isTimerRunning = false;
                fullSceneCamera.SetActive(false);
                mouseCamera.SetActive(true);
            }
        }
    }
}
