using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLegController : MonoBehaviour
{
    public bool onGround;

    void Start()
    {
        onGround = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ball")
        {
            onGround = true;
            // 一旦落地，二段跳自动无效
            transform.parent.GetComponent<MouseController>().isDoubleJump = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            onGround = false;
        }
    }
}
