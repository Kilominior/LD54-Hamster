using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    // public bool isLoading;

    public static void LoadScene(string sceneName)
    {
        // isLoading = true;
        SceneManager.LoadSceneAsync(sceneName);
    }

    public static void LoadScene(int buildId)
    {
        // isLoading = true;
        SceneManager.LoadSceneAsync(buildId);
    }

    public static void LoadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }
}
