using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : BallMountable
{
    // Mechanism Camera
    public GameObject targetGroupCamera;
    public GameObject targetGroupCameraPrefab;

    // 与本发电机相连的物体
    public GameObject connectedObject;

    // 鼠球锁定时的旋转
    protected Quaternion inRotation;

    // 连接动力机与其操控物体的连线
    // 连线的节点
    protected List<Transform> LinePoints;
    // 连线节点父物体
    protected Transform pointParent;
    // 连线绘制器
    protected LineRenderer lineRenderer;


    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip[] clips;

    protected virtual void Start()
    {
        BindLine();
        audioSource = GetComponent<AudioSource>();

        Initialize();
    }

    protected virtual void Update()
    {
        UpdateLine();

        if (hasBallMounted)
        {
            Generate();
        }
    }

    protected virtual void Initialize()
    {
        hasBallMounted = false;
    }

    // 初始化连线
    protected void BindLine()
    {
        pointParent = transform.Find("PointParent");
        lineRenderer = GetComponent<LineRenderer>();

        // 初始化连线的每个节点，并绘制
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

    protected void UpdateLine()
    {
        // 更新所有电线节点状态
        for (int i = 0; i < LinePoints.Count; i++)
        {
            lineRenderer.SetPosition(i, LinePoints[i].position);
        }
    }


    // 启动驱动进程
    protected virtual void StartGenerate()
    {

    }

    // 结束驱动进程
    protected virtual void EndGenerate()
    {

    }

    // 执行驱动操作
    protected virtual void Generate()
    {

    }


    protected override void MountBall()
    {
        base.MountBall();

        if (targetGroupCamera != null)
        {
            CameraManager.instance.SwitchVCameraTo(targetGroupCamera);
        }
        else
        {
            // 实例化机制相机
            targetGroupCamera = Instantiate(targetGroupCameraPrefab, transform.position, Quaternion.identity);
            TargetGroupCamera targetGroupCameraScript = targetGroupCamera.GetComponent<TargetGroupCamera>();
            targetGroupCameraScript.AddTarget(gameObject);
            targetGroupCameraScript.AddTarget(connectedObject);
            CameraManager.instance.SwitchVCameraTo(targetGroupCamera);
        }

        StartGenerate();
    }

    protected override void DismountBall()
    {
        EndGenerate();

        if (targetGroupCamera != null)
        {
            CameraManager.instance.SwitchVCameraBack();
        }

        base.DismountBall();
    }

    public override void EnableInteract()
    {
        base.EnableInteract();
    }

    public override void DisableInteract()
    {
        base.DisableInteract();
    }

    public override bool ExecuteInteract(MouseController player)
    {
        return base.ExecuteInteract(player);
    }

    protected void AudioPlay(int i)
    {
        audioSource.clip = clips[i];
        audioSource.Play();
    }
}
