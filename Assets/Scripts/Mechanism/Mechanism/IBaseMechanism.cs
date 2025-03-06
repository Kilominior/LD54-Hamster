
public interface IBaseMechanism
{


}

/// <summary>
/// 电动机关，能够与电池或发电机相连，并消耗电力运转
/// </summary>
public interface IElectricMechanism: IBaseMechanism
{

}

/// <summary>
/// 触发机关，可被某种方式一次性触发
/// </summary>
public interface ITriggerMechanism: IBaseMechanism
{
    /// <summary>
    /// 触发
    /// </summary>
    public void Trigger();
}

/// <summary>
/// 切换机关，能够接收开/关信号并做出反应
/// </summary>
public interface ISwitchMechanism: IBaseMechanism
{
    /// <summary>
    /// 切换至开启状态
    /// </summary>
    public void TurnOn();

    /// <summary>
    /// 切换至关闭状态
    /// </summary>
    public void TurnOff();
}

/// <summary>
/// 可调节机关，能够获取鼠球进入后的旋转情况并做出反应
/// </summary>
public interface IAdjustableMechanism: IBaseMechanism
{
    /// <summary>
    /// 当鼠球在机关中进行旋转时，更新机关的状态
    /// </summary>
    /// <param name="deltaDegree">鼠球当前角度的变化</param>
    public void OnAdjust(float deltaDegree);
}