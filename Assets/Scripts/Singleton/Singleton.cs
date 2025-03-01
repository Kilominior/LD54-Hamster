public class Singleton<T> where T : new()
{
    private static T _singleton;

    public static T Instance
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = new T();
            }
            return _singleton;
        }
    }
}