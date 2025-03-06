using UnityEngine;

/// <summary>
/// 仅支持单次触发的机关
/// </summary>
public abstract class TriggerMechanism : MonoBehaviour, ITriggerMechanism
{
    private bool _triggered = false;
    public bool triggered
    {
        get { return _triggered; }
        private set { _triggered = value; }
    }

    /// <summary>
    /// 尝试执行Trigger操作，会判断触发次数是否超标，确保执行次数
    /// </summary>
    public virtual void Trigger()
    {
        // 触发机关为一次性操作
        if (triggered) return;
        triggered = true;
        ExecuteTrigger();
    }

    /// <summary>
    /// 执行Trigger所触发的具体业务逻辑
    /// </summary>
    protected abstract void ExecuteTrigger();
}
