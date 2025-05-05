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
using BlockPuzzleGameToolkit.Scripts.Audio;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay.Managers
{
    public class TimerManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject timerPanel;
        [SerializeField] private Color timerColor = new Color(0.612f, 0.988f, 1f, 1f);
        private float remainingTime;
        private bool isTimerActive;
        private bool isTimerPaused;
        private bool isWarningActive;
        private Color originalTextColor;
        private Sequence bounceSequence;
        
        public Action OnTimerExpired;
        
        private bool waitingForTutorial = false;
        private float initialDuration;

        public int RemainingTime => Mathf.FloorToInt(remainingTime);
        
        private void OnEnable()
        {
            EventManager.OnGameStateChanged += HandleGameStateChange;
            EventManager.GetEvent(EGameEvent.TutorialCompleted).Subscribe(OnTutorialCompleted);
        }
        
        private void OnDisable()
        {
            EventManager.OnGameStateChanged -= HandleGameStateChange;
            EventManager.GetEvent(EGameEvent.TutorialCompleted).Unsubscribe(OnTutorialCompleted);
        }
        
        private void HandleGameStateChange(EGameState state)
        {
            if (state == EGameState.Paused)
            {
                PauseTimer(true);
            }
            else if (state == EGameState.Playing)
            {
                PauseTimer(false);
            }
        }

        public void InitializeTimer(float duration)
        {
            if (GameManager.instance.IsTutorialMode())
            {
                waitingForTutorial = true;
                if (timerPanel != null)
                {
                    timerPanel.SetActive(false);
                }
                return;
            }

            initialDuration = duration;
            remainingTime = duration;
            isTimerActive = true;
            isTimerPaused = false;
            enabled = true;
            
            if (timerPanel != null)
            {
                timerPanel.SetActive(true);
                if (timerText != null)
                {
                    timerText.color = timerColor;
                    originalTextColor = timerColor;
                }
                UpdateTimerDisplay();
            }
        }
        
        public void PauseTimer(bool pause)
        {
            isTimerPaused = pause;
        }

        private void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                float timeToDisplay = Mathf.Max(0, remainingTime);
                int minutes = Mathf.FloorToInt(timeToDisplay / 60);
                int seconds = Mathf.FloorToInt(timeToDisplay % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                
                if (timeToDisplay <= 5f && !isWarningActive && isTimerActive)
                {
                    StartWarningEffect();
                }
                else if (timeToDisplay > 5f && isWarningActive)
                {
                    StopWarningEffect();
                }
            }
        }
        
        private void Start()
        {
            if (timerText != null)
            {
                originalTextColor = timerText.color;
            }
        }
        
        private void StartWarningEffect()
        {
            if (timerText == null) return;

            SoundBase.instance.PlaySound(SoundBase.instance.alert);
            isWarningActive = true;
            originalTextColor = timerText.color;
            
            if (bounceSequence != null)
            {
                bounceSequence.Kill();
                bounceSequence = null;
            }
            
            bounceSequence = DOTween.Sequence();
            bounceSequence.Append(timerText.transform.DOScale(1.2f, 0.3f));
            bounceSequence.Append(timerText.transform.DOScale(1f, 0.3f));
            timerText.color = Color.red;
            bounceSequence.SetLoops(-1);
        }
        
        private void StopWarningEffect()
        {
            if (timerText == null) return;
            
            isWarningActive = false;
            
            if (bounceSequence != null)
            {
                bounceSequence.Kill();
                bounceSequence = null;
            }
            
            timerText.transform.localScale = Vector3.one;
            timerText.color = originalTextColor;
        }
        
        private void OnDestroy()
        {
            if (bounceSequence != null)
            {
                bounceSequence.Kill();
                bounceSequence = null;
            }
        }
        
        public void StopTimer()
        {
            isTimerActive = false;
            enabled = false;
            StopWarningEffect();
            if (timerPanel != null)
            {
                timerPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (isTimerActive && !isTimerPaused && EventManager.GameStatus == EGameState.Playing)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerDisplay();
                if (RemainingTime <= 0)
                {
                    isTimerActive = false;
                    EventManager.GetEvent(EGameEvent.TimerExpired).Invoke();
                    OnTimerExpired?.Invoke();
                    StopTimer();
                }
            }
        }

        private void OnTutorialCompleted()
        {
            if (waitingForTutorial)
            {
                waitingForTutorial = false;
                InitializeTimer(initialDuration);
            }
        }
    }
}