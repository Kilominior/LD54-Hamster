public class ControlSchemeChangeEvent
{
    public ControlScheme currentControlScheme;

    public ControlSchemeChangeEvent(ControlScheme currentControlScheme)
    {
        this.currentControlScheme = currentControlScheme;
    }
}

/// <summary>
/// 交互按键的ControlHint被展示时的事件，接收者可根据Hint子物体的数量判定当前支持的交互操作数量
/// </summary>
public class ShowInteractHintEvent
{
    /// <summary>
    /// 是否同时支持主要交互和次要交互
    /// </summary>
    public ControlSchemeHint hint;

    public ShowInteractHintEvent(ControlSchemeHint hint)
    {
        this.hint = hint;
    }
}

/// <summary>
/// 交互按键的ControlHint被隐藏时的事件
/// </summary>
public class HideInteractHintEvent
{
}