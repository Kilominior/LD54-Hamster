using UnityEditor;

public static class PlayerScoreManager
{
    public static int levelNums = 4;
    public static int[] scores = { 0, 0, 0, 0 };
    public static bool isEnteredStartMenu = false;

    public static void SetScore(int score, int levelNum)
    {
        if (levelNum > 0 && levelNum <= 5 && score > scores[levelNum - 1])
            scores[levelNum - 1] = score;
        else
            return;
    }

    public static int GetScore(int levelNum)
    {
        if (levelNum > 0 && levelNum <= 5)
            return scores[levelNum - 1];
        else
            return 0;
    }
}
