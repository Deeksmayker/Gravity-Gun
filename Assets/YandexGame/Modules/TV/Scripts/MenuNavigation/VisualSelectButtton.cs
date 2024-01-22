using UnityEngine;
using UnityEngine.UI;

namespace YG.MenuNav
{
    public class VisualSelectButtton : MonoBehaviour
    {
        public RectTransform vusualSlector;
        public float sizeFrame = 10.0f;

        private void OnEnable()
        {
            MenuNavigation.onIsActiveMavigation += Setup;
            MenuNavigation.onSelectButton += OnSelectButton;
        }

        private void OnDisable()
        {
            MenuNavigation.onIsActiveMavigation -= Setup;
            MenuNavigation.onSelectButton -= OnSelectButton;
        }

        private void Setup(bool isActiveNav)
        {
            vusualSlector.gameObject.SetActive(isActiveNav);
        }

        private void OnSelectButton(Button button)
        {
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            CopyTransform(vusualSlector, buttonRect);
            IncreaseSize(vusualSlector);
        }

        private void CopyTransform(RectTransform target, RectTransform source)
        {
            target.SetParent(source);
            target.anchorMin = new Vector2(0, 0);
            target.anchorMax = new Vector2(1, 1);
            target.position = Vector3.zero;
            target.sizeDelta = Vector2.zero;
            target.offsetMin = Vector2.zero;
            target.offsetMax = Vector2.zero;
        }

        private void IncreaseSize(RectTransform target)
        {
            Vector2 newSize = target.sizeDelta;
            newSize.x += sizeFrame;
            newSize.y += sizeFrame;
            target.sizeDelta = newSize;
        }
    }
}