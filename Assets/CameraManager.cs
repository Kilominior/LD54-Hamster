using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetGroup
{
}

public class CameraManager : MonoBehaviour
{
    public GameObject mainCamera;
    public static CameraManager instance;

    // 目标组与相机的字典
    private Dictionary<ITargetGroup, GameObject> targetGroupCameraDict;
    [SerializeField]
    private GameObject targetGroupCameraPrefab;

    private GameObject currentCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        targetGroupCameraDict = new Dictionary<ITargetGroup, GameObject>();
    }

    /// <summary>
    /// 查询某个目标组是否在字典中被注册，若未注册须手动创建
    /// </summary>
    public bool IsTargetGroupRegistered(ITargetGroup targetGroup)
    {
        return targetGroupCameraDict.ContainsKey(targetGroup);
    }

    /// <summary>
    /// 传入目标组及其目标的Transform列表，即可将目标组注册到字典中，允许切换到其对应的相机
    /// </summary>
    public void RegisterTargetGroup(ITargetGroup targetGroup, params Transform[] trans)
    {
        var cameraObj = Instantiate(targetGroupCameraPrefab, transform.position, Quaternion.identity);
        TargetGroupCamera targetGroupCamera = cameraObj.GetComponent<TargetGroupCamera>();
        for(int i = 0; i < trans.Length; i++)
        {
            targetGroupCamera.AddTarget(trans[i]);
        }
        targetGroupCameraDict[targetGroup] = cameraObj;
    }

    /// <summary>
    /// 更新当前使用的相机
    /// </summary>
    public void SwitchVCameraTo(ITargetGroup targetGroup)
    {
        if (mainCamera == null)
        {
            return;
        }

        mainCamera.SetActive(false);
        targetGroupCameraDict[targetGroup].SetActive(true);
        currentCamera = targetGroupCameraDict[targetGroup];
    }

    /// <summary>
    /// 恢复使用主相机
    /// </summary>
    public void SwitchVCameraBack()
    {
        if (mainCamera == null)
        {
            return;
        }
        if(mainCamera.activeSelf)
        {
            return;
        }

        mainCamera.SetActive(true);
        currentCamera.SetActive(false);
        currentCamera = mainCamera;
    }
}
