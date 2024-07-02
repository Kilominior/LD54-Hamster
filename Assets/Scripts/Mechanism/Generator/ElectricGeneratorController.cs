using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ElectricGeneratorController : GeneratorController
{
    // 物体开关状态
    public bool objectActivated;
    // 没电贴图
    public Sprite darkSprite;
    // 有电贴图
    public Sprite lightSprite;

    // 旋转目标指示器
    private GameObject rotateSignal;

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

    protected override void Start()
    {
        base.Start();

        if (objectActivated) GetComponent<SpriteRenderer>().sprite = lightSprite;
        else GetComponent<SpriteRenderer>().sprite = darkSprite;

        rotateSignal = transform.Find("RotateSignal").gameObject;
        rotateSignal.SetActive(false);
    }

    protected override void Initialize()
    {
        base.Initialize();

        UpdateActivatedStatus();
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

    protected override void MountBall()
    {
        base.MountBall();
        AudioPlay(0);
    }

    protected override void StartGenerate()
    {
        inRotation = ball.transform.rotation;
        inverseRotation = Quaternion.Inverse(inRotation);
        //Debug.Log("InRotation: "+inRotation.eulerAngles+"   Inversed: "+inverseRotation.eulerAngles);
        //rotateSignal.transform.rotation = Quaternion.FromToRotation(rotateSignal.transform.rotation.eulerAngles, ball.transform.rotation.eulerAngles);
        rotateSignal.gameObject.SetActive(true);
        roundNow = 0;
        hasRotated = false;
    }

    protected override void EndGenerate()
    {
        rotateSignal.gameObject.SetActive(false);
    }

    protected override void Generate()
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
        {
            roundNow++;
            //Debug.Log(name + ": Round: " + roundNow + " Finished.");
            if (roundNow < generateRound) AudioPlay(1);
            else AudioPlay(2);
            hasRotated = false;
        }
        // 成功发电时使得连接的电器开或关
        if (roundNow >= generateRound)
        {
            //Debug.Log(name + ": Electricity Generated!");
            objectActivated = true;
            UpdateActivatedStatus();
            roundNow = 0;
            hasRotated = false;
        }
    }
}
