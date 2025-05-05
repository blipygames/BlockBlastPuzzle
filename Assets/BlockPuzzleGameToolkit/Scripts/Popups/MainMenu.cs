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
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class MainMenu : Popup
    {
        public CustomButton timedMode;
        public CustomButton classicMode;
        public CustomButton adventureMode;
        public CustomButton settingsButton;
        public CustomButton luckySpin;
        public GameObject playObject;

        [SerializeField]
        private GameObject freeSpinMarker;

        [SerializeField]
        private Image background;

        public Action OnAnimationEnded;

        private const string LastFreeSpinTimeKey = "LastFreeSpinTime";

        private void Start()
        {
            timedMode.onClick.AddListener(PlayTimedMode);
            classicMode.onClick.AddListener(PlayClassicMode);
            adventureMode.onClick.AddListener(PlayAdventureMode);
            settingsButton.onClick.AddListener(SettingsButtonClicked);
            luckySpin.onClick.AddListener(LuckySpinButtonClicked);
            UpdateFreeSpinMarker();
            GameDataManager.LevelNum = PlayerPrefs.GetInt("Level", 1);
            var levelsCount = Resources.LoadAll<Level>("Levels").Length;
            luckySpin.gameObject.SetActive(GameManager.instance.GameSettings.enableLuckySpin);
            if(!GameManager.instance.GameSettings.enableTimedMode)
                timedMode.gameObject.SetActive(false);
        }
        private bool CanUseFreeSpinToday()
        {
            if (!PlayerPrefs.HasKey(LastFreeSpinTimeKey))
            {
                return true;
            }

            var lastFreeSpinTimeStr = PlayerPrefs.GetString(LastFreeSpinTimeKey);
            var lastFreeSpinTime = DateTime.Parse(lastFreeSpinTimeStr);
            return DateTime.Now.Date > lastFreeSpinTime.Date;
        }

        private void UpdateFreeSpinMarker()
        {
            var isFreeSpinAvailable = CanUseFreeSpinToday();
            freeSpinMarker.SetActive(isFreeSpinAvailable);
        }

        private void PlayClassicMode()
        {
            GameManager.instance.SetGameMode(EGameMode.Classic);
            GameManager.instance.OpenMap();
        }

        private void PlayAdventureMode()
        {
            GameManager.instance.SetGameMode(EGameMode.Adventure);
            GameManager.instance.OpenMap();
        }

        private void PlayTimedMode()
        {
            GameManager.instance.SetGameMode(EGameMode.Timed);
            GameManager.instance.OpenMap();
        }

        private void SettingsButtonClicked()
        {
            MenuManager.instance.ShowPopup<Settings>();
        }

        private void LuckySpinButtonClicked()
        {
            MenuManager.instance.ShowPopup<LuckySpin>(null, _ => UpdateFreeSpinMarker());
        }

        public void OnAnimationEnd(){
            OnAnimationEnded?.Invoke();
        }
    }
}