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
using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.Popups;
using BlockPuzzleGameToolkit.Scripts.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap
{
    public class ScrollableMapManager : SingletonBehaviour<ScrollableMapManager>
    {
        public LevelPin levelPrefab;

        [SerializeField] private GameObject circlePrefab;
        [SerializeField] private Transform levelsGrid;
        [SerializeField] private ContentStretchController contentStretchController;
        [SerializeField] private MapDecorator mapDecorator;

        [SerializeField] private ScrollMap scrollMap;
        [SerializeField] private CustomButton backButton;
        [SerializeField, Tooltip("Spacing between repeated level segments"), Range(0f, 100f)] private float levelSegmentGap = 0f;
        [SerializeField, Tooltip("Number of times to repeat the level segment"), Range(1, 10)] private int levelSegmentRepetitions = 3;
        private List<LevelPin> openedLevels = new List<LevelPin>();
        private Vector3[] debugPath;
        private Rect _mapBounds;
        private Rect _entireMapBounds;

        private void OnEnable()
        {
            UpdateLevelPinsAfterWin();
        }

        private void Start()
        {
            backButton.onClick.AddListener(SceneLoader.instance.GoMain);
            var lvls = FindObjectsOfType<LevelPin>().OrderBy(x => x.number).ToArray();
            var lastLevel = GameDataManager.GetLevelNum();

            List<Vector3> fullPathPoints = new List<Vector3>();
            openedLevels.Clear();
            
            HashSet<int> existingLevelNumbers = new HashSet<int>();
            
            foreach (var levelPin in lvls)
            {
                levelPin.name = $"Level_{levelPin.number}";
                fullPathPoints.Add(levelPin.transform.position);
                existingLevelNumbers.Add(levelPin.number);
                
                if (levelPin.number > lastLevel)
                {
                    levelPin.Lock();
                }
                else
                {
                    levelPin.UnLock();
                    openedLevels.Add(levelPin);
                    levelPin.SetCurrent(levelPin.number == lastLevel);
                }
            }
            
            // Get the total level count from Resources
            int totalLevelsInResources = Resources.LoadAll<LevelsData.Level>("Levels").Length;
            int baseLevelCount = lvls.Length;
            
            // Calculate how many repetitions we need to cover all available levels
            int requiredRepetitions = Mathf.CeilToInt((float)(totalLevelsInResources - baseLevelCount) / baseLevelCount);
            requiredRepetitions = Mathf.Clamp(requiredRepetitions, 1, 20); // Reasonable limit
            
            Debug.Log($"Total levels in resources: {totalLevelsInResources}, Base levels: {baseLevelCount}, Required repetitions: {requiredRepetitions}");
            
            // Duplicate levels vertically
            if (lvls.Length > 0)
            {
                RepeatLevelsVertically(lvls, requiredRepetitions);
            }
            
            // Calculate the path with all level positions
            LevelPin[] allLevels = FindObjectsOfType<LevelPin>().OrderBy(x => x.transform.position.y).ToArray();
            debugPath = allLevels.Select(l => l.transform.position).ToArray();

            contentStretchController.HandleLastLevelPositionUpdate((Vector2)debugPath[debugPath.Length - 1]);
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
                    
            scrollMap.ScrollToAvatar(GetPositionOpenedLevel());

            if (openedLevels.Count > 0)
            {
                var existingMarker = levelsGrid.Find("CurrentLevelMarker(Clone)");
                
                if (existingMarker == null)
                {
                    var circle = Instantiate(circlePrefab, GetPositionOpenedLevel(), Quaternion.identity, levelsGrid);
                    circle.name = "CurrentLevelMarker";
                }
                else
                {
                    existingMarker.transform.position = GetPositionOpenedLevel();
                }
            }
            
            CalculateMapBounds();
            
            if (mapDecorator == null)
            {
                mapDecorator = gameObject.AddComponent<MapDecorator>();
            }
            
            mapDecorator.Initialize(levelsGrid);
            mapDecorator.SetMapBounds(_entireMapBounds);
            // We'll handle decoration placement in RepeatLevelsVertically
            // instead of calling PlaceDecorativeImages() here
        }

        private void RepeatLevelsVertically(LevelPin[] originalLevels, int repetitions)
        {
            if (originalLevels.Length == 0) return;
            
            // Find the height of the original level set
            float minY = originalLevels.Min(pin => pin.transform.position.y);
            float maxY = originalLevels.Max(pin => pin.transform.position.y);
            float setHeight = maxY - minY;
            
            // Get decorative items in the original segment
            Transform[] originalDecorations = FindDecorationsInSegment(minY, maxY);
            
            // Use the configurable gap setting
            float totalHeight = setHeight + levelSegmentGap;
            
            // Get total levels from Resources for level number validation
            int totalLevelsInResources = Resources.LoadAll<LevelsData.Level>("Levels").Length;
            int baseLevelCount = originalLevels.Length;
            var lastLevel = GameDataManager.GetLevelNum();
            
            for (int repetition = 0; repetition < repetitions; repetition++)
            {
                float verticalOffset = totalHeight * (repetition + 1);
                
                // Duplicate level pins
                foreach (var originalPin in originalLevels)
                {
                    // Calculate the new level number
                    int newLevelNumber = baseLevelCount * (repetition + 1) + originalPin.number;
                    
                    // Skip if the level number exceeds total available levels
                    if (newLevelNumber > totalLevelsInResources)
                        continue;
                    
                    Vector3 newPosition = originalPin.transform.position + new Vector3(0, verticalOffset, 0);
                    var newPin = Instantiate(levelPrefab, newPosition, Quaternion.identity, levelsGrid);
                    
                    // Assign proper level number
                    newPin.name = $"Level_{newLevelNumber}";
                    newPin.SetNumber(newLevelNumber);
                    
                    // Set lock state based on current progress
                    if (newLevelNumber > lastLevel)
                    {
                        newPin.Lock();
                    }
                    else
                    {
                        newPin.UnLock();
                        if (!openedLevels.Contains(newPin))
                        {
                            openedLevels.Add(newPin);
                        }
                        newPin.SetCurrent(newLevelNumber == lastLevel);
                    }
                }
                
                // Duplicate decorations for this segment
                DuplicateDecorations(originalDecorations, verticalOffset);
            }
            
            // Now let MapDecorator handle any additional decorations
            if (mapDecorator != null)
            {
                mapDecorator.PlaceDecorativeImages();
            }
        }
        
        private Transform[] FindDecorationsInSegment(float minY, float maxY)
        {
            // Find all child objects in the grid that are not LevelPins
            // and are within the segment's vertical bounds
            List<Transform> decorations = new List<Transform>();
            
            foreach (Transform child in levelsGrid)
            {
                if (child.GetComponent<LevelPin>() == null && // Not a level pin
                    child.position.y >= minY && child.position.y <= maxY && // Within vertical bounds
                    !child.name.Contains("CurrentLevelMarker")) // Not the current level marker
                {
                    decorations.Add(child);
                }
            }
            
            return decorations.ToArray();
        }
        
        private void DuplicateDecorations(Transform[] originalDecorations, float verticalOffset)
        {
            foreach (Transform original in originalDecorations)
            {
                Vector3 newPosition = original.position + new Vector3(0, verticalOffset, 0);
                var copy = Instantiate(original.gameObject, newPosition, original.rotation, levelsGrid);
                copy.name = original.name + "_copy";
                
                // Maintain the scale
                copy.transform.localScale = original.localScale;
            }
        }

        private void OpenLevel()
        {
            var currentLevel = GameDataManager.GetLevelNum();
            SceneLoader.instance.StartGameScene(currentLevel);
        }

        private void CalculateMapBounds()
        {
            if (contentStretchController != null && contentStretchController.GetComponent<RectTransform>() != null)
            {
                RectTransform contentRect = contentStretchController.GetComponent<RectTransform>();
                
                Vector3[] corners = new Vector3[4];
                contentRect.GetWorldCorners(corners);
                
                float minX = corners.Min(c => c.x);
                float maxX = corners.Max(c => c.x);
                float minY = corners.Min(c => c.y);
                float maxY = corners.Max(c => c.y);
                
                _mapBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            
            if (debugPath != null && debugPath.Length > 0)
            {
                float minPathX = debugPath.Min(p => p.x);
                float maxPathX = debugPath.Max(p => p.x);
                float minPathY = debugPath.Min(p => p.y);
                float maxPathY = debugPath.Max(p => p.y);
                
                float marginX = 10f; // Fixed margin instead of amplitude
                float marginY = 50f;
                
                _entireMapBounds = new Rect(
                    minPathX - marginX,
                    minPathY - marginY,
                    (maxPathX - minPathX) + (marginX * 2),
                    (maxPathY - minPathY) + (marginY * 2)
                );
            }
            else
            {
                _entireMapBounds = _mapBounds;
            }
        }

        private Vector3 GetPositionOpenedLevel()
        {
            var currentLevel = GameDataManager.GetLevelNum();
            var currentLevelPin = openedLevels.FirstOrDefault(pin => pin.number == currentLevel);
            return currentLevelPin != null ? currentLevelPin.transform.position : openedLevels[^1].transform.position;
        }

        public void OpenLevel(int number)
        {
            SceneLoader.instance.StartGameScene(number);
        }
        
        public void UpdateLevelPinsAfterWin()
        {
            var lastLevel = GameDataManager.GetLevelNum();
            var allLevelPins = levelsGrid.GetComponentsInChildren<LevelPin>().OrderBy(x => x.number).ToArray();
            
            foreach (var levelPin in allLevelPins)
            {
                if (levelPin.number > lastLevel)
                {
                    levelPin.Lock();
                }
                else
                {
                    levelPin.UnLock();
                    if (!openedLevels.Contains(levelPin))
                    {
                        openedLevels.Add(levelPin);
                    }
                    levelPin.SetCurrent(levelPin.number == lastLevel);
                }
            }
            
            // Update the current level marker
            var existingMarker = levelsGrid.Find("CurrentLevelMarker(Clone)") ?? levelsGrid.Find("CurrentLevelMarker");
            if (existingMarker != null)
            {
                existingMarker.transform.position = GetPositionOpenedLevel();
            }
            
            scrollMap.ScrollToAvatar(GetPositionOpenedLevel());
        }
        
        // Decorator Convenience Methods
        public void SetDecorativeStartOffset(float offset)
        {
            if (mapDecorator != null)
            {
                mapDecorator.DecorationStartOffset = offset;
            }
        }
        
        public void SetDecorativeStepDistance(float distance)
        {
            if (mapDecorator != null)
            {
                mapDecorator.DecorationStepDistance = distance;
            }
        }
        
        public void SetDecorativePadding(float left, float right)
        {
            if (mapDecorator != null)
            {
                mapDecorator.LeftPadding = left;
                mapDecorator.RightPadding = right;
            }
        }
        
        public void UpdateDecorations(float startOffset, float stepDistance, float leftPadding, float rightPadding)
        {
            if (mapDecorator != null)
            {
                mapDecorator.UpdateDecorations(startOffset, stepDistance, leftPadding, rightPadding);
            }
        }
    }
}