using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pausePanel.activeSelf && Time.timeScale == 1f)
        {
            pausePanel.SetActive(true);
            DisablePlayerControl();
        }

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0) && victoryPanel.activeSelf && Time.timeScale == 0f)
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
