using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricGeneratorController : MonoBehaviour, IInteractable
{
    // 与本发电机相连的物体
    public GameObject connectedObject;
    // 物体开关状态
    public bool objectActivated;
    // 没电贴图
    public Sprite darkSprite;
    // 有电贴图
    public Sprite lightSprite;

    // 电机的中心位置
    private Transform centerPoint;
    // 旋转目标指示器
    private GameObject rotateSignal;
    // 电线的节点
    public List<Transform> LinePoints;
    // 电线节点父物体
    private Transform pointParent;
    // 电线绘制器
    private LineRenderer lineRenderer;

    // 仓鼠球已固定
    public bool hasBallIn;
    // 鼠鼠本体
    private GameObject hamster;
    // 鼠鼠的刚体
    private Rigidbody2D hrb;
    // 鼠球，若初始其中就有球请挂载
    public GameObject ball;
    // 鼠球的刚体
    private Rigidbody2D brb;
    // 判定仓鼠球离开前的等待时间
    public float exitTime = 0.5f;
    // 判定仓鼠球离开的距离
    public float exitDistance = 0.32f;
    // 判定仓鼠球离开后重新返回的等待时间
    public float returnTime = 1.0f;
    // 当前球已离开，暂时不判定球的进入
    public bool exiting;

    // 仓鼠球锁定时的旋转
    private Quaternion inRotation;
    // 发电需要的角速度
    public float generateVelocity = 600f;
    // 发电需要的圈数
    public int generateRound = 3;
    // 当前已旋转圈数
    private int roundNow;
    // 锁定时旋转的对角线处
    private Quaternion inverseRotation;
    // 曾经到达对角处，此次旋转合法
    private bool hasRotated;
    // 判定旋转一周的角度偏移量
    public float angleBias = 1f;

    private AudioSource audioSource;
    public AudioClip[] clips;

    private void Start()
    {
        if (objectActivated) GetComponent<SpriteRenderer>().sprite = lightSprite;
        else GetComponent<SpriteRenderer>().sprite = darkSprite;

        if (ball != null) hasBallIn = true;
        else hasBallIn = false;
        exiting = false;

        centerPoint = transform.Find("Center");
        rotateSignal = centerPoint.gameObject;
        rotateSignal.SetActive(false);
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

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // 更新所有电线节点状态
        for (int i = 0; i < LinePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, LinePoints[i].position);
        }
        if (hasBallIn)
        {
            ElectricityGenerate();
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

    public void EnableInteract()
    {
        //
    }

    public void DisableInteract()
    {
        //
    }

    public void ExecuteInteract(MouseController player)
    {
        hamster = player.gameObject;
        hrb = hamster.GetComponent<Rigidbody2D>();
        ball = player.ball.gameObject;
        brb = ball.GetComponent<Rigidbody2D>();
        MountBall();
    }

    private void MountBall()
    {
        AudioPlay(0);
        // 将鼠球和鼠鼠固定到中心位置
        hrb.velocity = Vector2.zero;
        hamster.transform.position = centerPoint.position;

        brb.velocity = Vector2.zero;
        brb.bodyType = RigidbodyType2D.Kinematic;
        ball.transform.position = centerPoint.position;

        brb.bodyType = RigidbodyType2D.Dynamic;
        brb.constraints = RigidbodyConstraints2D.FreezePosition;

        // 启动发电进程
        StartGenerate();
    }

    private void StartGenerate()
    {
        inRotation = ball.transform.rotation;
        inverseRotation = Quaternion.Inverse(inRotation);
        //Debug.Log("InRotation: "+inRotation.eulerAngles+"   Inversed: "+inverseRotation.eulerAngles);
        //rotateSignal.transform.rotation = Quaternion.FromToRotation(rotateSignal.transform.rotation.eulerAngles, ball.transform.rotation.eulerAngles);
        rotateSignal.gameObject.SetActive(true);
        roundNow = 0;
        hasRotated = false;
        hasBallIn = true;
    }

    private void ElectricityGenerate()
    {
        //Debug.Log("Rotation of ball: " + ball.transform.rotation + "; --- In Rotation: " + inRotation);
        // 旋转到反向位置，认为旋转有效
        if (Mathf.Abs((ball.transform.rotation.eulerAngles - inverseRotation.eulerAngles).z) <= angleBias)
        {
            hasRotated = true;
            //Debug.Log("InverseRotation!");
        }
        // 发电判定
        if (Mathf.Abs((ball.transform.rotation.eulerAngles - inRotation.eulerAngles).z) <= angleBias && hasRotated)
        //&& Mathf.Abs(ball.GetComponent<Rigidbody2D>().angularVelocity) >= generateVelocity)
        {
            roundNow++;
            Debug.Log(name + ": Round: " + roundNow + " Finished.");
            if (roundNow < generateRound) AudioPlay(1);
            else AudioPlay(2);
            hasRotated = false;
        }
        // 成功发电时使得连接的电器开或关
        if (roundNow >= generateRound)
        {
            Debug.Log(name + ": Electricity Generated!");
            objectActivated = true;
            //if (objectActivated) objectActivated = false;
            //else objectActivated = true;
            UpdateActivatedStatus();
            roundNow = 0;
            hasRotated = false;
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "Ball" && !hasBallIn && !exiting)
    //    {
    //        Debug.Log(name + ": Ball Detected!");
    //        ball = collision.gameObject;
    //        hasBallIn = true;

    //        // 位于电梯上的发电机自动暂停电梯
    //        if (transform.parent.GetComponent<LiftController>())
    //        {
    //            transform.parent.GetComponent<LiftController>().StopMove();
    //            if (connectedObject.GetComponent<LiftController>()) GetComponent<SpriteRenderer>().sprite = darkSprite;
    //        }

    //        //collision.GetComponent<Rigidbody2D>().MovePosition(transform.position);
    //        StartCoroutine(nameof(BallAbsorb), collision.gameObject);
    //    }
    //}

    //// 在球到达发电机时自动吸附球
    //private IEnumerator BallAbsorb(GameObject ball)
    //{
    //    while ((ball.transform.position - centerPoint.position).magnitude > 0.1f)
    //    {
    //        Vector2 absorbForce = (centerPoint.position - ball.transform.position) * 10.0f;
    //        ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);
    //        yield return new WaitForEndOfFrame();
    //    }
    //    ball.transform.position = centerPoint.position;
    //    ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
    //    StartCoroutine(nameof(WaitForLock), ball);
    //    Debug.Log(name + ": Ball Locked!");
    //}

    // 判定球离开前等待的时间
    private IEnumerator WaitForLock(GameObject ball)
    {
        yield return new WaitForSeconds(exitTime);
        StartCoroutine(nameof(BallControl), ball);
    }

    // 在锁定球后控制其水平位置，同时也判断仓鼠是否试图挣脱
    private IEnumerator BallControl(GameObject ball)
    {
        AudioPlay(0);
        // 置于电梯上的发电机不锁定仓鼠球Y轴
        if (transform.parent.GetComponent<LiftController>())
            ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        else ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;
        inRotation = ball.transform.rotation;
        inverseRotation = Quaternion.Inverse(inRotation);
        //Debug.Log("InRotation: "+inRotation.eulerAngles+"   Inversed: "+inverseRotation.eulerAngles);
        //rotateSignal.transform.rotation = Quaternion.FromToRotation(rotateSignal.transform.rotation.eulerAngles, ball.transform.rotation.eulerAngles);
        rotateSignal.gameObject.SetActive(true);
        roundNow = 0;
        hasRotated = false;
        Vector3 distance;
        while (hasBallIn)
        {
            // 离开判定
            distance = centerPoint.position - ball.transform.position;
            //Debug.Log(distance.magnitude);
            if (distance.magnitude >= exitDistance)
            {
                Debug.Log(name + ": Ball Let Go!");
                exiting = true;
                StopCoroutine(nameof(WaitForExit));
                StartCoroutine(nameof(WaitForExit));
                ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                hasBallIn = false; break;
            }

            // 位置维持
            Vector2 absorbForce = (distance) * 60.0f;
            ball.GetComponent<Rigidbody2D>().AddForce(absorbForce);

            //Debug.Log("Rotation of ball: " + ball.transform.rotation + "; --- In Rotation: " + inRotation);
            // 旋转到反向位置，认为旋转有效
            if(Mathf.Abs((ball.transform.rotation.eulerAngles - inverseRotation.eulerAngles).z) <= angleBias)
            {
                hasRotated = true;
                //Debug.Log("InverseRotation!");
            }
            // 发电判定
            if (Mathf.Abs((ball.transform.rotation.eulerAngles - inRotation.eulerAngles).z) <= angleBias && hasRotated)
                //&& Mathf.Abs(ball.GetComponent<Rigidbody2D>().angularVelocity) >= generateVelocity)
            {
                roundNow++;
                Debug.Log(name + ": Round: " + roundNow + " Finished.");
                if (roundNow < generateRound) AudioPlay(1);
                else AudioPlay(2);
                hasRotated = false;
            }
            // 成功发电时使得连接的电器开或关
            if (roundNow >= generateRound)
            {
                Debug.Log(name + ": Electricity Generated!");
                objectActivated = true;
                //if (objectActivated) objectActivated = false;
                //else objectActivated = true;
                UpdateActivatedStatus();
                roundNow = 0;
                hasRotated = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    // 球离开后重新判定球进入前的等待时间
    private IEnumerator WaitForExit()
    {
        rotateSignal.SetActive(false);
        yield return new WaitForSeconds(returnTime);
        exiting = false;
    }

    private void AudioPlay(int i)
    {
        audioSource.clip = clips[i];
        audioSource.Play();
    }

}
