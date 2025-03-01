using System.Collections;
using UnityEngine;

/// <summary>
/// 可挂载对象的挂载许可状态。
/// </summary>
public enum MountAccessState
{
    // 当鼠/球未挂载上时
    MountGranted,
    MountDenied,
    // 当鼠鼠/球处于挂载过程中时
    Mounting,
    // 当鼠/球已挂载上时
    DetachGranted,
    DetachDenied
}

/// <summary>
/// 允许鼠球挂载至的物体。
/// 第一次交互时鼠球将挂载到本物体上，与本物体同步运动；
/// 再次交互时鼠球将与本物体脱离。
/// </summary>
public class BallMountable : MonoBehaviour, IInteractable, ITargetGroup
{
    public static readonly InteractType interactType = InteractType.Pair;

    protected MountAccessState mountAccessState;

    protected MouseController hamster;
    protected BallController ball;
    protected Rigidbody2D hrb;
    protected Rigidbody2D brb;

    // 挂载点
    [SerializeField]
    protected Transform mountPoint;

    // 将鼠球吸附到挂载点的速度
    [SerializeField]
    protected float mountSpeed = 0.2f;
    protected Vector3 pullVector;

    // 交互的按键提示图，包括进入和离开两种
    [SerializeField]
    private ControlSchemeHint outsideInteractHint;
    [SerializeField]
    private ControlSchemeHint insideInteractHint;

    private WaitForFixedUpdate mountBallIE;

    public virtual void EnableInteract(MouseController player)
    {
        // 当鼠球正处于挂载状态或临界状态时，不允许改变交互状态
        if (HasObjectMounted()) return;
        if (mountAccessState == MountAccessState.Mounting) return;
        if (player.currentState == PlayerState.Hamster) return;

        mountAccessState = MountAccessState.MountGranted;
        ShowOutsideHint();
    }

    public virtual void DisableInteract()
    {
        // 当鼠球正处于挂载状态或临界状态时，不允许改变交互状态
        if (HasObjectMounted()) return;
        if (mountAccessState == MountAccessState.Mounting) return;

        mountAccessState = MountAccessState.MountDenied;
        HideHints();
    }

    public virtual void ExecuteInteract(MouseController player)
    {
        if (mountAccessState == MountAccessState.MountGranted)
        {
            // 获取鼠、球本身及其刚体
            hamster = player;
            hrb = hamster.GetComponent<Rigidbody2D>();
            ball = player.ball;
            brb = ball.GetComponent<Rigidbody2D>();

            ShowInsideHint();

            // 进行挂载
            MountBall();
        }
        else if (mountAccessState == MountAccessState.DetachGranted)
        {
            ShowOutsideHint();

            // 进行卸载
            DismountBall();
        }
    }

    // 将鼠球固定到挂载点
    protected virtual void MountBall()
    {
        // 设置当前状态为挂载中
        mountAccessState = MountAccessState.Mounting;

        // 设置球壳的父物体为挂载点
        ball.transform.SetParent(mountPoint);

        // 停止球壳原有的运动状态并启动挂载
        brb.velocity = Vector2.zero;
        brb.bodyType = RigidbodyType2D.Kinematic;

        mountBallIE = new WaitForFixedUpdate();
        StartCoroutine(nameof(PullBall));
    }

    // 将鼠球从挂载点卸载
    protected virtual void DismountBall()
    {
        // 恢复球壳无父物体的状态
        ball.transform.SetParent(null);

        // 停止对球壳运动的限制
        brb.constraints = RigidbodyConstraints2D.None;

        // 确认鼠球已卸载完毕，允许鼠球重新挂载
        mountAccessState = MountAccessState.MountGranted;
    }

    // 对球壳施加朝向挂载点的移动，直到其位置准确
    private IEnumerator PullBall()
    {
        // 持续进行移动
        // TODO: 改为Lerp插值运动？
        while ((ball.transform.position - mountPoint.position).magnitude > 0.1f)
        {
            pullVector = (mountPoint.position - ball.transform.position) * mountSpeed;
            ball.transform.position += pullVector;
            hamster.transform.position += pullVector;
            yield return mountBallIE;
        }

        // 恢复球壳的旋转能力
        ball.transform.position = mountPoint.position;
        brb.bodyType = RigidbodyType2D.Dynamic;
        brb.constraints = RigidbodyConstraints2D.FreezePosition;

        // 确认球壳已挂载完毕，允许鼠球卸载
        mountAccessState = MountAccessState.DetachGranted;
    }

    // 展示进入按键提示
    protected void ShowOutsideHint()
    {
        outsideInteractHint.Show();
        insideInteractHint.Hide();
    }

    // 显示离开按键提示
    protected void ShowInsideHint()
    {
        outsideInteractHint.Hide();
        insideInteractHint.Show();
    }

    // 隐藏所有按键提示
    protected void HideHints()
    {
        outsideInteractHint.Hide();
        insideInteractHint.Hide();
    }

    /// <summary>
    /// 鼠/球是否处于挂载状态
    /// </summary>
    protected bool HasObjectMounted()
    {
        return mountAccessState == MountAccessState.DetachGranted || mountAccessState == MountAccessState.DetachDenied;
    }
}