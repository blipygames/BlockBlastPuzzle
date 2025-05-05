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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class FieldManager : MonoBehaviour
    {
        public RectTransform field;
        public Cell prefab;

        public Cell[,] cells;

        public RectTransform outline;

        [SerializeField]
        private ItemFactory itemFactory;

        private float _cellSize;

        public void Generate(Level level)
        {
            var oneColorMode = level.levelType.singleColorMode;

            if (level == null)
            {
                Debug.LogError("Attempted to generate field with null level");
                return;
            }

            GenerateField(level.rows, level.columns);

            for (var i = 0; i < level.rows; i++)
            {
                for (var j = 0; j < level.columns; j++)
                {
                    var item = level.GetItem(i, j);
                    if (item != null)
                    {
                        cells[i, j].FillCell(item);
                    }

                    var bonus = false;
                    if (level.levelRows[i].bonusItems[j])
                    {
                        var bonusItemTemplates = level.targetInstance.Where(t => t.amount > 0 && t.targetScriptable.bonusItem != null).Select(t => t.targetScriptable.bonusItem).ToArray();
                        if (bonusItemTemplates.Length > 0)
                        {
                            cells[i, j].SetBonus(bonusItemTemplates[Random.Range(0, bonusItemTemplates.Length)]);
                            bonus = true;
                        }
                    }

                    if (item != null && oneColorMode && !bonus)
                    {
                        cells[i, j].FillCell(itemFactory.GetColor());
                    }

                    // Disable cell if it is marked as disabled in the level data
                    if (level.IsDisabled(i, j))
                    {
                        cells[i, j].DisableCell();
                    }

                    // Highlight cell if it is marked as highlighted in the level data
                    if (level.IsCellHighlighted(i, j))
                    {
                        cells[i, j].HighlightCellTutorial();
                    }
                }
            }
        }

        private void GenerateField(int rows, int columns)
        {
            foreach (Transform child in field)
            {
                Destroy(child.gameObject);
            }

            cells = new Cell[rows, columns];

            // Configure grid layout
            var gridLayout = field.GetComponent<GridLayoutGroup>();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;

            // Calculate cell size to fit the field
            var totalMargin = gridLayout.padding.left + gridLayout.padding.right;
            var spacing = gridLayout.spacing;
            var availableWidth = field.rect.width - totalMargin;
            var availableHeight = field.rect.height - (gridLayout.padding.top + gridLayout.padding.bottom);
            
            // Calculate cell size to fit both dimensions
            var cellSizeFromWidth = (availableWidth - (spacing.x * (columns - 1))) / columns;
            var cellSizeFromHeight = (availableHeight - (spacing.y * (rows - 1))) / rows;
            var cellSize = Mathf.Min(cellSizeFromWidth, cellSizeFromHeight);
            
            gridLayout.cellSize = new Vector2(cellSize, cellSize);

            // Create cells
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    var cell = Instantiate(prefab, field);
                    cells[i, j] = cell;
                    cell.name = $"Cell {i}, {j}";
                    cell.InitItem();
                }
            }

            _cellSize = cellSize;
            
            // Start coroutine to resize field with delay
            StartCoroutine(ResizeFieldWithDelay(rows, columns, cellSize, gridLayout));
        }
        
        private IEnumerator ResizeFieldWithDelay(int rows, int columns, float cellSize, GridLayoutGroup gridLayout)
        {
            
            // Wait for additional frame to ensure grid layout is calculated
            yield return new WaitForFixedUpdate();
            
            // Calculate exact size based on grid layout
            var spacing = gridLayout.spacing;
            var padding = gridLayout.padding;
            float width = (cellSize * columns) + (spacing.x * (columns - 1)) + padding.left + padding.right;
            float height = (cellSize * rows) + (spacing.y * (rows - 1)) + padding.top + padding.bottom;
            
            // Resize field to exactly match the grid layout size
            GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }

        public void RestoreFromState(LevelRow[] levelRows)
        {
            //restore score
            if (levelRows == null) return;
            GenerateField(levelRows.Length, levelRows[0].cells.Length);
            for (var i = 0; i < levelRows.Length; i++)
            {
                for (var j = 0; j < levelRows[i].cells.Length; j++)
                {
                    var item = levelRows[i].cells[j];
                    if (item != null)
                    {
                        cells[i,j].FillCell(item);
                    }

                    if (levelRows[i].disabled[j])
                    {
                        cells[i,j].DisableCell();
                    }
                }
            }
        }

        public List<List<Cell>> GetFilledLines(bool preview = false, bool merge = true)
        {
            var horizontalLines = GetFilledLinesHorizontal(preview);
            var verticalLines = GetFilledLinesVertical(preview);

            var lines = new List<List<Cell>>();
            lines.AddRange(horizontalLines);
            lines.AddRange(verticalLines);
            return lines;
        }

        public List<List<Cell>> GetFilledLinesHorizontal(bool preview)
        {
            var lines = new List<List<Cell>>();
            for (var i = 0; i < cells.GetLength(0); i++)
            {
                var isLineFilled = true;
                var line = new List<Cell>();
                for (var j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].IsEmpty(preview))
                    {
                        isLineFilled = false;
                        break;
                    }

                    line.Add(cells[i, j]);
                }

                if (isLineFilled)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        public List<List<Cell>> GetFilledLinesVertical(bool preview)
        {
            var lines = new List<List<Cell>>();
            for (var i = 0; i < cells.GetLength(1); i++)
            {
                var isLineFilled = true;
                var line = new List<Cell>();
                for (var j = 0; j < cells.GetLength(0); j++)
                {
                    if (cells[j, i].IsEmpty(preview))
                    {
                        isLineFilled = false;
                        break;
                    }

                    line.Add(cells[j, i]);
                }

                if (isLineFilled)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        public bool CanPlaceShape(Shape shape)
        {
            if (cells == null)
            {
                return false;
            }

            var activeItems = shape.GetActiveItems();
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            // Find the bounding box of the shape
            foreach (var item in activeItems)
            {
                var pos = item.GetPosition();
                minX = Mathf.Min(minX, pos.x);
                minY = Mathf.Min(minY, pos.y);
                maxX = Mathf.Max(maxX, pos.x);
                maxY = Mathf.Max(maxY, pos.y);
            }

            var shapeWidth = maxX - minX + 1;
            var shapeHeight = maxY - minY + 1;

            // Try to place the shape at every possible position on the field
            for (var fieldY = 0; fieldY <= cells.GetLength(0) - shapeHeight; fieldY++)
            {
                for (var fieldX = 0; fieldX <= cells.GetLength(1) - shapeWidth; fieldX++)
                {
                    if (CanPlaceShapeAt(activeItems, fieldX - minX, fieldY - minY))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Cell GetCenterCell()
        {
            var x = cells.GetLength(1) / 2;
            var y = cells.GetLength(0) / 2;
            return cells[y, x];
        }

        private bool CanPlaceShapeAt(List<Item> items, int offsetX, int offsetY)
        {
            foreach (var item in items)
            {
                var pos = item.GetPosition();
                var x = offsetX + pos.x;
                var y = offsetY + pos.y;

                if (x < 0 || x >= cells.GetLength(1) || y < 0 || y >= cells.GetLength(0))
                {
                    return false; // Out of bounds
                }

                if (!cells[y, x].IsEmpty() && cells[y, x].busy)
                {
                    return false; // Cell is already occupied
                }
            }

            return true;
        }

        public void ShowOutline(bool show)
        {
            var paddingX = 0.033f;
            var paddingY = 0.033f;

            outline.anchoredPosition = field.anchoredPosition;
            outline.sizeDelta = field.sizeDelta;
            outline.anchorMin = new Vector2(field.anchorMin.x - paddingX, field.anchorMin.y - paddingY);
            outline.anchorMax = new Vector2(field.anchorMax.x + paddingX, field.anchorMax.y + paddingY);
            outline.pivot = field.pivot;
            outline.gameObject.SetActive(show);
        }

        public Cell[,] GetAllCells()
        {
            return cells;
        }

        public List<List<Cell>> GetRow(int i)
        {
            var row = new List<List<Cell>>();
            for (var j = 0; j < cells.GetLength(1); j++)
            {
                row.Add(new List<Cell> { cells[i, j] });
            }

            return row;
        }

        public Cell[] GetEmptyCells()
        {
            return cells.Cast<Cell>().Where(cell => !cell.busy).ToArray();
        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        public List<Cell> GetTutorialLine()
        {
            var line = new List<Cell>();
            for (var i = 0; i < cells.GetLength(0); i++)
            {
                for (var j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].IsHighlighted())
                    {
                        line.Add(cells[i, j]);
                    }
                }
            }

            return line;
        }
    }
}