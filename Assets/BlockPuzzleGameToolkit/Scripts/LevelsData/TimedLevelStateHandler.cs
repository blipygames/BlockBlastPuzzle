using UnityEngine;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Popups;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData
{
    // [CreateAssetMenu(fileName = "TimedStateHandler", menuName = "BlockPuzzleGameToolkit/Levels/TimedStateHandler")]
    public class TimedLevelStateHandler : LevelStateHandler
    {
        private protected override void HandlePreFailed(LevelManager levelManager)
        {
            var level = levelManager.GetCurrentLevel();
            var preFailedPopup = level.levelType.preFailedPopup;
            levelManager.timerManager?.StopTimer();

            levelManager.StartCoroutine(levelManager.EndAnimations(() =>
            {
                if (levelManager.timerManager != null && levelManager.timerManager.RemainingTime <= 0)
                {
                    EventManager.GameStatus = EGameState.Failed;
                    return;
                }

                if (preFailedPopup != null && GameManager.instance.GameSettings.enablePreFailedPopup)
                {
                    MenuManager.instance.ShowPopup(preFailedPopup, levelManager.ClearEmptyCells, result =>
                    {
                        if (result == EPopupResult.Continue)
                        {
                            levelManager.cellDeck.UpdateCellDeckAfterFail();
                            levelManager.timerManager?.InitializeTimer(levelManager.timerManager.RemainingTime );
                            EventManager.GameStatus = EGameState.Playing;
                        }
                    });
                }
                else
                {
                    
                    EventManager.GameStatus = EGameState.Failed;
                }
            }));
        }

        private protected override void HandlePreWin(LevelManager levelManager)
        {
            levelManager.timerManager?.StopTimer();
            base.HandlePreWin(levelManager);
        }
    }
}