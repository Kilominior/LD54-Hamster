using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float MAXHP = 100f;      // 最大生命值
    public float HP;                // 当前生命值


    void Start()
    {
        HP = MAXHP;
    }


}
