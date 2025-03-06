using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ElectricGeneratorController : GeneratorController
{
    // 计算对角线角度所需的四元数
    private static readonly Quaternion halfTurn = Quaternion.Euler(0f, 0f, 180f);

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
    private Quaternion reverseRotation;
    // 曾经到达对角处，此次旋转合法
    private bool hasRotated;
    // 判定旋转一周的角度偏移量
    public float angleBias = 1f;

    private SpriteRenderer sr;

    protected override void Initialize()
    {
        base.Initialize();
        sr = GetComponent<SpriteRenderer>();
        roundNow = 0;

        rotateSignal = transform.Find("RotateSignal").gameObject;
        rotateSignal.SetActive(false);

        UpdateActivatedStatus(false);
    }

    // 更新自身的提示灯亮暗以及连接电器的开关状态
    private void UpdateActivatedStatus(bool activated)
    {
        // TODO 更新线条材质
        if (activated)
        {
            sr.sprite = lightSprite;
            connectedObject.Trigger();
        }
        else
        {
            sr.sprite = darkSprite;
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
        reverseRotation = inRotation * halfTurn;
        // Debug.Log("InRotation: "+ inRotation.eulerAngles+"   Inversed: "+ this.reverseRotation.eulerAngles);
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
        if (Mathf.Abs((ball.transform.rotation.eulerAngles - reverseRotation.eulerAngles).z) <= angleBias)
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
            UpdateActivatedStatus(true);
            roundNow = 0;
            hasRotated = false;
        }
    }
}
