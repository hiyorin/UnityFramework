using UnityEngine;

namespace Framework.Resource
{
    [System.Serializable]
    public class ResourceItem
    {
        [SerializeField, ReadOnlyAttribute]
        private Object _resource = null;
        [SerializeField, ReadOnlyAttribute]
        private int _referenceCount = 0;

        public Object resource {
            get { return _resource; }
        }

        public int referenceCount {
            get { return _referenceCount; }
        }

        public ResourceItem (Object resource)
        {
            _resource = resource;
            _referenceCount = 1;
        }

        public void IncRef ()
        {
            _referenceCount++;
        }

        public void DecRef ()
        {
            _referenceCount --;
        }
    }
}