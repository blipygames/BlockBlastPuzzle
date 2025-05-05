using UnityEngine;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Popups;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData
{
    // [CreateAssetMenu(fileName = "ClassicStateHandler", menuName = "BlockPuzzleGameToolkit/Levels/ClassicStateHandler")]
    public class ClassicLevelStateHandler : LevelStateHandler
    {
        private protected override void HandlePreFailed(LevelManager levelManager)
        {
            var level = levelManager.GetCurrentLevel();
            var preFailedPopup = level.levelType.preFailedPopup;
            levelManager.timerManager?.StopTimer();

            levelManager.StartCoroutine(levelManager.EndAnimations(() =>
            {
                if (preFailedPopup != null && GameManager.instance.GameSettings.enablePreFailedPopup)
                {
                    MenuManager.instance.ShowPopup(preFailedPopup, levelManager.ClearEmptyCells, result =>
                    {
                        if (result == EPopupResult.Continue)
                        {
                            levelManager.cellDeck.UpdateCellDeckAfterFail();
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
    }
}