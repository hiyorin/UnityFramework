using UnityEngine;

namespace Framework.Resource
{
    public abstract class BaseResourceRequest
    {
        private enum State
        {
            Wait,
            Loading,
            Complete,
        }

        private State _state = State.Wait;

        public float progress { get; set; }

        public void Start ()
        {
            _state = State.Loading;
        }

        public void Stop ()
        {
            _state = State.Wait;
        }

        public void Complete ()
        {
            _state = State.Complete;
        }

        public bool IsWait ()
        {
            return _state == State.Wait;
        }

        public bool IsLoading ()
        {
            return _state == State.Loading;
        }

        public bool IsComplete ()
        {
            return _state == State.Complete;
        }
    }
}