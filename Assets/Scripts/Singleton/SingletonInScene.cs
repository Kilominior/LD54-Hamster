using UnityEngine;

public class SingletonInScene<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            gameObject.name = $@"[{typeof(T).Name}]";
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.Log($"destroying singleton {typeof(T)}");
            Instance = null;
        }
    }
}