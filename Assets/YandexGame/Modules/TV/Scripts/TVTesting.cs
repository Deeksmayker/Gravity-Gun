using UnityEngine;

namespace YG
{
    public class TVTesting : MonoBehaviour
    {
        public static TVTesting Instance;

        private void Start()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                if (YandexGame.EnvironmentData.isMobile)
                {
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public static void Create()
        {
            GameObject tvTestObj = new GameObject { name = "TV Testing" };
            tvTestObj.AddComponent<TVTesting>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                YandexGame.onTVKeyDown("Up");
            }
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            {
                YandexGame.onTVKeyUp("Up");
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                YandexGame.onTVKeyDown("Left");
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                YandexGame.onTVKeyUp("Left");
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                YandexGame.onTVKeyDown("Down");
            }
            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                YandexGame.onTVKeyUp("Down");
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                YandexGame.onTVKeyDown("Right");
            }
            if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                YandexGame.onTVKeyUp("Right");
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                YandexGame.onTVKeyDown("Enter");
            }
            if (Input.GetKeyUp(KeyCode.Return))
            {
                YandexGame.onTVKeyUp("Enter");
            }

            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                YandexGame.onTVKeyBack();
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                YandexGame.onTVKeyDown("MediaRewind");
            }
            if (Input.GetKeyUp(KeyCode.F6))
            {
                YandexGame.onTVKeyUp("MediaRewind");
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                YandexGame.onTVKeyDown("MediaPlayPause");
            }
            if (Input.GetKeyUp(KeyCode.F7))
            {
                YandexGame.onTVKeyUp("MediaPlayPause");
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                YandexGame.onTVKeyDown("MediaFastForward");
            }
            if (Input.GetKeyUp(KeyCode.F8))
            {
                YandexGame.onTVKeyUp("MediaFastForward");
            }
        }
    }
}