using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlackPanelManager : MonoBehaviour, IBaseMechanism
{
    public GameObject blackPanel;
    public GameObject defaultSelectObj;
    public MouseController player;

    private Image image;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        image = blackPanel.GetComponent<Image>();
        canvasGroup = blackPanel.GetComponent<CanvasGroup>();

        // 曾经进入过开始菜单，则直接关闭Panel
        if (PlayerScoreManager.isEnteredStartMenu)
        {
            DecreaseBlack();
        }

        EventRegister();
    }

    // 逐渐降低透明度，最终关闭Panel
    public void DecreaseBlack()
    {
        StartCoroutine(FadeOut());
    }

    // 设置Panel至关闭状态，须在player初始化完毕后执行
    private void HidePanel()
    {
        // 确保最终透明值为0
        canvasGroup.alpha = 0;
        image.color = new Color(0, 0, 0, 0);

        // 将面板设置为不活跃
        PlayerScoreManager.isEnteredStartMenu = true;
        blackPanel.SetActive(false);

        // 启用UI导航控制
        EventSystem.current.SetSelectedGameObject(defaultSelectObj);
        player.SetActionMapToUI();
    }

    private IEnumerator FadeOut()
    {
        float duration = 2f; // 淡出的时间
        float elapsedTime = 0f;
        Color initialColor = image.color;

        while (elapsedTime < duration)
        {
            // 使用Lerp在一定时间内逐渐改变透明值
            image.color = Color.Lerp(initialColor, new Color(initialColor.r, initialColor.g, initialColor.b, 0f), elapsedTime / duration);
            canvasGroup.alpha = image.color.a;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        HidePanel();
    }


    private void EventRegister()
    {
        TypeEventSystem.Global.Register<GamePauseTriggeredEvent>(OnGamePause).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGamePause(GamePauseTriggeredEvent @event)
    {
        HidePanel();
    }
}
