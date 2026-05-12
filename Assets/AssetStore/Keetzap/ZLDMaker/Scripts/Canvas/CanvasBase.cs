using UnityEngine;
using UnityEngine.EventSystems;

namespace Keetzap.ZeldaMaker
{
    public class CanvasBase : MonoBehaviour
    {
        protected GameObject _lastSelected;
        protected GameObject _currentSelected;

        void Update()
        {
            _currentSelected = EventSystem.current.currentSelectedGameObject;

            if (_currentSelected != null)
            {
                _lastSelected = _currentSelected;
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject() && _lastSelected != null)
                {
                    EventSystem.current.SetSelectedGameObject(_lastSelected);
                }
            }
        }

        public virtual void OnEnable()
        {
            _lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
