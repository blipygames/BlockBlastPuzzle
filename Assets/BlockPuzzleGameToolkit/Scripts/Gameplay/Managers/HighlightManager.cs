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

using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay.FX;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEngine;
using UnityEngine.Pool;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class HighlightManager : MonoBehaviour
    {
        private static readonly Dictionary<Item, ObjectPool<Item>> CustomItemPools = new();
        private readonly Dictionary<Cell, Item> highlightedCells = new();
        private readonly List<List<Cell>> highlightedFillCells = new();
        private readonly List<Outline> activeOutlines = new();
        private readonly Dictionary<Cell, Item> customPrefabHighlights = new();

        public Outline outlinePrefab;
        public Canvas canvas;

        private readonly Dictionary<Cell, Outline> cellToOutlineMap = new();
        private ObjectPool<GameObject> outlinePool;

        private void OnEnable()
        {
            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Subscribe(OnShapePlaced);
            outlinePool = new ObjectPool<GameObject>(
                () => Instantiate(outlinePrefab.gameObject, transform),
                obj => obj.SetActive(true),
                obj => obj.SetActive(false),
                Destroy
            );
        }

        private void OnDisable()
        {
            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Unsubscribe(OnShapePlaced);
        }

        private void OnShapePlaced(Shape obj)
        {
            highlightedFillCells.Clear();
            highlightedCells.Clear();

            ClearOutline();
        }

        private void ClearOutline()
        {
            foreach (var outline in activeOutlines)
            {
                outlinePool.Release(outline.gameObject);
            }

            activeOutlines.Clear();
            cellToOutlineMap.Clear();
            
            // Destroy any custom prefab highlights
            foreach (var highlightItem in customPrefabHighlights.Values)
            {
                if (highlightItem != null)
                {
                    GetOrCreatePool(highlightItem.GetComponent<Item>()).Release(highlightItem);
                }
            }
            customPrefabHighlights.Clear();
        }

        public void HighlightCell(Transform cell, Item item)
        {
            var cellComponent = cell.GetComponent<Cell>();
            if (cellComponent == null || !highlightedCells.TryAdd(cellComponent, item))
                return;

            cellComponent.HighlightCell(item.itemTemplate);
        }

        private void HighlightFill(List<Cell> cellTransforms, ItemTemplate itemTemplate)
        {
            var firstCell = cellTransforms[0];
            Outline outline;

            if (!cellToOutlineMap.TryGetValue(firstCell, out outline))
            {
                outline = outlinePool.Get().GetComponent<Outline>();
                cellToOutlineMap[firstCell] = outline;
                activeOutlines.Add(outline);
            }

            var fillCells = cellTransforms;
            if (!highlightedFillCells.Contains(fillCells))
            {
                highlightedFillCells.Add(fillCells);
            }

            foreach (var cellTransform in cellTransforms)
            {
                if (highlightedCells.ContainsKey(cellTransform))
                {
                    continue;
                }

                cellTransform.HighlightCellFill(itemTemplate);
            }

            UpdateOutline(outline, itemTemplate.topColor, fillCells);
        }

        public void HighlightFill(List<List<Cell>> filledCells, ItemTemplate itemTemplate)
        {
            var firstCells = new List<Cell>();
            foreach (var line in filledCells)
            {
                if (line.Count > 0)
                {
                    firstCells.Add(line[0]);
                    HighlightFill(line, itemTemplate);
                }
            }

            //check cellToOutlineMap for any outlines that are not in filledCells
            foreach (var kvp in cellToOutlineMap)
            {
                if (!firstCells.Contains(kvp.Key))
                {
                    outlinePool.Release(kvp.Key.gameObject);
                    activeOutlines.Remove(kvp.Value);
                }
            }
        }

        private void UpdateOutline(Outline outline, Color color, List<Cell> highlightedFillCells)
        {
            if (highlightedFillCells.Count == 0)
            {
                outlinePool.Release(outline.gameObject);
                activeOutlines.Remove(outline);
                return;
            }

            var (min, max, sizeInLocalSpace, centerLocalPoint) = RectTransformUtils.GetMinMaxAndSizeForCanvas(highlightedFillCells, canvas);
            var padding = 40f;
            sizeInLocalSpace += new Vector2(padding, padding);

            outline.Init(centerLocalPoint, sizeInLocalSpace, color);
        }

        public void ClearAllHighlights()
        {
            foreach (var cell in highlightedFillCells)
            {
                foreach (var cellImage in cell)
                {
                    cellImage.ClearCell();
                    
                    // Remove any custom prefab highlights
                    if (customPrefabHighlights.TryGetValue(cellImage, out var highlight))
                    {
                        GetOrCreatePool(highlight.GetComponent<Item>()).Release(highlight);
                    }
                }
            }

            highlightedFillCells.Clear();

            foreach (var kvp in highlightedCells)
            {
                var cellImage = kvp.Key;
                if (cellImage != null)
                {
                    cellImage.ClearCell();
                    
                    // Remove any custom prefab highlights
                    if (customPrefabHighlights.TryGetValue(cellImage, out var highlight))
                    {
                        GetOrCreatePool(highlight.GetComponent<Item>()).Release(highlight);
                    }
                }
            }

            ClearOutline();
            highlightedCells.Clear();
            customPrefabHighlights.Clear();
        }

        public Dictionary<Cell, Item> GetHighlightedCells()
        {
            return highlightedCells;
        }

        public void OnDragEndedWithoutPlacement()
        {
            ClearOutline();
        }

        private ObjectPool<Item> GetOrCreatePool(Item prefab)
        {
            if (!CustomItemPools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool<Item>(
                    createFunc: () => Instantiate(prefab),
                    actionOnGet: item => item.gameObject.SetActive(true),
                    actionOnRelease: item => item.gameObject.SetActive(false),
                    actionOnDestroy: item => Destroy(item.gameObject)
                );
                CustomItemPools[prefab] = pool;
            }
            return pool;
        }
    }
}