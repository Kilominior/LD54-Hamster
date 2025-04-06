using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlackPanelManager : TriggerMechanism
{
    public GameObject blackPanel;
    public GameObject defaultSelectObj;
    public MouseController player;
    public OnScreenControlPanel onScreenControlPanel;
    public Button SkipButton;

    private Image image;
    private Color showColor;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        image = blackPanel.GetComponent<Image>();
        showColor = image.color;
        canvasGroup = blackPanel.GetComponent<CanvasGroup>();

        SkipButton.onClick.AddListener(() => {
            HidePanel();
            SkipButton.gameObject.SetActive(false);
        });

        // 首次进入开始菜单则显示黑幕
        if (PlayerScoreManager.isFirstLaunchGame)
        {
            ShowPanel();
            PlayerScoreManager.isFirstLaunchGame = false;
        }
        else
        {
            HidePanel();
        }

        EventRegister();
    }

    private void ShowPanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        image.color = showColor;

        // 允许控制鼠/球
        player.RecoverActionMap();
    }

    // 设置Panel至关闭状态，直接调用需要先判定Panel仍处于开启状态
    public void HidePanel()
    {
        // 确保最终透明值为0
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        image.color = new Color(0, 0, 0, 0);

        SkipButton.gameObject.SetActive(false);

#if UNITY_ANDROID || UNITY_IOS
        // 关闭虚拟手柄
        onScreenControlPanel.Hide();
#endif

        // 启用UI导航控制
        EventSystem.current.SetSelectedGameObject(defaultSelectObj);
        player.SetActionMapToUI();
    }

    // 逐渐降低透明度，最终关闭Panel
    private void FadePanel()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float duration = 2f; // 淡出的时间
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 使用Lerp在一定时间内逐渐改变透明值
            image.color = Color.Lerp(showColor, new Color(showColor.r, showColor.g, showColor.b, 0f), elapsedTime / duration);
            canvasGroup.alpha = image.color.a;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        HidePanel();
    }

    protected override void ExecuteTrigger()
    {
        FadePanel();
    }

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<GamePauseTriggeredEvent>(OnGamePause).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGamePause(GamePauseTriggeredEvent @event)
    {
        if (canvasGroup.alpha != 1.0f) return;
        HidePanel();
    }
}
