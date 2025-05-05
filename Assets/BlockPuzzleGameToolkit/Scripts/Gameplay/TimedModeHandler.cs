using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class TimedModeHandler : BaseModeHandler
    {
        [SerializeField]
        private float gameDuration = 180f; // 3 minutes default game duration
        
        private TimerManager _timerManager;
        private Sequence _pulseSequence;

        public TimerManager TimerManager
        {
            get
            {
                if (_timerManager == null)
                {
                    _timerManager = FindObjectOfType<TimerManager>();
                }
                return _timerManager;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (TimerManager == null)
            {
                Debug.LogError("TimerManager not found!");
                return;
            }

            EventManager.OnGameStateChanged += HandleGameStateChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventManager.OnGameStateChanged -= HandleGameStateChange;
        }

        protected override void LoadScores()
        {
            // Load best score from resources using timed-specific key
            bestScore = ResourceManager.instance.GetResource("TimedBestScore").GetValue();
            bestScoreText.text = bestScore.ToString();

            // Load current score from timed game state
            var state = GameState.Load(EGameMode.Timed) as TimedGameState;
            if (state != null)
            {
                score = state.score;
                bestScore = state.bestScore;
                scoreText.text = score.ToString();
                TimerManager.InitializeTimer(state.remainingTime > 0 ? state.remainingTime : gameDuration);
            }
            else
            {
                score = 0;
                scoreText.text = "0";
                TimerManager.InitializeTimer(gameDuration);
            }
        }

        protected override void SaveGameState()
        {
            var fieldManager = _levelManager.GetFieldManager();
            if (fieldManager != null)
            {
                var state = new TimedGameState
                {
                    score = score,
                    bestScore = bestScore,
                    remainingTime = GetRemainingTime(),
                    gameMode = EGameMode.Timed,
                    gameStatus = EventManager.GameStatus
                };
                GameState.Save(state, fieldManager);
            }
        }

        protected override void DeleteGameState()
        {
            GameState.Delete(EGameMode.Timed);
        }

        public override void OnScored(int scoreToAdd)
        {
            base.OnScored(scoreToAdd);
            // AddBonusTime(scoreToAdd);
        }

        private void AddBonusTime(int scoreValue)
        {
            // Add 1 second for every 10 points scored
            float bonusTime = scoreValue / 10f;
            float currentTime = TimerManager.RemainingTime;
            TimerManager.InitializeTimer(Mathf.Min(currentTime + bonusTime, gameDuration));
        }

        public override void OnLose()
        {
            // Only update best score if timer actually reached 0
            if (TimerManager != null && TimerManager.RemainingTime <= 0)
            {
                bestScore = ResourceManager.instance.GetResource("TimedBestScore").GetValue();
                if (score > bestScore)
                {
                    ResourceManager.instance.GetResource("TimedBestScore").Set(score);
                }
            }
            
            base.OnLose();
        }

        // Optional: Pause functionality
        public void PauseGame()
        {
            if (TimerManager != null)
            {
                TimerManager.PauseTimer(true);
            }
        }

        public void ResumeGame()
        {
            if (TimerManager != null)
            {
                TimerManager.PauseTimer(false);
            }
        }

        public float GetRemainingTime()
        {
            return TimerManager != null ? TimerManager.RemainingTime : 0f;
        }

        private void HandleGameStateChange(EGameState newState)
        {
            if (TimerManager != null)
            {
                if (newState == EGameState.Playing)
                {
                    TimerManager.PauseTimer(false);
                }
                else if (newState == EGameState.Paused)
                {
                    TimerManager.PauseTimer(true);
                }
            }
        }
    }
}