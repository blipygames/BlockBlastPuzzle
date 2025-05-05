using UnityEngine;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Popups;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData
{
    public abstract class LevelStateHandler : ScriptableObject
    {
        public virtual void HandleState(EGameState state, LevelManager levelManager)
        {
            switch (state)
            {
                case EGameState.PrepareGame:
                    HandlePrepareGame(levelManager);
                    break;
                case EGameState.Playing:
                    HandlePlaying(levelManager);
                    break;
                case EGameState.PreFailed:
                    HandlePreFailed(levelManager);
                    break;
                case EGameState.Failed:
                    HandleFailed(levelManager);
                    break;
                case EGameState.PreWin:
                    HandlePreWin(levelManager);
                    break;
                case EGameState.Win:
                    HandleWin(levelManager);
                    break;
            }
        }
        
        private protected virtual void HandlePrepareGame(LevelManager levelManager)
        {
            var level = levelManager.GetCurrentLevel();
            var prePlayPopup = level.levelType.prePlayPopup;

            if (prePlayPopup != null)
            {
                MenuManager.instance.ShowPopup(prePlayPopup, null, _ => EventManager.GameStatus = EGameState.Playing);
            }
            else
            {
                EventManager.GameStatus = EGameState.Playing;
            }
        }

        private protected virtual void HandlePlaying(LevelManager levelManager) {}

        private protected virtual void HandlePreFailed(LevelManager levelManager) {}

        private protected virtual void HandleFailed(LevelManager levelManager)
        {
            var failedPopup = levelManager.GetCurrentLevel().levelType.failedPopup;
            if (failedPopup != null)
            {
                MenuManager.instance.ShowPopup(failedPopup);
            }
        }

        private protected virtual void HandlePreWin(LevelManager levelManager)
        {
            var preWinPopup = levelManager.GetCurrentLevel().levelType.preWinPopup;
            if (preWinPopup != null)
            {
                MenuManager.instance.ShowPopupDelayed(preWinPopup, null, _ => EventManager.GameStatus = EGameState.Win);
            }
            else
            {
                EventManager.GameStatus = EGameState.Win;
            }
        }

        private protected virtual void HandleWin(LevelManager levelManager)
        {
            var winPopup = levelManager.GetCurrentLevel().levelType.winPopup;
            if (winPopup != null)
            {
                MenuManager.instance.ShowPopup(winPopup);
            }
            else
            {
                GameManager.instance.OpenMap();
            }
        }
    }
} 