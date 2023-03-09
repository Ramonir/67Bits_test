using UnityEngine;


public abstract class BaseManager<T> : MonoBehaviour where T : MonoBehaviour
{

    static T _instance;

    public static T Instance
    {
        get { return _instance; }
        set
        {
            if (_instance == null)
            {
                _instance = value;
                DontDestroyOnLoad(_instance.gameObject);
            }
            else if (_instance != value)
            {
                Destroy(value.gameObject);
            }
        }
    }

    protected BaseManager()
    {
        _instance = this as T;
    }
}
