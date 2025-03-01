/// <summary>
/// 可交互接口，所有玩家可进行交互的游戏物体都需继承此接口
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 开始接受交互。可交互对象可根据玩家当前状态进行交互的准备。
    /// </summary>
    /// <param name="player">玩家</param>
    public abstract void EnableInteract(MouseController player);

    /// <summary>
    /// 停止接受交互。
    /// </summary>
    public abstract void DisableInteract();

    /// <summary>
    /// 执行交互操作。
    /// </summary>
    /// <param name="player">玩家</param>
    public abstract void ExecuteInteract(MouseController player);
}

/// <summary>
/// 具有次要交互键响应能力的接口，
/// </summary>
public interface ISubInteractable
{
    /// <summary>
    /// 执行次要交互操作。
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns>是否允许再次交互</returns>
    public abstract void ExecuteSubInteract(MouseController player);
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
/// 允许鼠鼠进入的物体。
/// 第一次交互时鼠鼠将进入本物体，与本物体同步运动；
/// 再次交互时鼠鼠将与本物体脱离。
/// </summary>
public class HamsterEnterable
{

}