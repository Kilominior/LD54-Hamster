using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlScheme
{
    None,
    Keyboard,
    Gamepad
}

public class ControlManager : SingletonInSceneDontDestroy<ControlManager>
{
    private static readonly string KeyboardSchemeName = "Keyboard&Mouse";
    private static readonly string GamepadSchemeName = "Gamepad";

    private ControlScheme currentScheme = ControlScheme.None;

    // 在当前控制器变化时调用，更新现在的操作设备
    public void OnControlsUpdate(PlayerInput input)
    {
        //Debug.Log("Controls update to " + input.currentControlScheme);
        if (input.currentControlScheme == KeyboardSchemeName)
        {
            currentScheme = ControlScheme.Keyboard;
        }
        else if (input.currentControlScheme == GamepadSchemeName)
        {
            currentScheme = ControlScheme.Gamepad;
        }
        else
        {
            return;
        }

        TypeEventSystem.Global.Send(new ControlSchemeChangeEvent(currentScheme));
    }

    /// <summary>
    /// 场景加载时进行操作布局的初始化，优先沿用现有的布局
    /// </summary>
    public void InitControlScheme(PlayerInput input)
    {
        // Debug.Log("Init Control Scheme by " + input.currentControlScheme);
        if(currentScheme == ControlScheme.None)
        {
            OnControlsUpdate(input);
            return;
        }
        TypeEventSystem.Global.Send(new ControlSchemeChangeEvent(currentScheme));
    }
}
