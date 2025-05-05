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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Audio;
using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay.FX;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Managers;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Pool;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public partial class LevelManager : MonoBehaviour
    {
        public int currentLevel;
        public LineExplosion lineExplosionPrefab;
        public ComboText comboTextPrefab;
        public Transform pool;
        public Transform fxPool;

        public int comboCounter;
        private int missCounter;

        [SerializeField]
        private RectTransform gameCanvas;

        [SerializeField]
        private RectTransform shakeCanvas;

        [SerializeField]
        private GameObject scorePrefab;

        [SerializeField]
        private GameObject[] words;

        [SerializeField]
        private TutorialManager tutorialManager;

        [SerializeField]
        private GameObject timerPanel;

        public EGameMode gameMode;
        public Level _levelData;

        private Cell[] emptyCells;

        public UnityEvent<Level> OnLevelLoaded;
        public Action<int> OnScored;
        public Action OnLose;
        private FieldManager field;
        public CellDeckManager cellDeck;
        private ItemFactory itemFactory;
        private TargetManager targetManager;

        private ObjectPool<ComboText> comboTextPool;
        private ObjectPool<LineExplosion> lineExplosionPool;
        private ObjectPool<ScoreText> scoreTextPool;
        private ObjectPool<GameObject> wordsPool;
        private ClassicModeHandler classicModeHandler;
        private TimedModeHandler timedModeHandler;
        public TimerManager timerManager;
        private int timerDuration;
        
        private Vector3 cachedFieldCenter;
        private bool isFieldCenterCached;

        private void OnEnable()
        {
            StateManager.instance.CurrentState = EScreenStates.Game;
            EventManager.GetEvent(EGameEvent.RestartLevel).Subscribe(RestartLevel);
            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Subscribe(CheckLines);
            EventManager.OnGameStateChanged += HandleGameStateChange;
            targetManager = FindObjectOfType<TargetManager>();
            itemFactory = FindObjectOfType<ItemFactory>();
            cellDeck = FindObjectOfType<CellDeckManager>();
            field = FindObjectOfType<FieldManager>();
            // Get or add the TimerManager component
            timerManager = GetComponent<TimerManager>();
            if (timerManager == null)
            {
                timerManager = gameObject.AddComponent<TimerManager>();
            }

            if (timerManager != null && timerPanel != null)
            {
                timerManager.OnTimerExpired += OnTimerExpired;
            }

            comboTextPool = new ObjectPool<ComboText>(
                () => Instantiate(comboTextPrefab, fxPool),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );

            lineExplosionPool = new ObjectPool<LineExplosion>(
                () => Instantiate(lineExplosionPrefab, pool),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );

            scoreTextPool = new ObjectPool<ScoreText>(
                () => Instantiate(scorePrefab, fxPool).GetComponent<ScoreText>(),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );

            wordsPool = new ObjectPool<GameObject>(
                () => Instantiate(words[Random.Range(0, words.Length)], fxPool),
                obj => obj.SetActive(true),
                obj => obj.SetActive(false),
                Destroy
            );
            RestartLevel();
            if (gameMode == EGameMode.Classic)
                RestoreGameState();
            else if (gameMode == EGameMode.Timed)
                RestoreTimedGameState();
        }

        private void RestoreGameState()
        {
            var state = GameState.Load(EGameMode.Classic) as ClassicGameState;
            if (state != null)
            {
                GameManager.instance.Score = state.score;

                if (state.levelRows != null)
                {
                    var fieldManager = FindObjectOfType<FieldManager>();
                    if (fieldManager != null)
                    {
                        fieldManager.RestoreFromState(state.levelRows);
                    }
                }
            }
        }

        private void RestoreTimedGameState()
        {
            var state = GameState.Load(EGameMode.Timed) as TimedGameState;
            if (state != null)
            {
                Debug.Log($"Restoring timed game state with {(state.levelRows?.Length ?? 0)} rows");
                timedModeHandler = FindObjectOfType<TimedModeHandler>();
                if (timedModeHandler != null)
                {
                    timedModeHandler.score = state.score;
                    timedModeHandler.bestScore = state.bestScore;
                    
                    // Initialize timer with saved remaining time
                    if (timerManager != null)
                    {
                        timerManager.InitializeTimer(state.remainingTime);
                    }

                    // Restore field state if we have saved rows
                    if (state.levelRows != null && state.levelRows.Length > 0)
                    {
                        var fieldManager = FindObjectOfType<FieldManager>();
                        if (fieldManager != null)
                        {
                            Debug.Log("Restoring field state from saved state");
                            fieldManager.RestoreFromState(state.levelRows);
                        }
                        else
                        {
                            Debug.LogError("Could not find FieldManager component to restore field state");
                        }
                    }

                    // Let TimedModeHandler handle the timer start
                    timedModeHandler.ResumeGame();
                }
            }
            else
            {
                // If no saved state, start fresh timer
                if (timerManager != null && timedModeHandler != null)
                {
                    timerManager.InitializeTimer(GameManager.instance.GameSettings.globalTimedModeSeconds);
                }
            }
        }

        private void RestartLevel()
        {
            comboCounter = 0;
            missCounter = 0;
            field.ShowOutline(false);
            Load();
        }

        private void SaveGameState()
        {
            if (gameMode == EGameMode.Classic)
            {
                classicModeHandler = FindObjectOfType<ClassicModeHandler>();
                var state = new ClassicGameState
                {
                    score = classicModeHandler.score,
                    bestScore = classicModeHandler.bestScore,
                    gameMode = EGameMode.Classic,
                    gameStatus = EventManager.GameStatus
                };
                GameState.Save(state, field);
            }
            else if (gameMode == EGameMode.Timed)
            {
                timedModeHandler = FindObjectOfType<TimedModeHandler>();
                if (timedModeHandler != null)
                {
                    var state = new TimedGameState
                    {
                        score = timedModeHandler.score,
                        bestScore = timedModeHandler.bestScore,
                        remainingTime = timedModeHandler.GetRemainingTime(),
                        gameMode = EGameMode.Timed,
                        gameStatus = EventManager.GameStatus
                    };
                    GameState.Save(state, field);
                }
            }
        }

        private void OnDisable()
        {
            EventManager.GetEvent(EGameEvent.RestartLevel).Unsubscribe(RestartLevel);
            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Unsubscribe(CheckLines);
            EventManager.OnGameStateChanged -= HandleGameStateChange;

            // Unsubscribe from timer events
            if (timerManager != null)
            {
                timerManager.OnTimerExpired -= OnTimerExpired;
            }
        }

        private void OnTimerExpired()
        {
            // Check if level is complete before triggering a loss
            if (targetManager != null && targetManager.IsLevelComplete())
            {
                // Level complete, trigger win
                SetWin();
            }
            else
            {
                // Level not complete, trigger loss
                SetLose();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if ((gameMode == EGameMode.Classic || gameMode == EGameMode.Timed) && EventManager.GameStatus == EGameState.Playing)
                SaveGameState();

            PauseTimer(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            if ((gameMode == EGameMode.Classic || gameMode == EGameMode.Timed) && EventManager.GameStatus == EGameState.Playing)
                SaveGameState();
        }

        private void Load()
        {
            if (GameManager.instance.IsTutorialMode())
            {
                _levelData = tutorialManager.GetLevelForPhase();
            }
            else
            {
                gameMode = GameDataManager.GetGameMode();
                _levelData = GameDataManager.GetLevel();
                currentLevel = _levelData.Number;
            }
            if(_levelData == null)
            {
                Debug.LogError("Level data is null");
                return;
            }

            // Apply global time settings if timed mode is enabled
            if (GameManager.instance.GameSettings.enableTimedMode && _levelData.enableTimer)
            {
                timerDuration = _levelData.timerDuration;
                if(_levelData.timerDuration == 0)
                    timerDuration = GameManager.instance.GameSettings.globalTimedModeSeconds;
            }

            FindObjectsOfType<MonoBehaviour>().OfType<IBeforeLevelLoadable>().ToList().ForEach(x => x.OnLevelLoaded(_levelData));
            LoadLevel(_levelData);
            FindObjectsOfType<MonoBehaviour>().OfType<ILevelLoadable>().ToList().ForEach(x => x.OnLevelLoaded(_levelData));
            Invoke(nameof(StartGame), 0.5f);
            if (GameManager.instance.IsTutorialMode())
            {
                tutorialManager.StartTutorial();
            }

            // Initialize timer if enabled for this level or if global timed mode is enabled
            if (_levelData.enableTimer && timerManager != null)
            {
                timerManager.InitializeTimer(timerDuration);
                if (timerPanel != null)
                {
                    timerPanel.SetActive(true);
                }
            }
            else if (timerManager != null)
            {
                timerManager.StopTimer();
                if (timerPanel != null)
                {
                    timerPanel.SetActive(false);
                }
            }
        }

        private void StartGame()
        {
            EventManager.GameStatus = EGameState.PrepareGame;
            classicModeHandler = FindObjectOfType<ClassicModeHandler>();
        }

        private void LoadLevel(Level levelData)
        {
            field.Generate(levelData);
            // Reset field center cache when loading new level
            isFieldCenterCached = false;
            EventManager.GetEvent<Level>(EGameEvent.LevelLoaded).Invoke(levelData);
            OnLevelLoaded?.Invoke(levelData);
        }

        private void CheckLines(Shape obj)
        {
            var lines = field.GetFilledLines(false, false);
            if (lines.Count > 0)
            {
                comboCounter++;
                shakeCanvas.DOShakePosition(0.2f, 35f, 50);
                StartCoroutine(AfterMoveProcessing(obj, lines));
                if (comboCounter > 1)
                {
                    field.ShowOutline(true);
                }
            }
            else
            {
                missCounter++;
                if (missCounter >= GameManager.instance.GameSettings.ResetComboAfterMoves)
                {
                    field.ShowOutline(false);
                    missCounter = 0;
                    comboCounter = 0;
                }

                StartCoroutine(CheckLose());
            }
        }

        private Vector3 GetFieldCenter()
        {
            if (isFieldCenterCached)
                return cachedFieldCenter;

            Vector3 fieldCenter = Vector3.zero;
            int rowCount = field.cells.GetLength(0);
            int colCount = field.cells.GetLength(1);
            
            if (rowCount > 0 && colCount > 0)
            {
                Cell centerCell = field.cells[rowCount/2, colCount/2];
                if (centerCell != null)
                {
                    fieldCenter = centerCell.transform.position;
                }
            }
            
            cachedFieldCenter = fieldCenter;
            isFieldCenterCached = true;
            return fieldCenter;
        }

        private void ShowComboText(int comboCount)
        {
            Vector3 center = GetFieldCenter();
            Vector3 comboPosition = center + new Vector3(0, 0.75f, 0); // Same height as score text
            var comboText = comboTextPool.Get();
            comboText.transform.position = comboPosition;
            comboText.Show(comboCount);
            DOVirtual.DelayedCall(0.75f, () => { comboTextPool.Release(comboText); }); // Adjusted to match faster animation
        }

        private IEnumerator AfterMoveProcessing(Shape shape, List<List<Cell>> lines)
        {
            Vector3 center = GetFieldCenter();
            Vector3 scorePosition = center + new Vector3(0, 0.75f, 0); // Move score higher
            Vector3 gratzPosition = center + new Vector3(0, 0.35f, 0); // Position gratz between score and center

            yield return new WaitForSeconds(0.1f);
            if (gameMode == EGameMode.Adventure)
            {
                StartCoroutine(targetManager.AnimateTarget(lines));
            }

            yield return StartCoroutine(DestroyLines(lines, shape));

            var scoreTarget = GameManager.instance.GameSettings.ScorePerLine * lines.Count * comboCounter;
            OnScored?.Invoke(scoreTarget);
            if (gameMode == EGameMode.Adventure)
            {
                targetManager.UpdateScoreTarget(scoreTarget);
            }
            
            // Show combo first if active
            if (comboCounter > 1)
            {
                ShowComboText(comboCounter);
                yield return new WaitForSeconds(0.5f);
            }

            // Then show score at higher position
            var scoreText = scoreTextPool.Get();
            scoreText.transform.position = scorePosition;
            scoreText.ShowScore(scoreTarget, scorePosition);
            DOVirtual.DelayedCall(0.75f, () => { scoreTextPool.Release(scoreText); }); // Halved from 1.5f to match faster animation

            // Show congratulatory words below score
            if (Random.Range(0, 3) == 0)
            {
                var txt = wordsPool.Get();
                txt.transform.position = gratzPosition;

                // Ensure txt is within the bounds of the gameCanvas
                var canvasCorners = new Vector3[4];
                gameCanvas.GetWorldCorners(canvasCorners);

                var txtPosition = txt.transform.position;
                txtPosition.x = Mathf.Clamp(txtPosition.x, canvasCorners[0].x, canvasCorners[2].x);
                txtPosition.y = Mathf.Clamp(txtPosition.y, canvasCorners[0].y, canvasCorners[2].y);
                txt.transform.position = txtPosition;

                DOVirtual.DelayedCall(1.5f, () => { wordsPool.Release(txt); });
            }

            if (EventManager.GameStatus == EGameState.Playing)
                yield return StartCoroutine(CheckLose());
        }

        private IEnumerator CheckLose()
        {
            if (gameMode != EGameMode.Classic && targetManager != null && targetManager.WillLevelBeComplete())
            {
                EventManager.GameStatus = EGameState.WinWaiting;
            }

            yield return new WaitForSeconds(0.5f); // Keep a small delay for game flow
            var lose = true;
            var availableShapes = cellDeck.GetShapes();
            foreach (var shape in availableShapes)
            {
                if (field.CanPlaceShape(shape))
                {
                    lose = false;
                    break;
                }
            }
            
            if (gameMode != EGameMode.Classic && targetManager != null && targetManager.WillLevelBeComplete())
            {
                yield return new WaitForSeconds(0.5f);
                SetWin();
                lose = false;
            }

            if (lose)
            {
                SetLose();
            }

            yield return null;
        }

        private void SetWin()
        {
            GameDataManager.UnlockLevel(currentLevel + 1);
            EventManager.GameStatus = EGameState.PreWin;
        }

        private void SetLose()
        {
            if (gameMode == EGameMode.Classic)
                GameState.Delete(EGameMode.Classic);
            else if (gameMode == EGameMode.Timed)
                GameState.Delete(EGameMode.Timed);
            OnLose?.Invoke();
            EventManager.GameStatus = EGameState.PreFailed;
        }

        public IEnumerator EndAnimations(Action action)
        {
            yield return StartCoroutine(FillEmptyCellsFailed());
            action?.Invoke();
        }

        private IEnumerator FillEmptyCellsFailed()
        {
            SoundBase.instance.PlaySound(SoundBase.instance.fillEmpty);
            var template = Resources.Load<ItemTemplate>("Items/ItemTemplate 0");
            emptyCells = field.GetEmptyCells();
            foreach (var cell in emptyCells)
            {
                cell.FillCellFailed(template);
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void ClearEmptyCells()
        {
            foreach (var cell in emptyCells)
            {
                cell.ClearCell();
            }
        }

        private IEnumerator DestroyLines(List<List<Cell>> lines, Shape shape)
        {
            SoundBase.instance.PlayLimitSound(SoundBase.instance.combo[Mathf.Min(comboCounter, SoundBase.instance.combo.Length - 1)]);
            EventManager.GetEvent<Shape>(EGameEvent.LineDestroyed).Invoke(shape);

            // Mark cells as destroying immediately at the start
            foreach (var line in lines)
            {
                foreach (var cell in line)
                {
                    cell.SetDestroying(true);
                }
            }
            
            foreach (var line in lines)
            {
                if (line.Count == 0) continue;
                
                var lineExplosion = lineExplosionPool.Get();
                lineExplosion.Play(line, shape, RectTransformUtils.GetMinMaxAndSizeForCanvas(line, gameCanvas.GetComponent<Canvas>()), GetExplosionColor(shape));
                DOVirtual.DelayedCall(1.5f, () => { lineExplosionPool.Release(lineExplosion); });
                foreach (var cell in line)
                {
                    cell.DestroyCell();
                }
            }
            
            yield return null;
        }

        private Color GetExplosionColor(Shape shape)
        {
            var itemTemplateTopColor = shape.GetActiveItems()[0].itemTemplate.overlayColor;
            if (_levelData.levelType.singleColorMode)
            {
                itemTemplateTopColor = itemFactory.GetOneColor().overlayColor;
            }

            return itemTemplateTopColor;
        }

        private void Update()
        {
            if (Keyboard.current != null)
            {
                // Debug keys for win/lose
                if(Keyboard.current[GameManager.instance.debugSettings.Win].wasPressedThisFrame)
                {
                    SetWin();
                }

                if(Keyboard.current[GameManager.instance.debugSettings.Lose].wasPressedThisFrame)
                {
                    SetLose();
                }

                // Other debug keys
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    // Fill the first row with tiles
                    var rowCells = new List<Cell>();
                    for (int col = 0; col < field.cells.GetLength(1); col++)
                    {
                        rowCells.Add(field.cells[0, col]);
                    }

                    var itemTemplate = Resources.Load<ItemTemplate>("Items/ItemTemplate 0");
                    
                    // Get all available bonus items from the level data
                    var availableBonuses = _levelData.targetInstance
                        .Where(t => t.targetScriptable.bonusItem != null)
                        .Select(t => t.targetScriptable.bonusItem)
                        .ToList();

                    foreach (var cell in rowCells)
                    {
                        if (cell != null && cell.IsEmpty())
                        {
                            cell.FillCell(itemTemplate);
                            
                            // 30% chance to add a bonus to the cell
                            if (availableBonuses.Count > 0 && Random.Range(0f, 1f) < 0.3f)
                            {
                                var randomBonus = availableBonuses[Random.Range(0, availableBonuses.Count)];
                                cell.SetBonus(randomBonus);
                            }
                        }
                    }

                    // Increment combo and show effects
                    comboCounter++;
                    field.ShowOutline(true);
                    
                    // Calculate score for a full row
                    int scoreToAdd = GameManager.instance.GameSettings.ScorePerLine * comboCounter;
                    
                    // Add score based on game mode
                    if (gameMode == EGameMode.Classic)
                    {
                        if (classicModeHandler != null)
                            classicModeHandler.UpdateScore(classicModeHandler.score + scoreToAdd);
                    }
                    else if (gameMode == EGameMode.Timed)
                    {
                        if (timedModeHandler != null)
                            timedModeHandler.UpdateScore(timedModeHandler.score + scoreToAdd);
                    }

                    // Create a dummy shape for the animation position
                    var dummyShape = itemFactory.CreateRandomShape(null, PoolObject.GetObject(cellDeck.shapePrefab.gameObject));
                    dummyShape.transform.position = rowCells[0].transform.position;
                    
                    // Screen shake effect
                    shakeCanvas.DOShakePosition(0.2f, 35f, 50);
                    
                    // Process the row destruction with proper animations
                    StartCoroutine(AfterMoveProcessing(dummyShape, new List<List<Cell>> { rowCells }));
                    
                    // Clean up the dummy shape
                    Destroy(dummyShape.gameObject);
                }

                // Use the configurable UpdateDeck key from debug settings instead of hardcoded dKey
                if (Keyboard.current[GameManager.instance.debugSettings.UpdateDeck].wasPressedThisFrame)
                {
                    cellDeck.ClearCellDecks();
                    cellDeck.FillCellDecks();
                }

                if (Keyboard.current.aKey.wasPressedThisFrame)
                {
                    StartCoroutine(CheckLose());
                }

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    GameManager.instance.RestartLevel();
                }
            }
        }

        public Level GetCurrentLevel()
        {
            return _levelData;
        }

        public EGameMode GetGameMode()
        {
            return gameMode;
        }

        public FieldManager GetFieldManager()
        {
            return field;
        }

        public void PauseTimer(bool pause)
        {
            if (timerManager != null)
            {
                timerManager.PauseTimer(pause);
            }
        }
    }
}