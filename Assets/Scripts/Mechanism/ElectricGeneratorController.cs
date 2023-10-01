using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricGeneratorController : MonoBehaviour
{
    public GameObject connectedObject;     // 与本发电机相连的物体
    public List<Transform> LinePoints;     // 电线的节点
    private Transform pointParent;         // 电线节点父物体
    private LineRenderer lineRenderer;     // 电线绘制器

    public bool hasBallIn;                 // 仓鼠球已固定
    public GameObject ball;                // 仓鼠球，若初始其中就有球请挂载

    private void Start()
    {
        if(ball != null) hasBallIn = true;
        else hasBallIn = false;

        pointParent = transform.Find("PointParent");
        lineRenderer = GetComponent<LineRenderer>();

        // 初始化电线的每个节点，并绘制
        LinePoints = new List<Transform>
        {
            transform.Find("StartPoint")
        };
        lineRenderer.positionCount = pointParent.childCount + 2;
        lineRenderer.SetPosition(0, transform.Find("StartPoint").position);
        for (int i = 0; i < pointParent.childCount; i++)
        {
            LinePoints.Add(pointParent.GetChild(i));
            lineRenderer.SetPosition(i + 1, pointParent.GetChild(i).position);
        }
        LinePoints.Add(connectedObject.transform.Find("EndPoint"));
        lineRenderer.SetPosition(LinePoints.Count - 1, LinePoints[LinePoints.Count - 1].position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ball" && !hasBallIn)
        {
            Debug.Log(name + ": Ball Detected!");
            ball = collision.gameObject;
            hasBallIn = true;
            //collision.GetComponent<Rigidbody2D>().MovePosition(transform.position);
            StartCoroutine(nameof(BallAbsorb), collision.gameObject);
        }
    }

    private IEnumerator BallAbsorb(GameObject ball)
    {
        while(true)
        {
            Vector2 absorbForce = (transform.position - ball.transform.position) * 100.0f;
            ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);
            yield return new WaitForEndOfFrame();
        }

        Debug.Log(name + ": Ball Locked!");
    }
}
