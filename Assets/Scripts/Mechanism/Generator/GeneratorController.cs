using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : BallMountable
{
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

        if (HasObjectMounted())
        {
            Generate();
        }
    }

    protected virtual void Initialize()
    {
        mountAccessState = MountAccessState.MountDenied;
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
        SwitchCameraToTargetGroup();

        StartGenerate();
    }

    protected override void DismountBall()
    {
        EndGenerate();
        CameraManager.instance?.SwitchVCameraBack();

        base.DismountBall();
    }

    protected virtual void SwitchCameraToTargetGroup()
    {
        if (CameraManager.instance != null)
        {
            if (CameraManager.instance.IsTargetGroupRegistered(this))
            {
                CameraManager.instance.SwitchVCameraTo(this);
            }
            else
            {
                // 实例化机制相机
                CameraManager.instance.RegisterTargetGroup(this, transform, connectedObject.transform);
                CameraManager.instance.SwitchVCameraTo(this);
            }
        }
    }

    protected void AudioPlay(int i)
    {
        audioSource.clip = clips[i];
        audioSource.Play();
    }
}
