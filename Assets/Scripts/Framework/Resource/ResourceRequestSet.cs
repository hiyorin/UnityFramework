using System.Collections;
using System.Collections.Generic;

namespace Framework.Resource
{
    public class ResourceRequestSet
    {
        private enum State
        {
            Wait,
            Loading,
            Complete,
        }

        private readonly List<ResourceRequestItem> _listResourceSet = new List<ResourceRequestItem> ();

        private State _state = State.Wait;

        /// <summary>
        /// ロードするリソースを追加
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="type">Type.</param>
        public void Add (string url, ResourceType type)
        {
            _listResourceSet.Add (new ResourceRequestItem (url, type));
        }

        /// <summary>
        /// ロードを開始する
        /// </summary>
        public void Start ()
        {
            _state = State.Loading;
        }

        /// <summary>
        /// ロードを停止させる
        /// </summary>
        public void Stop ()
        {
            _state = State.Complete;
        }

        /// <summary>
        /// 待機中か
        /// </summary>
        /// <returns><c>true</c> if this instance is wait; otherwise, <c>false</c>.</returns>
        public bool IsWait ()
        {
            return (_state == State.Wait);
        }

        /// <summary>
        /// ロード中か
        /// </summary>
        /// <returns><c>true</c> if this instance is loading; otherwise, <c>false</c>.</returns>
        public bool IsLoading ()
        {
            return (_state == State.Loading);
        }

        /// <summary>
        /// ロード完了しているか
        /// </summary>
        /// <returns><c>true</c> if this instance is complete; otherwise, <c>false</c>.</returns>
        public bool IsComplete ()
        {
            return (_state == State.Complete);
        }

        /// <summary>
        /// リソースリストの取得
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<ResourceRequestItem> GetList ()
        {
            return _listResourceSet;
        }

        public int Count ()
        {
            return _listResourceSet.Count;
        }
    }
}