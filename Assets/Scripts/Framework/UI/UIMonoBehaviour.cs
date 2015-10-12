using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Framework.UI
{
	[RequireComponent (typeof (RectTransform))]
	public abstract class UIMonoBehaviour : UIBehaviour
	{
		private RectTransform m_rectTransform;
		public RectTransform rectTransform {
			get {
				if (m_rectTransform == null)
					m_rectTransform = GetComponent<RectTransform> ();
				return m_rectTransform;
			}
		}

		private Canvas m_rootCanvas;
		protected Canvas rootCanvas {
			get {
				if (m_rootCanvas == null)
					m_rootCanvas = GetComponentInParent<Canvas> ();
				return m_rootCanvas;
			}
		}

        private UICanvas m_rootUICanvas;
        protected UICanvas rootUICanvas {
            get {
                if (m_rootUICanvas == null)
                    m_rootUICanvas = GetComponentInParent<UICanvas> ();
                return m_rootUICanvas;
            }
        }
	}
}
