using UnityEngine;

public class SingletonMonoDontDestroy<T> : MonoBehaviour where T : MonoBehaviour
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
                if (_singleton == null)
                {
                    Debug.Log($"creating singleton {typeof(T)}");
                    var target = new GameObject
                    {
                        name = $@"[{typeof(T).Name}]"
                    };
                    _singleton = target.AddComponent<T>();
                    DontDestroyOnLoad(target);
                }
            }
            return _singleton;
        }
    }
}
