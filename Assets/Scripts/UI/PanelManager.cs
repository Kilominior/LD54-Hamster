using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject victoryPanel;
    public MouseController mouseController;

    private void Start()
    {
        pausePanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
            DisablePlayerControl();
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
            victoryPanel.SetActive(false);
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
