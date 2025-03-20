using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlScheme
{
    None,
    Keyboard,
    Gamepad,
    Touchscreen
}

public static class ControlManager
{
    private static readonly string KeyboardSchemeName = "Keyboard&Mouse";
    private static readonly string GamepadSchemeName = "Gamepad";
    private static readonly string TouchscreenSchemeName = "Touch";
    private static ControlScheme _currentScheme = ControlScheme.None;
    public static ControlScheme currentScheme
    {
        get { return _currentScheme; }
        private set { _currentScheme = value; }
    }

    // 在当前控制器变化时调用，更新现在的操作设备
    public static void OnControlsUpdate(PlayerInput input)
    {
        // Debug.Log("Controls update to " + input.currentControlScheme);
#if UNITY_ANDROID || Unity_IOS
        // 移动平台上仅支持显示触摸屏交互提示
        if (currentScheme != ControlScheme.Touchscreen)
        {
            currentScheme = ControlScheme.Touchscreen;
            TypeEventSystem.Global.Send(new ControlSchemeChangeEvent(currentScheme));
        }
        // 锁定触摸布局，并禁止PlayerInput自动切换布局
        // UpdatePlayerInputByCurrentScheme(input);
        // input.neverAutoSwitchControlSchemes = true;
#else
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
#endif
    }

    /// <summary>
    /// 场景加载时进行操作布局的初始化，优先沿用现有的布局
    /// </summary>
    public static void InitControlScheme(PlayerInput input)
    {
        // Debug.Log("Init Control Scheme by player input: " + input.currentControlScheme + ", control manager: " + currentScheme);
        // 若初次进行初始化，则获取当前player input布局并广播
        if(currentScheme == ControlScheme.None)
        {
            OnControlsUpdate(input);
            return;
        }
        // 否则，强制修改player input布局，使其与记录的布局一致
        UpdatePlayerInputByCurrentScheme(input);
        TypeEventSystem.Global.Send(new ControlSchemeChangeEvent(currentScheme));
    }

    private static void UpdatePlayerInputByCurrentScheme(PlayerInput input)
    {
        if (currentScheme == ControlScheme.Keyboard)
        {
            input.SwitchCurrentControlScheme(KeyboardSchemeName);
        }
        else if (currentScheme == ControlScheme.Gamepad)
        {
            input.SwitchCurrentControlScheme(GamepadSchemeName);
        }
        else if(currentScheme == ControlScheme.Touchscreen)
        {
            input.SwitchCurrentControlScheme(TouchscreenSchemeName);
        }
    }
}
