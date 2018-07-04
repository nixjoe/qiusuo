public class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;

    protected Singleton()
    {
        _instance = (T)this;
    }

    public static void Create()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
}