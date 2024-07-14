using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject mainCamera;
    public static CameraManager instance;

    private GameObject currentCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SwitchVCameraTo(GameObject newCamera)
    {
        if (mainCamera == null)
        {
            return;
        }

        mainCamera.SetActive(false);
        newCamera.SetActive(true);
        currentCamera = newCamera;
    }

    public void SwitchVCameraBack()
    {
        if (mainCamera == null)
        {
            return;
        }

        mainCamera.SetActive(true);
        currentCamera.SetActive(false);
        currentCamera = mainCamera;
    }
}
