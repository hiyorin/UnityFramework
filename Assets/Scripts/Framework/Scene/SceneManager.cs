using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

namespace Framework.Scene
{
    /// <summary>
    /// シーン管理クラス
    /// </summary>
    [RequireComponent (typeof (SceneLoader))]
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
#if UNITY_EDITOR
        public abstract class AbstractEditor : UnityEditor.Editor {
			public SceneManager instance { get { return target as SceneManager; } }
            public List<string> listCollectionIgnoreObjectName { get { return instance._listCollectionIgnoreObjectName; } }
            public SceneNodeSet sceneNodeSet { get { return instance._sceneNodeSet; } }
		}
#endif
    	private readonly List<string> _listCollectionIgnoreObjectName = new List<string> ();
    	private readonly SceneNodeSet _sceneNodeSet = new SceneNodeSet (); 
        private readonly List<string> _listDestroySceneName = new List<string> ();

    	private SceneLoader _sceneLoader = null;
    	private IEnumerator _loadStaticSceneProcess = null;
    	private IEnumerator _nextSceneProcess = null;
        private IEnumerator _changeSubSceneProcess = null;

    	#region Lifecycle Method
        protected override void OnInitialize ()
    	{
    		base.OnInitialize ();
    		_sceneLoader = GetComponent<SceneLoader> ();
            _listCollectionIgnoreObjectName.AddRange (SceneManagerSettings.ListCollectionIgnoreObjectName);
    		_listCollectionIgnoreObjectName.Add (gameObject.name);
    	}

        protected override void OnFinalize ()
    	{
    		base.OnFinalize ();
            _sceneLoader = null;
            _loadStaticSceneProcess = null;
            _nextSceneProcess = null;
            _changeSubSceneProcess = null;
    	}

        protected override void OnStart ()
        {
            base.OnStart ();

#if UNITY_EDITOR
            if (UnityEditor.EditorBuildSettings.scenes.Any (x => 
                    x.path.EndsWith (
                        Application.loadedLevelName + ".unity")) == false)
                return;
#endif
            _sceneLoader.InitializeWithCollection (_listCollectionIgnoreObjectName);

            GameObject rootObject = _sceneLoader.Pop ();
            rootObject.transform.parent = this.gameObject.transform;
            SceneNode sceneNode = new SceneNode (rootObject.name, rootObject);
            sceneNode.state = SceneState.Create;
            _sceneNodeSet.Add (sceneNode);
            _sceneNodeSet.Next (sceneNode.name, false);

            if (_loadStaticSceneProcess == null)
                _loadStaticSceneProcess = LoadFirstSceneProcess (sceneNode, SceneManagerSettings.ListStaticScene, SceneManagerSettings.ListResidentScene);
            else
                Debug.LogErrorFormat ("{0} sceneLoader not initialized.", typeof(SceneManager).Name);
        }

        protected override void Update ()
    	{
            base.Update ();
    		if (_loadStaticSceneProcess != null)
    		{
    			if (_loadStaticSceneProcess.MoveNext () == false)
    				_loadStaticSceneProcess = null;
    			return;
    		}

            if (_nextSceneProcess != null)
            {
                if (_nextSceneProcess.MoveNext () == false)
                    _nextSceneProcess = null;
            }
            else if (_changeSubSceneProcess != null)
            {
                if (_changeSubSceneProcess.MoveNext () == false)
                    _changeSubSceneProcess = null;
            }

    		// ライフサイクル制御
    		foreach (var sceneNode in _sceneNodeSet.nodeList)
    		{
    			switch (sceneNode.state)
    			{
    			case SceneState.Empty:
    			case SceneState.Create:
    				if (sceneNode.scene.OnSceneCreate () == true)
    					sceneNode.state = SceneState.Initialize;
    				break;
                case SceneState.Initialize:
                    if (_sceneNodeSet.IsCurrentSceneLoadAndWaitAllComplete () == false)
                        break;
                    if (sceneNode.isInitialized == false)
                        if (sceneNode.scene.OnSceneInitialize () == false)
                            break;
                    sceneNode.isInitialized = true;
                    if (sceneNode.isVisibled == true)
                        sceneNode.state = SceneState.Start;
                    else
                        sceneNode.state = SceneState.Invisible;
    				break;
                case SceneState.Start:
                    if (_sceneNodeSet.currentSceneNode.Equals (sceneNode) == true)
                    {
                        sceneNode.scene.OnSceneStart ();
                        TransitionSceneManager.Instance.FadeIn ();
                        sceneNode.state = SceneState.Update;
                    }
                    else if (sceneNode.isVisibled == true)
                        sceneNode.state = SceneState.Visible;
                    else
                        sceneNode.state = SceneState.Update;
    				break;
                case SceneState.Visible:
                    sceneNode.scene.OnSceneVisible ();
                    sceneNode.state = SceneState.Update;
                    break;
    			case SceneState.Update:
    				sceneNode.scene.OnSceneUpdate ();
    				break;
                case SceneState.Invisible:
                    sceneNode.scene.OnSceneInvisible ();
                    sceneNode.state = SceneState.Sleep;
                    break;
                case SceneState.Stop:
                    if (_sceneNodeSet.currentSceneNode.Equals (sceneNode) == true)
                        sceneNode.scene.OnSceneStop ();
                    if (sceneNode.property.isAlwaysDestroy == true)
                        sceneNode.state = SceneState.Destroy;
                    else
                    {
                        sceneNode.isInitialized = false;
                        sceneNode.state = SceneState.Sleep;
                    }
                    sceneNode.isActive = false;
    				break;
    			case SceneState.Sleep:
    				break;
                case SceneState.Destroy:
                    sceneNode.scene.OnSceneDestroy ();
                    _listDestroySceneName.Add (sceneNode.name);
    				break;
    			default:
    				break;
    			}
    		}

            if (_listDestroySceneName.Count > 0)
            {
                foreach (var sceneName in _listDestroySceneName)
                    _sceneNodeSet.Remove (sceneName);
                _listDestroySceneName.Clear ();
            }
    	}
    	#endregion

