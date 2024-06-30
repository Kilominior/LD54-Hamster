using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLegController : MonoBehaviour
{
    public bool onGround;
    private MouseController owner;
    private int colliderCount;

    void Start()
    {
        onGround = false;
        owner = transform.parent.GetComponent<MouseController>();
        colliderCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Ball") && !collision.CompareTag("Ground"))
        {
            return;
        }
        //Debug.Log("In"+colliderCount);
        colliderCount++;
        onGround = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Ball") && !collision.CompareTag("Ground"))
        {
            return;
        }
        //Debug.Log("Out"+colliderCount);
        colliderCount--;
        if (colliderCount == 0)
        {
            onGround = false;
        }
    }
}
