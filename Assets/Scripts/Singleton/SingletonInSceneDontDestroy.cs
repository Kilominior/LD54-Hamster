using UnityEngine;

public class SingletonInSceneDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            var go = gameObject;
            go.name = $@"[{typeof(T).Name}]";
            DontDestroyOnLoad(go);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}