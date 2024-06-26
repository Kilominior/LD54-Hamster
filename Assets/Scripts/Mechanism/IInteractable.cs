

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
