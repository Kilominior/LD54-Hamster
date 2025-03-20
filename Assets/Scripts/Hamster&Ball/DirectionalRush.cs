using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class DirectionalRush : MonoBehaviour
{
    // 玩家
    private MouseController player;

    // 冲撞是否正在冷却
    public bool isRushCooling;
    // 冲撞的冷却时间
    [SerializeField]
    private float rushCD = 0.5f;
    // 冲撞冷却倒计时
    private WaitForSeconds WaitForRushCD;

    // 方向向量
    private Vector3 _dirVector;
    private Vector3 dirVector
    {
        get { return _dirVector; }
        set
        {
            _dirVector = value;
            if (_dirVector.magnitude > dirLength)
            {
                _dirVector = _dirVector.normalized * dirLength;
            }
        }
    }

    // 每帧方向的变化量
    private Vector3 deltaDirVector;

    // 方向线条的最大长度
    [SerializeField]
    private float dirLength = 3.0f;

    // 方向线条的起点和终点
    private Vector3 dirBeginPos;
    private Vector3 dirEndPos;

    // 当前正在瞄准中
    private bool isAiming;

    private LineRenderer dirRenderer;

    void Start()
    {
        player = transform.parent.GetComponent<MouseController>();
        dirRenderer = GetComponent<LineRenderer>();
        WaitForRushCD = new WaitForSeconds(rushCD);

        Initialize();
    }

    private void Initialize()
    {
        EndAiming(true);
        isRushCooling = false;
    }

    void Update()
    {
        AimUpdate();
    }

    // 开始瞄准
    private void StartAiming()
    {
        if (isRushCooling) { return; }
        isAiming = true;
        dirRenderer.enabled = true;
        //GetMousePosition(out mousePosInit);
        dirVector = Vector2.zero;
        deltaDirVector = Vector2.zero;
        TimeManager.Instance.ExecuteTimeSlowDown();
    }

    // 瞄准中
    /// <summary>
    /// 通过指针的位置变化量决定当前的瞄准方向，输入的vector应为指针的变化原始值
    /// </summary>
    private void AimByDelta(Vector2 delta)
    {
        if (!isAiming) return;
        // 计算并更新当前的方向向量
        deltaDirVector = delta;
        dirVector += deltaDirVector / 5.0f;
        // dirVector += deltaDirVector;
        // Debug.Log("delta: " + deltaDirVector + ", DirVector: " + dirVector);
    }

    /// <summary>
    /// 通过推动摇杆的距离决定当前的瞄准方向，输入的vector应为0~1之间的值
    /// </summary>
    private void AimByPushAmount(Vector2 amount)
    {
        if (!isAiming) return;
        dirVector = amount * dirLength;
    }

    // 结束瞄准
    private void EndAiming(bool canceled = false)
    {
        isAiming = false;
        TimeManager.Instance.RecoverTimeScale();
        dirRenderer.enabled = false;

        if (canceled) return;
        ExecuteRush();
    }

    private void ExecuteRush()
    {
        // 若长度过短判定为不冲撞
        if (dirVector.magnitude <= 0.01f) return;
        // 进行冲撞并启动冷却
        isRushCooling = true;
        player.Rush(dirVector / dirLength);
        dirVector = Vector3.zero;
        StartCoroutine(nameof(rushCoolDown));
    }

    private IEnumerator rushCoolDown()
    {
        yield return WaitForRushCD;
        isRushCooling = false;
    }

    private void AimUpdate()
    {
        if (!isAiming) return;
        // 更新当前瞄准线的起点和终点
        dirBeginPos = player.transform.position;
        dirEndPos = dirBeginPos + dirVector;

        dirRenderer.SetPosition(0, dirBeginPos);
        dirRenderer.SetPosition(1, dirEndPos);
    }

    public void OnAimStart()
    {
        StartAiming();
    }

    public void OnAimByDelta(Vector2 delta)
    {
        AimByDelta(delta);
    }

    public void OnAimByPushAmount(Vector2 amount)
    {
        AimByPushAmount(amount);
    }

    public void OnAimEnd()
    {
        EndAiming();
    }
}