    	#region Private Method
        /// <summary>
        /// すべてのシーンがDeactiveになるのを待機してから指定されたシーンをロードする
        /// </summary>
        /// <returns>The next scene process.</returns>
        /// <param name="sceneName">Scene name.</param>
        /// <param name="fadeType">Fade type.</param>
        /// <param name="isRegisterStack">If set to <c>true</c> is register stack.</param>
    	private IEnumerator CreateNextSceneProcess (string sceneName, FadeType fadeType, bool isRegisterStack)
        {
            // フェードアウト
            TransitionSceneManager.Instance.FadeOut (fadeType);
            while (TransitionSceneManager.Instance.isComplete == false)
                yield return null;

            // 現在展開されているシーンを止める
            _sceneNodeSet.currentSceneNode.state = SceneState.Stop;
            foreach (var sceneNode in _sceneNodeSet.nodeList)
                if (sceneNode.sceneType != SceneType.Resident)
                    sceneNode.state = SceneState.Stop;
            while (_sceneNodeSet.IsCurrentSceneLoadAllDeactive () == false)
                yield return null;

    		// メインの読み込み
            SceneNode mainSceneNode = null;
            IEnumerator loadMainSceneProcess = LoadSingleSceneProcess (sceneName, SceneType.Standard, (SceneNode result) => {
    			mainSceneNode = result;
    			_sceneNodeSet.Next (sceneName, isRegisterStack);
    		});
            while (loadMainSceneProcess.MoveNext () == true)
                yield return null;

    		// サブの読み込み
            IEnumerator loadSubSceneProcess = LoadSubSceneProcess (mainSceneNode);
            while (loadSubSceneProcess.MoveNext () == true)
                yield return null;
    	}

        private IEnumerator LoadSubSceneProcess (SceneNode sceneNode)
        {
            foreach (var subSceneProperty in sceneNode.property.listAddSubScene)
            {
                IEnumerator loadSubScenePorocess = LoadSingleSceneProcess (subSceneProperty.sceneName, SceneType.Standard,
                    (SceneNode subSceneNode) => {
                        subSceneNode.isActive = subSceneProperty.isVisible;
                    }
                );
                while (loadSubScenePorocess.MoveNext () == true)
                    yield return null;
            }
        }

        private IEnumerator LoadSingleSceneProcess (string sceneName, SceneType sceneType, Action<SceneNode> callback)
        {
    		SceneNode sceneNode = _sceneNodeSet.Find (sceneName);
    		if (sceneNode == null)
    		{
    			_sceneLoader.Push (sceneName);

                while (_sceneLoader.IsComplete (sceneName) == false)
                    yield return null;
                
    			GameObject rootObject = _sceneLoader.Pop (sceneName);

    			// SceneManager配下で管理する
                rootObject.transform.SetParent (this.gameObject.transform);

                sceneNode = new SceneNode (sceneName, rootObject, sceneType);
    			sceneNode.state = SceneState.Create;
    			_sceneNodeSet.Add (sceneNode);
    		}
    		else
            {
    			sceneNode.state = SceneState.Initialize;
                sceneNode.isActive = true;
                TransitionSceneManager.Instance.ResetSlide (sceneNode.rootObject);
    		}

    		if (callback != null)
    			callback.Invoke (sceneNode);
    	}

