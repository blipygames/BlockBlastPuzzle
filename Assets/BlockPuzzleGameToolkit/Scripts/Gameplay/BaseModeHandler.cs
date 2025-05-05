using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Enums;
using TMPro;
using UnityEngine;
using System.Collections;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public abstract class BaseModeHandler : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI bestScoreText;

        [HideInInspector]
        public int bestScore;

        [HideInInspector]
        public int score;

        protected LevelManager _levelManager;
        protected Coroutine _counterCoroutine;
        protected int _displayedScore = 0;
        [SerializeField]
        protected float counterSpeed = 0.01f;

        protected virtual void OnEnable()
        {
            _levelManager = FindObjectOfType<LevelManager>(true);
            
            if (_levelManager == null)
            {
                Debug.LogError("LevelManager not found!");
                return;
            }

            _levelManager.OnLose += OnLose;
            _levelManager.OnScored += OnScored;

            ResetScore();
            LoadScores();
        }

        protected virtual void OnDisable()
        {
            if (_levelManager != null)
            {
                _levelManager.OnLose -= OnLose;
                _levelManager.OnScored -= OnScored;
            }
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && EventManager.GameStatus == EGameState.Playing)
            {
                SaveGameState();
            }
        }

        protected virtual void OnApplicationQuit()
        {
            if (EventManager.GameStatus == EGameState.Playing)
            {
                SaveGameState();
            }
        }

        public virtual void OnScored(int scoreToAdd)
        {
            int previousScore = this.score;
            this.score += scoreToAdd;

            // Update UI immediately
            scoreText.text = score.ToString();

            if (_counterCoroutine != null)
            {
                StopCoroutine(_counterCoroutine);
            }
            _counterCoroutine = StartCoroutine(CountScore(previousScore, this.score));
        }

        protected IEnumerator CountScore(int startValue, int endValue)
        {
            _displayedScore = startValue;

            float actualSpeed = counterSpeed;
            if (endValue - startValue > 100)
                actualSpeed = counterSpeed * 0.5f;
            else if (endValue - startValue > 500)
                actualSpeed = counterSpeed * 0.2f;

            while (_displayedScore < endValue)
            {
                _displayedScore++;
                scoreText.text = _displayedScore.ToString();
                yield return new WaitForSeconds(actualSpeed);
            }

            _displayedScore = endValue;
            scoreText.text = endValue.ToString();
        }

        public virtual void OnLose()
        {
            DeleteGameState();
        }

        public virtual void UpdateScore(int newScore)
        {
            int previousScore = this.score;
            this.score = newScore;
            
            // Update UI immediately
            scoreText.text = score.ToString();
            
            // Animate the change
            if (_counterCoroutine != null)
            {
                StopCoroutine(_counterCoroutine);
            }
            _counterCoroutine = StartCoroutine(CountScore(previousScore, this.score));
        }

        public virtual void ResetScore()
        {
            // Stop any ongoing score animation
            if (_counterCoroutine != null)
            {
                StopCoroutine(_counterCoroutine);
                _counterCoroutine = null;
            }

            // Reset score and displayed score
            score = 0;
            _displayedScore = 0;

            // Update UI
            scoreText.text = "0";

            // Delete the game state since we're resetting
            DeleteGameState();
        }

        protected abstract void LoadScores();
        protected abstract void SaveGameState();
        protected abstract void DeleteGameState();
    }
}