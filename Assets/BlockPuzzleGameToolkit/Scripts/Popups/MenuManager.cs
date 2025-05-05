// // ©2015 - 2025 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class MenuManager : SingletonBehaviour<MenuManager>
    {
        public Fader fader;
        public List<Popup> popupStack = new();

        [SerializeField]
        private Canvas canvas;

        public override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
            Popup.OnClosePopup += ClosePopup;
            Popup.OnBeforeCloseAction += OnBeforeCloseAction;
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }

        private void OnBeforeCloseAction(Popup popup)
        {
            if (fader != null && popupStack.Count == 0)
            {
                fader.FadeOut();
            }
        }

        private void OnSceneLoaded(Scene scene, Scene scene1)
        {
            //fader.FadeAfterLoadingScene();
            // Check if the canvas is null
            if (canvas == null && this != null)
            {
                // Find the canvas in the scene
                canvas = GetComponent<Canvas>();
            }

            canvas.worldCamera = Camera.main;
        }

        private void OnDisable()
        {
            Popup.OnClosePopup -= ClosePopup;
            SceneManager.activeSceneChanged -= OnSceneLoaded;
            Popup.OnBeforeCloseAction -= OnBeforeCloseAction;
        }

        public T ShowPopup<T>(Action onShow = null, Action<EPopupResult> onClose = null) where T : Popup
        {
            // Check if the popup is already opened
            if (popupStack.OfType<T>().Any())
            {
                return popupStack.OfType<T>().First();
            }

            return (T)ShowPopup("Popups/" + typeof(T).Name, onShow, onClose);
        }

        public Popup ShowPopup(string pathWithType, Action onShow = null, Action<EPopupResult> onClose = null)
        {
            // Check if the popup is already opened
            if (popupStack.Any(p => p.GetType().Name == pathWithType.Split('/').Last()))
            {
                return popupStack.First(p => p.GetType().Name == pathWithType.Split('/').Last());
            }

            var popupPrefab = Resources.Load<Popup>(pathWithType);
            if (popupPrefab == null)
            {
                Debug.LogError("Popup prefab not found in Resources folder: " + pathWithType);
                return null;
            }

            return ShowPopup(popupPrefab, onShow, onClose);
        }

        public Popup ShowPopup(Popup popupPrefab, Action onShow = null, Action<EPopupResult> onClose = null)
        {
            var popup = Instantiate(popupPrefab, transform);

            if (popupStack.Count > 0)
            {
                popupStack.Last().Hide();
            }

            popupStack.Add(popup);

            if (fader != null && popupStack.Count > 0 && popup.fade)
            {
                fader.FadeIn(.997f, popup.FadeInDuration);
            }

            // Initialize the delayed show
            popup.InitDelayedShow(onShow, onClose);

            return popup;
        }


        public Popup ShowPopupDelayed(Popup popupPrefab, Action onShow = null, Action<EPopupResult> onClose = null)
        {
            // Get delay from popup prefab
            float delay = popupPrefab.appearanceDelay;

            // Call fade in before starting coroutine
            if (fader != null  && popupPrefab.fade)
            {
                fader.FadeIn(.997f, popupPrefab.FadeInDuration);
            }

            // Start coroutine to delay instantiation
            StartCoroutine(ShowPopupDelayed(popupPrefab, delay, onShow, onClose));

            return null;
        }

        private IEnumerator ShowPopupDelayed(Popup popupPrefab, float delay, Action onShow, Action<EPopupResult> onClose)
        {
            yield return new WaitForSeconds(delay);

            var popup = Instantiate(popupPrefab, transform);

            if (popupStack.Count > 0)
            {
                popupStack.Last().Hide();
            }

            popupStack.Add(popup);
            
            // Initialize the delayed show
            popup.InitDelayedShow(onShow, onClose);
        }

        private void ClosePopup(Popup popupClose)
        {
            if (popupStack.Count > 0)
            {
                popupStack.Remove(popupClose);
                if (popupStack.Count > 0)
                {
                    var popup = popupStack.Last();
                    popup.Show();
                }
            }

            if (fader != null && popupStack.Count == 0 && fader.IsFaded())
            {
                fader.FadeOut();
            }
        }

        public void ShowPurchased(GameObject imagePrefab, string boostName)
        {
            var menu = ShowPopup<PurchasedMenu>();
            menu.GetComponent<PurchasedMenu>().SetIconSprite(imagePrefab, boostName);
        }

        private void Update()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                // Replace old Input system code with new Input System
                if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    if (popupStack is { Count: > 0 })
                    {
                        var closeButton = popupStack.Last().closeButton;
                        if (closeButton != null)
                        {
                            closeButton.onClick?.Invoke();
                        }
                    }
                }
            }
        }

        public T GetPopupOpened<T>() where T : Popup
        {
            foreach (var popup in popupStack)
            {
                if (popup.GetType() == typeof(T))
                {
                    return (T)popup;
                }
            }

            return null;
        }

        public void CloseAllPopups()
        {
            for (var i = 0; i < popupStack.Count; i++)
            {
                var popup = popupStack[i];
                popup.Close();
            }

            popupStack.Clear();
        }

        public bool IsAnyPopupOpened()
        {
            return popupStack.Count > 0;
        }

        public Popup GetLastPopup()
        {
            return popupStack.Last();
        }

        public void FadeIn(float fadeAlpha, Action action)
        {
            fader.FadeIn(fadeAlpha, action);
        }

        public void FadeOut()
        {
            fader.FadeOut();
        }
    }
}