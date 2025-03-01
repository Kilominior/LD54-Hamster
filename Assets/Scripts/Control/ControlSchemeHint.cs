using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class ControlSchemeHint : MonoBehaviour
{
    private CanvasGroup cg;
    // 不同键盘布局对应不同的提示CanvasGroup
    private List<CanvasGroup> schemeHintsList;

    [Tooltip("是否在游戏开始运行时默认显示")]
    [SerializeField]
    private bool showAtRuntime = false;

    void Awake()
    {
        EventRegister();
        cg = GetComponent<CanvasGroup>();
        if(showAtRuntime) Show();
        else Hide();
        InitHints();
    }

    private void InitHints()
    {
        schemeHintsList = new List<CanvasGroup>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent<CanvasGroup>(out var group))
            {
                schemeHintsList.Add(group);
                group.alpha = 0.0f;
            }
        }
    }

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<ControlSchemeChangeEvent>(OnControlSchemeChange).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnControlSchemeChange(ControlSchemeChangeEvent @event)
    {
        UpdateSchemeHint(@event.currentControlScheme);
    }

    /// <summary>
    /// 更新当前的控制器提示
    /// </summary>
    /// <param name="controlScheme">新的控制器类型</param>
    public void UpdateSchemeHint(ControlScheme controlScheme)
    {
        // Debug.Log("Update Control Scheme To " + controlScheme);
        switch(controlScheme)
        {
            case ControlScheme.Keyboard:
                SetSchemeCanvasGroup(0);
                break;
            case ControlScheme.Gamepad:
                SetSchemeCanvasGroup(1);
                break;
        }
    }

    /// <summary>
    /// 仅显示id对应的CanvasGroup
    /// </summary>
    /// <param name="id"></param>
    private void SetSchemeCanvasGroup(int id)
    {
        for (int i = 0; i < schemeHintsList.Count; i++)
        {
            if (i == id)
            {
                schemeHintsList[i].alpha = 1.0f;
            }
            else
            {
                schemeHintsList[i].alpha = 0;
            }
        }
    }

    public void Show()
    {
        cg.alpha = 1.0f;
    }

    public void Hide()
    {
        cg.alpha = 0.0f;
    }
}
