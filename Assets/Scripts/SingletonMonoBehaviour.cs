using UnityEngine;

/// <summary>
/// GameObjectのシングルトン化を提供する
/// </summary>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	private static T _instance;

	/// <summary>
	/// シングルトンのインスタンス
	/// </summary>
	/// <value>The instance.</value>
	public static T Instance {
		get {
			if (_instance == null)
            {
				_instance = FindObjectOfType<T> ();
				if (_instance == null)
                {
					GameObject go = new GameObject (typeof (T).Name, typeof (T));
					_instance = go.GetComponent<T> ();
                    _instance.CheckInstance ();
				}
			}
			return _instance;
		}
	}

	/// <summary>
	/// シングルトンのインスタンス取得
	/// </summary>
	/// <returns>The instance.</returns>
	public static T GetInstance ()
	{
		return Instance;
	}

    [SerializeField]
    private bool _isDontDestroyOnLoad = true;

    private bool isExecuteAwake = false;
    private bool isExecuteStart = false;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	protected SingletonMonoBehaviour () {}

    private void Awake ()
    {
        if (isExecuteAwake == true)
            return;
        if (CheckInstance () == false)
            return;

        isExecuteAwake = true;
        OnAwake ();
    }

    private void Start ()
    {
        if (isExecuteStart == true)
            return;

        isExecuteStart = true;
        OnStart ();
    }

    private void OnDestroy ()
    {
        if (_isDontDestroyOnLoad == true)
            return;
        if (this != _instance)
            return;
        
        OnFinalize ();
        _instance = null;
    }

	protected virtual void OnInitialize ()
	{

	}

    protected virtual void OnFinalize ()
    {

    }

    protected virtual void OnAwake ()
    {

    }

	protected virtual void OnStart ()
	{

	}

	protected virtual void Update ()
	{

	}

	/// <summary>
	/// インスタンスが存在するか確認する
	/// </summary>
	/// <returns><c>true</c>, if instance was checked, <c>false</c> otherwise.</returns>
	protected bool CheckInstance()
	{
		if (_instance == null)
        {
            _instance = (T)this;
            _instance.OnInitialize ();
            if (_instance._isDontDestroyOnLoad == true)
    			DontDestroyOnLoad (this.gameObject);
			return true;
		}
        else if (_instance == this)
        {
			return true;
		}

		Destroy (this);
		return false;
	}
}