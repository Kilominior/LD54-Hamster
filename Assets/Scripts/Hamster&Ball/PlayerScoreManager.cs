using UnityEditor;

public static class PlayerScoreManager
{
    public static int levelNums = 4;
    public static int[] scores = { 0, 0, 0, 0 };
    public static bool isFirstLaunchGame = true;

    public static void SetScore(int score, int levelNum)
    {
        if (levelNum >= 0 && levelNum < 5 && score > scores[levelNum])
            scores[levelNum] = score;
        else
            return;
    }

    public static int GetScore(int levelNum)
    {
        if (levelNum >= 0 && levelNum < 5)
            return scores[levelNum];
        else
            return 0;
    }
}
