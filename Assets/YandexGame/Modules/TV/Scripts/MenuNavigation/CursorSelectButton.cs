using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace YG.MenuNav
{
    public class CursorSelectButton : MonoBehaviour
    {
        public UnityEvent PointEnterEvent;

        private bool active;
        public static Action onPointEnter;

        private void OnEnable()
        {
            MenuNavigation.onIsActiveMavigation += Setup;
            MenuNavigation.onOpenWindow += OnOpenWindow;
        }

        private void OnDisable()
        {
            MenuNavigation.onIsActiveMavigation -= Setup;
            MenuNavigation.onOpenWindow -= OnOpenWindow;
        }

        private void Setup(bool isActiveNav)
        {
            active = isActiveNav;
            OpenLayer();
        }

        private void OnOpenWindow(GameObject obj)
        {
            OpenLayer();
        }

        private void OpenLayer()
        {
            if (active)
            {
                int layerNum = MenuNavigation.Instance.layers.Count - 1;
                foreach (Button button in MenuNavigation.Instance.layers[layerNum].buttons)
                {
                    PointerEnterHelperNav helper = button?.GetComponent<PointerEnterHelperNav>();

                    if (!helper)
                        helper = button.gameObject.AddComponent<PointerEnterHelperNav>();

                    helper.Init(button, this);
                }
            }
        }

        public void PointerEnterCallback()
        {
            PointEnterEvent?.Invoke();
            onPointEnter?.Invoke();
        }
    }
}