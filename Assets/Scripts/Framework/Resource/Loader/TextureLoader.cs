using UnityEngine;
using System.Collections;

namespace Framework.Resource.Loader
{
    public class TextureLoader : BaseLoader
    {
        public Texture2D texture { private set; get; }

        protected override IEnumerator GenerateLoadProcess (string path)
        {
            using (WWW www = new WWW (path))
            {
                yield return www;
                while (www.progress < 1.0f || www.isDone == false)
                {
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