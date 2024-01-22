using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace YG.MenuNav
{
    public class MenuNavigation : MonoBehaviour
    {
        public Button usButton;
        public GameObject exitGameObj;
        public bool navigationOnDesktop;
        public UnityEvent OnBackButton;
        public List<LayerUI> layers = new List<LayerUI>();

        public static MenuNavigation Instance;
        public static Action<bool> onIsActiveMavigation;
        public static Action<Button> onSelectButton;
        public static Action<GameObject> onOpenWindow;
        public static Action onCloseWindow;
        public static Action onEnterButton;

        private void OnEnable()
        {
            YandexGame.onTVKeyDown += OnKeyDown;
            YandexGame.onTVKeyBack += OnKeyBack;
        }

        private void OnDisable()
        {
            YandexGame.onTVKeyDown -= OnKeyDown;
            YandexGame.onTVKeyBack -= OnKeyBack;
        }

        private void Start()
        {
            Instance = this;
            bool isActiveMavigation = true;

            if (!YandexGame.EnvironmentData.isTV)
            {
                if (navigationOnDesktop && YandexGame.EnvironmentData.isDesktop)
                {
                    TVTesting.Create();
                }
                else
                {
                    isActiveMavigation = false;
                }
            }

            if (isActiveMavigation)
            {
                AddLayer(null);

                if (usButton == null)
                    usButton = FindFirstObjectByType<Button>();

                SelectButton(usButton);
            }

            onIsActiveMavigation?.Invoke(isActiveMavigation);
        }

        private bool NavigationAllow()
        {
            if (YandexGame.EnvironmentData.isMobile || YandexGame.EnvironmentData.isTablet)
                return false;
            else if (!YandexGame.EnvironmentData.isTV && !navigationOnDesktop)
                return false;
            else 
                return true;
        }

        private void OnKeyDown(string key)
        {
            Button b;
            switch (key)
            {
                case "Up":
                    b = usButton.FindSelectableOnUp()?.GetComponent<Button>();
                    SelectButton(b);
                    break;
                case "Left":
                    b = usButton.FindSelectableOnLeft()?.GetComponent<Button>();
                    SelectButton(b);
                    break;
                case "Down":
                    b = usButton.FindSelectableOnDown()?.GetComponent<Button>();
                    SelectButton(b);
                    break;
                case "Right":
                    b = usButton.FindSelectableOnRight()?.GetComponent<Button>();
                    SelectButton(b);
                    break;
                case "Enter":
                    if (usButton)
                    {
                        usButton.onClick?.Invoke();
                        onEnterButton?.Invoke();
                    }
                    break;
            }
        }

        private void OnKeyBack()
        {
            if (layers.Count > 1)
            {
                CloseWindow();
            }
            else if (exitGameObj && YandexGame.EnvironmentData.isTV)
            {
                OpenWindow(exitGameObj);
            }
            else
            {
                OnBackButton.Invoke();
            }
        }

        public void SelectButton(Button button)
        {
            if (button == null || !NavigationAllow())
                return;

            usButton = button;
            onSelectButton?.Invoke(button);
        }

        public void SelectFirstButton()
        {
            Button[] buttons = FindObjectsOfType<Button>();
            usButton = buttons.FirstOrDefault(button => button.enabled);
            SelectButton(usButton);
        }

        private void AddLayer(GameObject openLayerObj)
        {
            if (openLayerObj)
                openLayerObj.SetActive(true);

            LayerUI newLayer = new LayerUI
            {
                layer = openLayerObj,
                buttons = new List<Button>()
            };

            Button[] allButtonsInScene = FindObjectsOfType<Button>();

            foreach (Button button in allButtonsInScene)
            {
                if (button.enabled)
                {
                    newLayer.buttons.Add(button);
                }
            }

            layers.Add(newLayer);
        }

        public void OpenWindow(GameObject openLayerObj)
        {
            foreach (Button button in layers[layers.Count - 1].buttons)
            {
                button.enabled = false;
            }

            AddLayer(openLayerObj);
            SelectFirstButton();

            onOpenWindow?.Invoke(openLayerObj);
        }

        public void CloseWindow()
        {
            onCloseWindow?.Invoke();
            
            if (layers.Count <= 1)
                return;

            int usLayer = layers.Count - 1;
            layers[usLayer].layer.SetActive(false);

            usLayer--;

            for (int i = 0; i < layers[usLayer].buttons.Count; i++)
            {
                if (layers[usLayer].buttons[i])
                    layers[usLayer].buttons[i].enabled = true;
            }

            layers.RemoveAt(usLayer + 1);
            SelectFirstButton();
        }

        [Serializable]
        public struct LayerUI
        {
            public GameObject layer;
            public List<Button> buttons;
        }
    }
}