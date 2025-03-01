using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelButtonsManager : MonoBehaviour
{
    public GameObject[] levelButtons;
    public Image[] stars;
    public Sprite disabledButton;
    public Sprite[] scores;

    [SerializeField]
    private SceneLoader sceneLoader;
    private PlayerInput pi;
    private InputActionMap UIAM;

    private void Start()
    {
        pi = GetComponent<PlayerInput>();


        Initialize();
    }

    private void InitLevelButtons()
    {
        bool selectedObjSet = false;
        for (int i = 0; i < levelButtons.Length; i++)
        {
            // if (PlayerScoreManager.GetScore(i) > 0 || i == 0)
            // {
            //     enableButton(i);
            // }
            // else
            // {
            //     disableButton(i);
            // }
            enableButton(i);

            // 设置默认选项
            if (PlayerScoreManager.GetScore(i) == 0 && !selectedObjSet)
            {
                EventSystem.current.SetSelectedGameObject(levelButtons[i]);
                selectedObjSet = true;
            }
        }
        if(!selectedObjSet)
        {
            EventSystem.current.SetSelectedGameObject(levelButtons[0]);
        }
    }

    private void Initialize()
    {
        InitLevelButtons();

        // 初始化按键监听
        pi = GetComponent<PlayerInput>();
        UIAM = pi.actions.actionMaps[2];

        pi.onControlsChanged += OnControlsUpdate;
        ControlManager.Instance.InitControlScheme(pi);
        UIAM["Pause"].performed += OnPausePerformed;
    }

    private void OnDestroy()
    {
        pi.onControlsChanged -= OnControlsUpdate;
        UIAM["Pause"].performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        sceneLoader.LoadScene("Start Menu");
    }

    private void OnControlsUpdate(PlayerInput pi)
    {
        ControlManager.Instance.OnControlsUpdate(pi);
    }

    private void disableButton(int level)
    {
        levelButtons[level].GetComponent<Button>().interactable = false;
        levelButtons[level].GetComponent<Image>().sprite = disabledButton;
        stars[level].enabled = false;
    }

    private void enableButton(int level)
    {
        levelButtons[level].GetComponent<Button>().interactable = true;
        stars[level].enabled = true;
        stars[level].sprite = scores[PlayerScoreManager.GetScore(level)];
    }
}
