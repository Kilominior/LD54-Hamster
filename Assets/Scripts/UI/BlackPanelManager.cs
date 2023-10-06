using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackPanelManager : MonoBehaviour
{
    public GameObject blackPanel;

    private Image image;

    private void Awake()
    {
        if (PlayerScoreManager.isEnteredStartMenu)
            blackPanel.SetActive(false);
    }

    private void Start()
    {
        image = blackPanel.GetComponent<Image>();
        // DecreaseBlack();
    }

    // 逐渐降低透明度
    public void DecreaseBlack()
    {
        StartCoroutine(FadeOut());
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

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终透明值为0
        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        // 将面板设置为不活跃
        PlayerScoreManager.isEnteredStartMenu = true;
        blackPanel.SetActive(false);
    }
}
