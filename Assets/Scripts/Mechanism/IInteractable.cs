using System.Collections;
using UnityEngine;

/// <summary>
/// 可交互接口，所有玩家可进行交互的游戏物体都需继承此接口
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 尝试做出交互。
    /// 将根据玩家的信息判断是否允许此次交互，
    /// 若交互合法则开始接受交互，即执行<see cref="EnableInteract"/>
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>是否允许交互</returns>
    public abstract bool TryEnableInteract(MouseController player);

    /// <summary>
    /// 开始接受交互。
    /// 非必要不应调用此方法，此方法将罔顾玩家状态直接允许交互。
    /// 请调用<see cref="TryEnableInteract"/>
    /// </summary>
    public abstract void EnableInteract();

    /// <summary>
    /// 停止接受交互。
    /// </summary>
    public abstract void DisableInteract();

    /// <summary>
    /// 执行交互操作。
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>是否允许再次交互</returns>
    public abstract bool ExecuteInteract(MouseController player);
}

/// <summary>
/// 可交互物体的交互类型。
/// 分为单次（如拾取道具）、成对（如进出入口）和多次（如触发对话）
/// </summary>
public enum InteractType
{
    Single,
    Pair,
    Multiple
}

/// <summary>
/// 允许鼠球挂载至的物体。
/// 第一次交互时鼠球将挂载到本物体上，与本物体同步运动；
/// 再次交互时鼠球将与本物体脱离。
/// </summary>
public class BallMountable : MonoBehaviour, IInteractable
{
    public static readonly InteractType interactType = InteractType.Pair;

    protected MouseController hamster;
    protected BallController ball;
    protected Rigidbody2D hrb;
    protected Rigidbody2D brb;

    // 鼠球的此次交互是否是试图挂载到本物体上
    // 若否，则为试图卸载
    protected bool isBallMounting;

    // 当前是否正有鼠球被挂载在本物体上
    protected bool hasBallMounted;

    // 挂载点
    [SerializeField]
    protected Transform mountPoint;

    // 将鼠球吸附到挂载点的速度
    [SerializeField]
    protected float mountSpeed = 0.2f;
    protected Vector3 pullVector;

    private WaitForFixedUpdate mountBallIE;

    public virtual bool TryEnableInteract(MouseController player)
    {
        if(player.currentState == PlayerState.Ball)
        {
            EnableInteract();
            return true;
        }
        return false;
    }

    public virtual void EnableInteract()
    {
        isBallMounting = true;
    }

    public virtual void DisableInteract()
    {

    }

    public virtual bool ExecuteInteract(MouseController player)
    {
        if (isBallMounting)
        {
            // 获取鼠、球本身及其刚体
            hamster = player;
            hrb = hamster.GetComponent<Rigidbody2D>();
            ball = player.ball;
            brb = ball.GetComponent<Rigidbody2D>();

            // 下一次操作应为卸载
            isBallMounting = false;

            // 进行挂载
            MountBall();
        }
        else
        {
            // 下一次操作应为挂载
            isBallMounting = true;

            // 进行卸载
            DismountBall();
        }
        return true;
    }

    // 将鼠球固定到挂载点
    protected virtual void MountBall()
    {
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

        // 确认鼠球已卸载完毕
        hasBallMounted = false;
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
            yield return mountBallIE;
        }

        // 恢复球壳的旋转能力
        ball.transform.position = mountPoint.position;
        brb.bodyType = RigidbodyType2D.Dynamic;
        brb.constraints = RigidbodyConstraints2D.FreezePosition;

        // 确认球壳已挂载完毕
        hasBallMounted = true;
    }
}

/// <summary>
/// 允许鼠鼠进入的物体。
/// 第一次交互时鼠鼠将进入本物体，与本物体同步运动；
/// 再次交互时鼠鼠将与本物体脱离。
/// </summary>
public class HamsterEnterable
{

}