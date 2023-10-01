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
    public float exitTime = 0.5f;          // 判定仓鼠球离开前的等待时间
    public float exitDistance = 0.32f;     // 判定仓鼠球离开的距离
    public float returnTime = 1.0f;        // 判定仓鼠球离开后重新返回的等待时间
    public bool exiting;                   // 当前球已离开，暂时不判定球的进入

    private void Start()
    {
        if(ball != null) hasBallIn = true;
        else hasBallIn = false;
        exiting = false;

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
        if(collision.tag == "Ball" && !hasBallIn && !exiting)
        {
            Debug.Log(name + ": Ball Detected!");
            ball = collision.gameObject;
            hasBallIn = true;
            //collision.GetComponent<Rigidbody2D>().MovePosition(transform.position);
            StartCoroutine(nameof(BallAbsorb), collision.gameObject);
        }
    }

    // 在球到达发电机时自动吸附球
    private IEnumerator BallAbsorb(GameObject ball)
    {
        while((ball.transform.position - transform.position).magnitude > 0.1f)
        {
            Vector2 absorbForce = (transform.position - ball.transform.position) * 10.0f;
            ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);
            yield return new WaitForEndOfFrame();
        }
        ball.transform.position = transform.position;
        ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        StartCoroutine(nameof (WaitForLock), ball);
        Debug.Log(name + ": Ball Locked!");
    }

    // 判定球离开前等待的时间
    private IEnumerator WaitForLock(GameObject ball)
    {
        yield return new WaitForSeconds(exitTime);
        StartCoroutine(nameof(BallControl), ball);
    }

    // 在锁定球后控制其水平位置，同时也判断仓鼠是否试图挣脱
    private IEnumerator BallControl(GameObject ball)
    {
        ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        Vector3 distance;
        while (hasBallIn)
        {
            distance = transform.position - ball.transform.position;
            Debug.Log(distance.magnitude);
            if (distance.magnitude >= exitDistance)
            {
                Debug.Log(name + ": Ball Let Go!");
                exiting = true;
                StopCoroutine(nameof(WaitForExit));
                StartCoroutine(nameof(WaitForExit));
                ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                hasBallIn = false; break;
            }
            Vector2 absorbForce = (distance) * 60.0f;
            ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);
            yield return new WaitForEndOfFrame();
        }
    }

    // 球离开后重新判定球进入前的等待时间
    private IEnumerator WaitForExit()
    {
        yield return new WaitForSeconds(returnTime);
        exiting = false;
    }
}
