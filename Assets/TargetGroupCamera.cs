using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TargetGroupCamera : MonoBehaviour
{

    public CinemachineTargetGroup targetGroup;

    public void AddTarget(GameObject target)
    {
        targetGroup.AddMember(target.transform, 1, 2);
    }
}
