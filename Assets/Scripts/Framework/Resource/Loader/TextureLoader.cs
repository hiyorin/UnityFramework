using UnityEngine;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class TextureLoader : BaseLoader
    {
        public Texture2D texture { private set; get; }

        protected override IEnumerator GenerateLoadProcess ()
        {
            using (WWW www = new WWW (path))
            {
                while (www.progress < 1.0f || www.isDone == false)
                {
                    progress = www.progress;
                    yield return null;
                }

                if (www.texture == null)
                {
                    Failed (www.text);
                    yield break;
                }

                texture = www.texture;
                Successed ();
            }
        }
    }
}