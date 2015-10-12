using UnityEngine;
using System.Collections.Generic;
using UniLinq;

namespace Framework.Scene
{
    public class SceneNodeSet
    {
    	private readonly Dictionary<string, SceneNode> _dictSceneNode = new Dictionary<string, SceneNode> ();
    	private readonly Stack<string> _stackSceneName = new Stack<string> ();

    	public IEnumerable<SceneNode> nodeList { get { return _dictSceneNode.Values; } }
    	public IEnumerable<string> nameList { get { return _dictSceneNode.Keys; } }

    	public SceneNode currentSceneNode { private set; get; }

    	public void Add (SceneNode sceneNode)
    	{
    		if (_dictSceneNode.ContainsKey (sceneNode.name) == true)
    			return;

    		_dictSceneNode.Add (sceneNode.name, sceneNode);
    	}

    	public void Remove (string sceneName)
    	{
    		SceneNode sceneNode = null;
    		if (_dictSceneNode.TryGetValue (sceneName, out sceneNode) == false)
    			return;

            if (sceneNode.sceneType == SceneType.Static || sceneNode.sceneType == SceneType.Resident)
    		{
    			Debug.LogError ("[SceneNodeSet] Not Remove static scene.");
    			return;
    		}

    		if (sceneNode.rootObject != null)
    			GameObject.Destroy (sceneNode.rootObject);
    		
    		_dictSceneNode.Remove (sceneName);
    	}

    	public SceneNode Find (string sceneName)
    	{
            if (string.IsNullOrEmpty (sceneName) == true)
                return null;
            
    		SceneNode sceneNode = null;
    		if (_dictSceneNode.TryGetValue (sceneName, out sceneNode) == false)
    			return null;
    		
    		return sceneNode;
    	}

    	public SceneNode Next (string sceneName, bool isRegisterStack)
    	{
    		SceneNode sceneNode = Find (sceneName);
    		if (sceneNode == null)
    			return null;

    		if (currentSceneNode != null && isRegisterStack == true)
    			_stackSceneName.Push (currentSceneNode.name);
    		
    		currentSceneNode = sceneNode;

    		return sceneNode;
    	}

    	public SceneNode Back ()
    	{
    		if (_stackSceneName.Count <= 0)
    			return null;

    		SceneNode backSceneNode = Find (_stackSceneName.Peek ());
    		if (backSceneNode != null)
    			return null;
    		
    		currentSceneNode = backSceneNode;

    		return backSceneNode;
    	}

    	/// <summary>
    	/// メインシーン、サブシーンすべてロード完了したか
    	/// </summary>
    	/// <returns><c>true</c> if this instance is current scene load all complete; otherwise, <c>false</c>.</returns>
    	public bool IsCurrentSceneLoadAllComplete ()
    	{
    		if (currentSceneNode.property == null)
    			return false;
            
            foreach (var subSceneProperty in currentSceneNode.property.listAddSubScene)
    		{
    			SceneNode sceneNode = null;
                if (_dictSceneNode.TryGetValue (subSceneProperty.sceneName, out sceneNode) == false)
    				return false;
    		}

    		return true;
    	}

    	/// <summary>
    	/// メインシーン、サブシーンすべてロード完了しOnInitialize前の待機状態か
    	/// </summary>
    	/// <returns><c>true</c> if this instance is current scene load and wait all complete; otherwise, <c>false</c>.</returns>
    	public bool IsCurrentSceneLoadAndWaitAllComplete ()
    	{
    		if (IsCurrentSceneLoadAllComplete () == false)
    			return false;
            
            foreach (var subSceneProperty in currentSceneNode.property.listAddSubScene)
    		{
    			SceneNode sceneNode = null;
                if (_dictSceneNode.TryGetValue (subSceneProperty.sceneName, out sceneNode) == false)
    				return false;
    			if (sceneNode.state < SceneState.Initialize)
    				return false;
    		}

    		return true;
    	}

    	/// <summary>
    	/// メインシーン、サブシーンすべてDeactiveか
    	/// </summary>
    	/// <returns><c>true</c> if this instance is current scene load all deactive; otherwise, <c>false</c>.</returns>
    	public bool IsCurrentSceneLoadAllDeactive ()
    	{
            if (currentSceneNode.state != SceneState.Sleep && currentSceneNode.state != SceneState.Destroy)
                return false;

            // メインシーンが既に破棄済みの場合もある
            SceneProperty currentProperty = currentSceneNode.property;
            if (currentProperty == null)
                return true;
            
            foreach (var subSceneProperty in currentProperty.listAddSubScene)
    		{
    			SceneNode node = null;
                if (_dictSceneNode.TryGetValue (subSceneProperty.sceneName, out node) == false)
                    continue;
    			if (node.state != SceneState.Sleep)
    				return false;
    		}

    		return true;
    	}

        /// <summary>
        /// カレントのサブシーンに存在するか
        /// </summary>
        /// <returns><c>true</c> if this instance is current sub scene contains the specified subSceneName; otherwise, <c>false</c>.</returns>
        /// <param name="subSceneName">Sub scene name.</param>
        public bool IsCurrentSubSceneContains (string subSceneName)
        {
            return currentSceneNode.property.listAddSubScene.Any (x => x.sceneName == subSceneName);
        }
    }
}