        private IEnumerator LoadFirstSceneProcess (SceneNode mainSceneNode, string[] listStaticSceneName, string[] listResidentSceneName)
    	{
            // サブシーンの読み込み
            IEnumerator loadSubSceneProcess = LoadSubSceneProcess (mainSceneNode);
            while (loadSubSceneProcess.MoveNext () == true)
                yield return null;

            // スタティックシーンの読み込み
    		foreach (var sceneName in listStaticSceneName)
    		{
                IEnumerator process = LoadSingleSceneProcess (sceneName, SceneType.Static, null);
                while (process.MoveNext () == true)
                    yield return null;
    		}

            // 常駐シーンの読み込み
            foreach (var sceneName in listResidentSceneName)
            {
                IEnumerator process = LoadSingleSceneProcess (sceneName, SceneType.Resident, null);
                while (process.MoveNext () == true)
                    yield return null;
            }
    	}

        private IEnumerator CreateChangeSubSceneSlideProcess (string beforeSceneName, string afterSceneName, SlideType slideType)
        {
            SceneNode beforeSceneNode = _sceneNodeSet.Find (beforeSceneName);
            SceneNode afterSceneNode = _sceneNodeSet.Find (afterSceneName);
            if (afterSceneNode != null)
            {
                afterSceneNode.state = SceneState.Initialize;
                while (afterSceneNode.state != SceneState.Update)
                    yield return null;
                afterSceneNode.isActive = true;
            }

            TransitionSceneManager.Instance.Slide (
                beforeSceneNode == null ? null : beforeSceneNode.rootObject,
                afterSceneNode == null ? null : afterSceneNode.rootObject,
                slideType
            );
            while (TransitionSceneManager.Instance.isComplete == false)
                yield return null;
            
            if (beforeSceneNode != null)
                beforeSceneNode.state = SceneState.Invisible;
        }

    	#endregion

    	#region Public Method
    	/// <summary>
    	/// シーン読み込み
    	/// </summary>
    	/// <param name="sceneName">読み込むシーン</param>
        /// <param name="fadeType">>トランジションの種類を選ぶ</param>
    	/// <param name="isRegisterStack">スタックに積むかどうか</param"> 
    	public void NextScene (Scene scene, FadeType fadeType, bool isRegisterStack)
    	{
    		NextScene (scene.ToString (), fadeType, isRegisterStack);
    	}

    	/// <summary>
    	/// シーン読み込み
    	/// </summary>
        /// <param name="sceneName">読み込むシーン名</param>
        /// <param name="fadeType">>トランジションの種類を選ぶ</param>
    	/// <param name="isRegisterStack">スタックに積むかどうか</param"> 
    	public void NextScene (string sceneName, FadeType fadeType, bool isRegisterStack)
    	{
    		if (IsSceneLoading () == true)
                throw new Exception (string.Format ("{0}.{1} {2} The Loading other scene.", typeof (SceneManager).Name, MethodBase.GetCurrentMethod ().Name, sceneName));
    		if (_sceneNodeSet.currentSceneNode.name.Equals (sceneName) == true)
                throw new Exception (string.Format ("{0}.{1} {2} Same scene can not be loaded with the current scene.", typeof (SceneManager).Name,  MethodBase.GetCurrentMethod ().Name , sceneName));
    		
    		_nextSceneProcess = CreateNextSceneProcess (sceneName, fadeType, isRegisterStack);
        }

        /// <summary>
        /// スタックに積まれているシーンから読み込む
        /// </summary>
        public void BackScene (FadeType fadeType)
        {
            SceneNode backSceneNode = _sceneNodeSet.Back ();
            if (backSceneNode == null)
                return;
            
            if (IsSceneLoading () == true)
                throw new Exception (string.Format ("{0}.{1} Loading...", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name));
            if (_sceneNodeSet.currentSceneNode.name.Equals (backSceneNode.name) == true)
                throw new Exception (string.Format ("{0}.{1} Same scene can not be loaded with the current scene.", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name));

            _nextSceneProcess = CreateNextSceneProcess (backSceneNode.name, fadeType, false);
        }

        public void ChangeSubSceneSlide (SceneBase owner, Scene beforeScene, Scene afterScene, SlideType slideType)
        {
            ChangeSubSceneSlide (owner,
                beforeScene == Scene.Empty ? null : beforeScene.ToString (),
                afterScene == Scene.Empty ? null : afterScene.ToString (),
                slideType
            );
        }

