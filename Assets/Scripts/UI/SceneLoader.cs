using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // public bool isLoading;

    public void LoadScene(string sceneName)
    {
        // isLoading = true;
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void LoadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }
}
