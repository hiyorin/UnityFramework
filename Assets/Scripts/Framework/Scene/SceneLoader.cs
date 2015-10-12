using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

namespace Framework.Scene
{
    /// <summary>
    /// シーンの非同期ロードを提供する
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
    	private readonly Queue<string>      _queueTask = new Queue<string> ();
    	private readonly List<GameObject>   _listComplete = new List<GameObject> ();

        private  List<string> _listCollectionIgnoreObjectName = new List<string> ();
    	private IEnumerator _loadSceneProcess = null;
    	private string _executeSceneName = string.Empty;

    	public EventSystem eventSystem { get; private set; }
    	public bool isInitialized { get; private set; }

    	void Update ()
        {
    		if (_loadSceneProcess != null)
    		{
    			if (_loadSceneProcess.MoveNext () == true)
    				return;
    			_loadSceneProcess = null;
    			_executeSceneName = string.Empty;
    		}
    		else if (_queueTask.Count > 0)
    		{
    			string sceneName = _queueTask.Dequeue ();
    			_executeSceneName = sceneName;
    			_loadSceneProcess = GenerateLoadProcess (sceneName);
    		}
    	}

        /// <summary>
        /// 初期化処理とGameObjectの回収をする
        /// </summary>
        /// <param name="listCollectionIgnoreObjectName">List collection ignore object name.</param>
        public void InitializeWithCollection (List<string> listCollectionIgnoreObjectName)
        {
            if (listCollectionIgnoreObjectName != null)
                _listCollectionIgnoreObjectName = listCollectionIgnoreObjectName;

            isInitialized = true;

            string sceneName = Application.loadedLevelName;
            if (_listCollectionIgnoreObjectName.Contains (sceneName) == true)
                return;
            
            GameObject rootObject = new GameObject (sceneName);
            CollectionSceneGameObject (rootObject);
            _listComplete.Add (rootObject);
        }

        /// <summary>
        /// シーン非同期ロードプロセスを生成する
        /// </summary>
        /// <returns>The load process.</returns>
        /// <param name="sceneName">Scene name.</param>
    	private IEnumerator GenerateLoadProcess (string sceneName)
    	{
    		SetEventSystemEnabled (false);

    		AsyncOperation asyncOperation = Application.LoadLevelAdditiveAsync (sceneName);
    		asyncOperation.allowSceneActivation = false;
    		while (asyncOperation.progress < 0.9f && asyncOperation.isDone == false)
    			yield return null;
    		asyncOperation.allowSceneActivation = true;

    		// シーン内のGameObjectを構成するのに1フレームかかるため待機時間とする
    		yield return null;

    		GameObject rootObject = new GameObject (sceneName);
            CollectionSceneGameObject (rootObject);
    		_listComplete.Add (rootObject);

    		SetEventSystemEnabled (true);
    	}

        /// <summary>
        /// HierarchyのrootにあるGameObjectを回収する
        /// </summary>
        /// <param name="rootObject">Root object.</param>
    	private void CollectionSceneGameObject (GameObject rootObject)
    	{
    		if (_listCollectionIgnoreObjectName.Contains (rootObject.name) == false)
    			_listCollectionIgnoreObjectName.Add (rootObject.name);
            
    		foreach (var go in Resources.FindObjectsOfTypeAll<GameObject> ())
    		{
    			if (go.hideFlags != HideFlags.None)
    				continue;
    			if (go.transform.parent != null || _listCollectionIgnoreObjectName.Contains (go.name) == true)
    				continue;

                // Sceneに直接配置されているPrefabを弾く
                if (go.activeSelf != go.activeInHierarchy)
                    continue;

    			// EventSystemを回収
    			if (go.GetComponent<EventSystem> () != null)
    			{
    				if (eventSystem == null)
    				{
    					eventSystem = go.GetComponent<EventSystem> ();
    					go.transform.SetParent (transform);
    				}
    				else
    				{
    					DestroyObject (go);
    				}
    				continue;
                }
    			go.transform.SetParent (rootObject.transform);
    		}
    	}

    	/// <summary>
    	/// 回収したEventSystemのActiveを切り替える
    	/// </summary>
    	/// <param name="enabled">If set to <c>true</c> enabled.</param>
    	private void SetEventSystemEnabled (bool enabled)
    	{
    		if (eventSystem != null)
    			eventSystem.enabled = enabled;
    	}

        /// <summary>
        /// ロードするシーンをキューに積む
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
    	public void Push (string sceneName)
    	{
    		if (_queueTask.Contains (sceneName) == true)
    			return;
    		
    		_queueTask.Enqueue (sceneName);
    	}

        /// <summary>
        /// ロード完了したシーンを名前を指定して回収する
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
    	public GameObject Pop (string sceneName)
    	{
    		GameObject rootObject = _listComplete.FirstOrDefault (x => x.name == sceneName);
    		if (rootObject == null)
    			return null;

    		_listComplete.Remove (rootObject);
    		return rootObject;
    	}

        /// <summary>
        /// ロード完了したシーンを回収する
        /// </summary>
    	public GameObject Pop ()
    	{
    		if (_listComplete.Count <= 0)
    			return null;

    		GameObject rootObject = _listComplete [0];
    		_listComplete.Remove (rootObject);
    		return rootObject;
    	}

        /// <summary>
        /// 指定したシーンがロード中か
        /// </summary>
        /// <returns><c>true</c> if this instance is loading the specified sceneName; otherwise, <c>false</c>.</returns>
        /// <param name="sceneName">Scene name.</param>
    	public bool IsLoading (string sceneName)
    	{
    		return _executeSceneName.Equals (sceneName);
    	}

        /// <summary>
        /// ロード中か
        /// </summary>
        /// <returns><c>true</c> if this instance is loading; otherwise, <c>false</c>.</returns>
    	public bool IsLoading ()
    	{
            return (_queueTask.Count > 0 || string.IsNullOrEmpty (_executeSceneName) == false);
    	}

        /// <summary>
        /// 指定したシーンがロード完了したか
        /// </summary>
        /// <returns><c>true</c> if this instance is complete the specified sceneName; otherwise, <c>false</c>.</returns>
        /// <param name="sceneName">Scene name.</param>
    	public bool IsComplete (string sceneName)
    	{
    		return _listComplete.Any (x => x.name == sceneName);
    	}

        /// <summary>
        /// 全てのシーンがロード完了したか
        /// </summary>
        /// <returns><c>true</c> if this instance is complete; otherwise, <c>false</c>.</returns>
    	public bool IsComplete ()
    	{
    		return (_queueTask.Count == 0 && IsLoading () == false);
    	}
    }
}