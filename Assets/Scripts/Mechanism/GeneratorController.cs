using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GeneratorController : MonoBehaviour
{
    public GameObject connectedObject;     // 与本发电机相连的物体
    public bool objectActivated;           // 物体开关状态
    public Sprite darkSprite;              // 没电贴图
    public Sprite lightSprite;             // 有电贴图

    private Transform centerPoint;         // 电机的中心位置
    public List<Transform> LinePoints;     // 电线的节点
    private Transform pointParent;         // 电线节点父物体
    private LineRenderer lineRenderer;     // 电线绘制器

    public bool hasBallIn;                 // 仓鼠球已固定
    public GameObject ball;                // 仓鼠球，若初始其中就有球请挂载
    public float exitTime = 0.5f;          // 判定仓鼠球离开前的等待时间
    public float exitDistance = 0.32f;     // 判定仓鼠球离开的距离
    public float returnTime = 1.0f;        // 判定仓鼠球离开后重新返回的等待时间
    public bool exiting;                   // 当前球已离开，暂时不判定球的进入

    private Quaternion inRotation;         // 仓鼠球锁定时的旋转
    public float generateVelocity = 600f;  // 发电需要的角速度
    public int generateRound = 3;          // 发电需要的圈数
    private int roundNow;                  // 当前已旋转圈数

    private void Start()
    {
        if (objectActivated) GetComponent<SpriteRenderer>().sprite = lightSprite;
        else GetComponent<SpriteRenderer>().sprite = darkSprite;

        if (ball != null) hasBallIn = true;
        else hasBallIn = false;
        exiting = false;

        centerPoint = transform.Find("Center");
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

        UpdateActivatedStatus();
    }

    private void Update()
    {
        // 更新所有电线节点状态
        for (int i = 0; i < LinePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, LinePoints[i].position);
        }
    }

    // 更新连接电器的开关状态
    private void UpdateActivatedStatus()
    {
        // TODO 更新线条材质
        if (objectActivated) GetComponent<SpriteRenderer>().sprite = lightSprite;


        // 电灯
        if (connectedObject.GetComponent<LampController>())
        {
            if (objectActivated)
            {
                connectedObject.GetComponent<LampController>().GetComponent<SpriteRenderer>().sprite
                = connectedObject.GetComponent<LampController>().lightSprite;
            }
            //else
            //{
            //    connectedObject.GetComponent<LampController>().GetComponent<SpriteRenderer>().sprite
            //    = connectedObject.GetComponent<LampController>().darkSprite;
            //}
        }
        // 电梯
        else if (connectedObject.GetComponent<LiftController>())
        {
            if (objectActivated)
            {
                connectedObject.GetComponent<LiftController>().StartMove();
            }
            //else
            //{
            //    connectedObject.GetComponent<LiftController>().StopMove();
            //}
        }
        // 汽车
        else if (connectedObject.GetComponent<VehicleController>())
        {
            if (objectActivated)
                connectedObject.GetComponent<VehicleController>().StartMove();
            // else
            //     connectedObject.GetComponent<LiftController>().StopMove();
        }
        // 黑幕
        else if (connectedObject.GetComponent<BlackPanelManager>())
        {
            if (objectActivated)
                connectedObject.GetComponent<BlackPanelManager>().DecreaseBlack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && !hasBallIn && !exiting)
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
        while ((ball.transform.position - centerPoint.position).magnitude > 0.1f)
        {
            Vector2 absorbForce = (centerPoint.position - ball.transform.position) * 10.0f;
            ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);
            // ball.transform.position = transform.position;
            yield return new WaitForEndOfFrame();
        }
        ball.transform.position = centerPoint.position;
        ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        StartCoroutine(nameof(WaitForLock), ball);
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
        // 置于电梯上的发电机不锁定仓鼠球Y轴
        if (transform.parent.GetComponent<LiftController>())
            ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        else ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        inRotation = ball.transform.rotation;
        roundNow = 0;
        Vector3 distance;
        while (hasBallIn)
        {
            // 离开判定
            distance = centerPoint.position - ball.transform.position;
            //Debug.Log(distance.magnitude);
            // 位置维持
            Vector2 absorbForce = (distance) * 60.0f;
            ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);

            // 发电判定
            if (ball.transform.rotation == inRotation)
            {
                roundNow++;
            }
            // 成功发电时使得连接的电器开或关
            if (roundNow >= generateRound && Mathf.Abs(ball.GetComponent<Rigidbody2D>().angularVelocity) >= generateVelocity)
            {
                Debug.Log(name + "Electricity Generated!");
                objectActivated = true;
                //if (objectActivated) objectActivated = false;
                //else objectActivated = true;
                UpdateActivatedStatus();
                roundNow = 0;
            }
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
