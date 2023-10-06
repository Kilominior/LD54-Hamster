using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonsManager : MonoBehaviour
{
    public GameObject[] levelButtons;
    public Image[] stars;
    public Sprite disabledButton;
    public Sprite[] scores;

    private void Awake()
    {
        for (int i = 1; i <= levelButtons.Length; i++)
        {
            // if (PlayerScoreManager.GetScore(i - 1) > 0 || i == 1)
            //     enableButton(i);
            // else
            //     disableButton(i);
            enableButton(i);
        }
    }

    private void disableButton(int level)
    {
        levelButtons[level - 1].GetComponent<Button>().interactable = false;
        levelButtons[level - 1].GetComponent<Image>().sprite = disabledButton;
        stars[level - 1].enabled = false;
    }

    private void enableButton(int level)
    {
        levelButtons[level - 1].GetComponent<Button>().interactable = true;
        stars[level - 1].enabled = true;
        stars[level - 1].sprite = scores[PlayerScoreManager.GetScore(level)];
    }
}
