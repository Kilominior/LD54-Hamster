

using System.Collections;
using UnityEngine;

/// <summary>
/// 可交互接口，所有玩家可进行交互的游戏物体都需继承此接口
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 开始接受交互
    /// </summary>
    public abstract void EnableInteract();

    /// <summary>
    /// 停止接受交互
    /// </summary>
    public abstract void DisableInteract();

    /// <summary>
    /// 执行交互操作
    /// </summary>
    public abstract void ExecuteInteract(MouseController player);
}

/// <summary>
/// 允许鼠球挂载至的物体。
/// 第一次交互时鼠球将挂载到本物体上，与本物体同步运动；
/// 再次交互时鼠球将与本物体脱离。
/// </summary>
public class BallMountable : MonoBehaviour, IInteractable
{
    protected MouseController hamster;
    protected BallController ball;
    protected Rigidbody2D hrb;
    protected Rigidbody2D brb;

    [SerializeField]
    protected Transform mountPoint;

    [SerializeField]
    protected float mountSpeed = 10.0f;
    protected Vector2 pullForce;

    public virtual void EnableInteract()
    {

    }

    public virtual void DisableInteract()
    {

    }

    public virtual void ExecuteInteract(MouseController player)
    {
        // 再次确认，非鼠球状态不交互
        if (player.currentState == MouseController.PlayerState.Hamster)
        {
            return;
        }

        // 获取鼠、球本身及其刚体
        hamster = player;
        hrb = hamster.GetComponent<Rigidbody2D>();
        ball = player.ball;
        brb = ball.GetComponent<Rigidbody2D>();

        // 进行挂载
        MountBall();
    }

    // 将鼠球和鼠鼠固定到挂载点
    protected virtual void MountBall()
    {
        ball.transform.SetParent(transform);
        // 对球壳施加朝向挂载点的拉力，直到其位置准确
        brb.velocity = Vector2.zero;
        StartCoroutine(nameof(PullBall));
        //hrb.velocity = Vector2.zero;
        //hamster.transform.position = mountPoint.position;

        //brb.velocity = Vector2.zero;
        //brb.bodyType = RigidbodyType2D.Kinematic;
        //ball.transform.position = mountPoint.position;

        //brb.bodyType = RigidbodyType2D.Dynamic;
        //brb.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    private IEnumerator PullBall()
    {
        while ((ball.transform.position - mountPoint.position).magnitude > 0.1f)
        {
            pullForce = (mountPoint.position - ball.transform.position) * mountSpeed;
            brb.AddForce(pullForce, ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
        ball.transform.position = mountPoint.position;
        brb.bodyType = RigidbodyType2D.Dynamic;
        ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
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