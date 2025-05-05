using System;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.System
{
    [Serializable]
    public abstract class GameState
    {
        public EGameState gameStatus;
        public int currentLevel;
        public EGameMode gameMode;
        public int score;
        public LevelRow[] levelRows;
        public DateTime quitTime;
        public int bestScore;

        public static void Save(GameState state, FieldManager field)
        {
            if (state == null) return;
            
            if (field != null)
            {
                var cells = field.GetAllCells();
                state.levelRows = new LevelRow[cells.GetLength(0)];
                
                for (var i = 0; i < cells.GetLength(0); i++)
                {
                    state.levelRows[i] = new LevelRow(cells.GetLength(1));
                    for (var j = 0; j < cells.GetLength(1); j++) 
                    {
                        if (cells[i, j].item != null && !cells[i, j].IsEmpty())
                        {
                            state.levelRows[i].cells[j] = cells[i, j].item?.itemTemplate;
                            state.levelRows[i].bonusItems[j] = cells[i, j].HasBonusItem();
                            state.levelRows[i].disabled[j] = cells[i, j].IsDisabled();
                        }
                    }
                }
            }
            
            state.quitTime = DateTime.Now;
            
            var json = JsonUtility.ToJson(state);
            string key = "GameState_" + state.gameMode;
            PlayerPrefs.SetString(key, json);
            
            // Also save the current game mode
            PlayerPrefs.SetString("LastPlayedMode", state.gameMode.ToString());
            PlayerPrefs.Save();
        }

        public static GameState Load(EGameMode gameMode)
        {
            string key = "GameState_" + gameMode;
            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                GameState state = null;
                
                try
                {
                    switch (gameMode)
                    {
                        case EGameMode.Classic:
                            state = JsonUtility.FromJson<ClassicGameState>(json);
                            break;
                        case EGameMode.Timed:
                            state = JsonUtility.FromJson<TimedGameState>(json);
                            break;
                    }
                    
                    // Validate loaded state
                    if (state != null && state.gameMode != gameMode)
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading game state: {e.Message}");
                    return null;
                }
                
                return state;
            }
            return null;
        }

        public static GameState Load()
        {
            // Legacy loading for backward compatibility
            if (PlayerPrefs.HasKey("GameState"))
            {
                var json = PlayerPrefs.GetString("GameState");
                var tempState = JsonUtility.FromJson<LegacyGameState>(json);
                
                // Convert to appropriate state based on gameMode
                switch (tempState.gameMode)
                {
                    case EGameMode.Classic:
                        var classicState = new ClassicGameState();
                        CopyBaseProperties(tempState, classicState);
                        return classicState;
                    case EGameMode.Timed:
                        var timedState = new TimedGameState();
                        CopyBaseProperties(tempState, timedState);
                        timedState.remainingTime = tempState.remainingTime;
                        return timedState;
                    default:
                        return null;
                }
            }
            return null;
        }

        private static void CopyBaseProperties(LegacyGameState source, GameState target)
        {
            target.gameStatus = source.gameStatus;
            target.currentLevel = source.currentLevel;
            target.gameMode = source.gameMode;
            target.score = source.score;
            target.levelRows = source.levelRows;
            target.quitTime = source.quitTime;
            target.bestScore = source.bestScore;
        }

        public static void Delete(EGameMode gameMode)
        {
            PlayerPrefs.DeleteKey("GameState_" + gameMode);
            PlayerPrefs.Save();
        }

        public static void Delete()
        {
            // Delete legacy key
            PlayerPrefs.DeleteKey("GameState");
            
            // Delete all game mode specific keys
            foreach (EGameMode mode in Enum.GetValues(typeof(EGameMode)))
            {
                PlayerPrefs.DeleteKey("GameState_" + mode);
            }
            
            PlayerPrefs.Save();
        }
    }

    [Serializable]
    public class ClassicGameState : GameState
    {
        public int level;

        public ClassicGameState()
        {
            gameMode = EGameMode.Classic;
        }
    }

    [Serializable]
    public class TimedGameState : GameState
    {
        public float remainingTime;

        public TimedGameState()
        {
            gameMode = EGameMode.Timed;
            remainingTime = 180f; // Default duration if not set
            score = 0;
            bestScore = 0;
        }

        public void SetBestScore(int newScore)
        {
            if (remainingTime <= 0 && newScore > bestScore)
            {
                bestScore = newScore;
            }
        }
    }

    [Serializable]
    public class LegacyGameState
    {
        // For backwards compatibility when loading old saved states
        public EGameState gameStatus;
        public int currentLevel;
        public EGameMode gameMode;
        public int score;
        public int remainingTime;
        public LevelRow[] levelRows;
        public DateTime quitTime;
        public int bestScore;
    }
}
