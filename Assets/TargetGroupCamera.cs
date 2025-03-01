using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TargetGroupCamera : MonoBehaviour
{

    public CinemachineTargetGroup targetGroup;

    public void AddTarget(Transform target)
    {
        targetGroup.AddMember(target, 1, 2);
    }
}