        public void ChangeSubSceneSlide (SceneBase owner, string beforeSceneName, string afterSceneName, SlideType slideType)
        {
            if (IsSceneLoading () == true)
                throw new Exception (string.Format ("{0}.{1} Loading...", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name));
            if (IsSubSceneChanging () == true)
                throw new Exception (string.Format ("{0}.{1} SubScene Changing...", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name));
            if (_sceneNodeSet.currentSceneNode.scene.Equals (owner) == false)
                throw new Exception (string.Format ("{0}.{1} Please call from the main scene.", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name));
            if (string.IsNullOrEmpty (beforeSceneName) == true && string.IsNullOrEmpty (afterSceneName) == true)
                throw new Exception (string.Format ("{0}.{1} Param All Empty...", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name));
            if (string.IsNullOrEmpty (beforeSceneName) == false && _sceneNodeSet.IsCurrentSubSceneContains (beforeSceneName) == false)
                throw new Exception (string.Format ("{0}.{1} beforeSceneName:{3} NotFound in SubSceneList...", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name, beforeSceneName));
            if (string.IsNullOrEmpty (afterSceneName) == false && _sceneNodeSet.IsCurrentSubSceneContains (afterSceneName) == false)
                throw new Exception (string.Format ("{0}.{1} afterSceneName: {3} NotFound in SubSceneList...", typeof(SceneManager).Name, MethodBase.GetCurrentMethod ().Name, afterSceneName));

            _changeSubSceneProcess = CreateChangeSubSceneSlideProcess (beforeSceneName, afterSceneName, slideType);
        }

    	/// <summary>
    	/// シーン読み込み中か
    	/// </summary>
    	/// <returns><c>true</c> if this instance is scene loading; otherwise, <c>false</c>.</returns>
    	public bool IsSceneLoading ()
    	{
    		return (_nextSceneProcess != null || _sceneLoader.IsLoading () == true);
    	}

        /// <summary>
        /// サブシーン変更中か
        /// </summary>
        /// <returns><c>true</c> if this instance is sub scene changing; otherwise, <c>false</c>.</returns>
        public bool IsSubSceneChanging ()
        {
            return (_changeSubSceneProcess != null);
        }

        public bool IsCurrentScene (SceneBase owner)
        {
            return _sceneNodeSet.currentSceneNode.scene.Equals (owner);
        }

    	/// <summary>
    	/// 回収されないオブジェクトの名前を登録する
    	/// </summary>
    	/// <param name="name">Name.</param>
    	public void AddIgnoreCollection (string name)
    	{
    		_listCollectionIgnoreObjectName.Add (name);
    	}

    	/// <summary>
    	/// 回収されないオブジェクトの名前を削除する
    	/// </summary>
    	/// <param name="name">Name.</param>
    	public void RemoveIgnoreCollection (string name)
    	{
    		_listCollectionIgnoreObjectName.Remove (name);
    	}

        /// <summary>
        /// シーンのキャッシュを削除する
        /// </summary>
        /// <param name="scene">Scene.</param>
    	public void ClearSceneCache (Scene scene)
    	{
    		SceneNode sceneNode = _sceneNodeSet.Find (scene.ToString ());
    		if (sceneNode == null)
    			return;
    		if (sceneNode.isActive == true)
    			return;
    		sceneNode.scene.OnSceneDestroy ();
    		_sceneNodeSet.Remove (scene.ToString ());
    	}

        /// <summary>
        /// シーンのキャッシュを全て削除する
        /// </summary>
    	public void ClearAllSceneCache ()
    	{
    		foreach (var sceneName in _sceneNodeSet.nameList.ToArray ())
    		{
    			SceneNode sceneNode = _sceneNodeSet.Find (sceneName);
    			if (sceneNode.isActive == true)
    				continue;
    			sceneNode.scene.OnSceneDestroy ();
    			_sceneNodeSet.Remove (sceneName);
    		}
    	}

    	public void SendMessage (SceneBase owner, Dictionary<string, object> param)
    	{
    		SceneNode ownerSceneNode = _sceneNodeSet.nodeList.FirstOrDefault (x => x.scene == owner);
    		if (ownerSceneNode == null)
    			return;

            Scene currentScene = (Scene)Enum.Parse (typeof(Scene), ownerSceneNode.name);
    		SceneBase currentSceneBase = _sceneNodeSet.currentSceneNode.scene;
    		if (currentSceneBase.Equals (owner) == false)
    			currentSceneBase.OnSceneMessage (currentScene, param);

            foreach (var subSceneProperty in _sceneNodeSet.currentSceneNode.property.listAddSubScene)
    		{
                SceneNode subSceneNode = _sceneNodeSet.Find (subSceneProperty.sceneName);
    			if (subSceneNode.scene.Equals (owner) == false)
    				subSceneNode.scene.OnSceneMessage (currentScene, param);
    		}
    	}

    	public T GetSubScene<T> (SceneBase owner) where T:SceneBase
    	{
    		if (_sceneNodeSet.currentSceneNode.scene.Equals (owner) == false)
    			return null;
    		
            foreach (var subSceneProperty in _sceneNodeSet.currentSceneNode.property.listAddSubScene)
    		{
                SceneNode sceneNode = _sceneNodeSet.Find (subSceneProperty.sceneName);
    			if (sceneNode != null && sceneNode.scene is T)
    				return sceneNode.scene as T;
    		}

    		return null;
    	}
    	#endregion
    }
}
