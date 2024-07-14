using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using QFramework;

public class PanelManager : MonoBehaviour
{
    public int levelNum;
    public GameObject pausePanel;
    public GameObject victoryPanel;
    public MouseController mouseController;

    private void Start()
    {
        pausePanel.SetActive(false);
        victoryPanel.SetActive(false);
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        EventRegister();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
            DisablePlayerControl();
        }

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0) && victoryPanel.activeSelf)
        {
            PlayerScoreManager.SetScore(victoryPanel.GetComponent<VictoryPanelManager>().score, levelNum);
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync("Choose Menu");
        }
    }

    public void DisablePausePanel()
    {
        if (pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
            EnablePlayerControl();
        }
    }

    private void EventRegister()
    {
        TypeEventSystem.Global.Register<GameWinEvent>(OnGameWin).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGameWin(GameWinEvent @event)
    {
        EnableVictoryPanel();
    }

    public void EnableVictoryPanel()
    {
        if (!victoryPanel.activeSelf)
        {
            victoryPanel.SetActive(true);
            DisablePlayerControl();
        }
    }

    public void DisableVictoryPanel()
    {
        if (victoryPanel.activeSelf)
        {
            victoryPanel.SetActive(false);
            EnablePlayerControl();
        }
    }

    private void EnablePlayerControl()
    {
        Time.timeScale = 1f;
        mouseController.enabled = true;
    }

    private void DisablePlayerControl()
    {
        Time.timeScale = 0f;
        mouseController.enabled = false;
    }
}
