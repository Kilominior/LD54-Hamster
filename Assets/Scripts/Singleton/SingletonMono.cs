using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _singleton;

    public static T Instance
    {
        get
        {
            if (!Application.isPlaying)
            {
                return null;
            }
            if (_singleton == null)
            {
                _singleton = (T)FindObjectOfType(typeof(T));
                // if (_singleton == null)
                // {
                //     Debug.Log($"creating singleton {typeof(T)}");
                //     var target = new GameObject
                //     {
                //         name = $@"[{typeof(T).Name}]"
                //     };
                //     _singleton = target.AddComponent<T>();
                // }
            }
            return _singleton;
        }
    }

    private void OnDestroy()
    {
        if (_singleton == this)
        {
            Debug.Log($"destroying singleton {typeof(T)}");
            _singleton = null;
        }
    }
    public static bool exists => _singleton != null;
}
