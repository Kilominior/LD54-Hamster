using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionalRushTest : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private Mouse curMouse;

    private LineRenderer dirRenderer;

    private Color clearColor;

    private Vector3 mousePosLast;

    private Vector3 mousePosCur;

    private Vector3 mousePosPredict;

    private Vector3 mousePosInterpolate;

    private float t_update;

    private bool isPressing;

    void Start()
    {
        dirRenderer = GetComponent<LineRenderer>();
        clearColor = new Color(0, 0, 0, 0);
        isPressing = false;
        t_update = 0;
    }

    private void FixedUpdate()
    {

        if (isPressing)
        {
            mousePosCur = curMouse.position.ReadValue();
            mousePosCur = Camera.main.ScreenToWorldPoint(mousePosCur);
            mousePosCur.z = 0;
            mousePosPredict = mousePosCur + (mousePosCur - mousePosLast);
            mousePosInterpolate = math.lerp(mousePosCur, mousePosPredict, t_update / Time.fixedDeltaTime);

            Debug.Log(t_update / Time.fixedDeltaTime);

            dirRenderer.SetPosition(0, player.transform.position);
            dirRenderer.SetPosition(1, mousePosCur);
            dirRenderer.startColor = Color.blue;
            dirRenderer.endColor = Color.green;

            mousePosLast = mousePosCur;
        }
        else
        {
            dirRenderer.startColor = clearColor;
            dirRenderer.endColor = clearColor;
        }

        t_update = 0;
    }

    void Update()
    {
        t_update += Time.deltaTime;
        curMouse = Mouse.current;
        if (curMouse != null)
        {
            if (curMouse.leftButton.wasPressedThisFrame)
            {
                isPressing = true;
            }
            if (curMouse.leftButton.wasReleasedThisFrame)
            {
                isPressing = false;
            }
        }
    }
}
