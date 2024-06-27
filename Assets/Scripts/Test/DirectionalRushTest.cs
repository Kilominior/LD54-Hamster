using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionalRushTest : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private Mouse curMouse;

    private LineRenderer dirRenderer;

    private Color clearColor;

    private Vector3 mousePos;

    private bool isPressing;

    void Start()
    {
        dirRenderer = GetComponent<LineRenderer>();
        clearColor = new Color(0, 0, 0, 0);
        isPressing = false;
    }

    void Update()
    {
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

        if (isPressing)
        {
            mousePos = curMouse.position.ReadValue();
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.z = 0;
            dirRenderer.SetPosition(0, player.transform.position);
            dirRenderer.SetPosition(1, mousePos);
            dirRenderer.startColor = Color.blue;
            dirRenderer.endColor = Color.green;
        }
        else
        {
            dirRenderer.startColor = clearColor;
            dirRenderer.endColor = clearColor;
        }
    }
}
