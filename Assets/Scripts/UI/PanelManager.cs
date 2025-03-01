using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using QFramework;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PanelManager : SingletonMono<PanelManager>
{
    public int levelNum;
    [SerializeField]
    private CanvasGroup pausePanel;
    public GameObject pauseSelectedObj;
    public GameObject victoryPanel;
    public MouseController mouseController;

    private void Start()
    {
        DisablePausePanel();
        victoryPanel.SetActive(false);

        EventRegister();
    }

    public void Pause()
    {
        // 胜利时无法触发暂停
        if (victoryPanel.activeSelf) return;
        if (pausePanel.alpha != 0)
        {
            // 重复按下暂停可继续游戏
            GameContinue();
            return;
        }
        // 暂停触发时，显示暂停页面，同时更新输入模式为UI模式
        GamePause();
    }

    /// <summary>
    /// 在胜利Panel按任意键回到选关页面
    /// </summary>
    private void PassLevel()
    {
        PlayerScoreManager.SetScore(victoryPanel.GetComponent<VictoryPanelManager>().score, levelNum);
        SceneManager.LoadSceneAsync("Choose Menu");
    }

    private void GamePause()
    {
        EnablePausePanel();

        EventSystem.current.SetSelectedGameObject(pauseSelectedObj);
        PauseWithUIControl();
    }

    private void GameContinue()
    {
        DisablePausePanel();

        ContinueWithPlayerControl();
    }

    private void EnablePausePanel()
    {
        pausePanel.alpha = 1.0f;
        pausePanel.blocksRaycasts = true;
    }

    public void DisablePausePanel()
    {
        pausePanel.alpha = 0;
        pausePanel.blocksRaycasts = false;
    }

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<GameWinEvent>(OnGameWin).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GamePauseTriggeredEvent>(OnGamePause).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGameWin(GameWinEvent @event)
    {
        EnableVictoryPanel();
    }

    private void OnGamePause(GamePauseTriggeredEvent @event)
    {
        Pause();
    }

    public void EnableVictoryPanel()
    {
        if (!victoryPanel.activeSelf)
        {
            victoryPanel.SetActive(true);
            PauseWithUIControl();
            InputSystem.onAnyButtonPress.CallOnce(ctrl => PassLevel());
        }
    }

    public void DisableVictoryPanel()
    {
        if (victoryPanel.activeSelf)
        {
            victoryPanel.SetActive(false);
            ContinueWithPlayerControl();
        }
    }

    private void ContinueWithPlayerControl()
    {
        TimeManager.Instance.ExecuteContinue();
        mouseController.RecoverActionMap();
    }

    private void PauseWithUIControl()
    {
        TimeManager.Instance.ExecutePause();
        mouseController.SetActionMapToUI();
    }
}
