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
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData
{
    [Serializable]
    public class LevelRow
    {
        public ItemTemplate[] cells;
        public bool[] bonusItems; // bonus item with diamonds
        public bool[] disabled; // disabled collider
        public bool[] highlighted; // highlighted cell for tutorial

        public LevelRow(int size)
        {
            cells = new ItemTemplate[size];
            bonusItems = new bool[size];
            disabled = new bool[size];
            highlighted = new bool[size];
        }
    }

    [CreateAssetMenu(fileName = "Level", menuName = "BlockPuzzleGameToolkit/Levels/Level", order = 1)]
    public class Level : ScriptableObject
    {
        public int rows = 8;
        public int columns = 8;
        public LevelRow[] levelRows;
        public LevelTypeScriptable levelType;
        public bool enableTimer = false;
        public int timerDuration = 120;

        [SerializeField]
        public Dictionary<Color, int> bonusItemColors;

        [SerializeField]
        public List<Target> targetInstance = new();

        public float emptyCellPercentage = 10f;

        public int Number => GetLevelNum();


        private void OnEnable()
        {
            InitializeIfNeeded();
        }

        public void InitializeIfNeeded()
        {
            if (levelRows == null || levelRows.Length != rows)
            {
                levelRows = new LevelRow[rows];
                for (var i = 0; i < rows; i++)
                {
                    levelRows[i] = new LevelRow(columns);
                }
            }
        }

        public ItemTemplate GetItem(int row, int column)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                return levelRows[row].cells[column];
            }

            return null;
        }

        public void SetBonus(int row, int column, bool bonus)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                levelRows[row].bonusItems[column] = bonus;
            }
        }

        public void Resize(int newRows, int newColumns)
        {
            var newLevelRows = new LevelRow[newRows];
            for (var i = 0; i < newRows; i++)
            {
                newLevelRows[i] = new LevelRow(newColumns);
            }

            rows = newRows;
            columns = newColumns;
            levelRows = newLevelRows;
        }

        private int GetLevelNum()
        {
            var levelName = name;
            var numericPart = new string(levelName.Where(char.IsDigit).ToArray());

            if (int.TryParse(numericPart, out var levelNum))
            {
                return levelNum;
            }

            Debug.LogWarning("Unable to parse the numeric part from the level name.");
            return -1;
        }

        public bool GetBonus(int row, int col)
        {
            if (row >= 0 && row < rows && col >= 0 && col < columns)
            {
                return levelRows[row].bonusItems[col];
            }

            return false;
        }

        public void UpdateTargets()
        {
            targetInstance.Clear();
            foreach (var targetScriptable in levelType.targets)
            {
                targetInstance.Add(new Target(targetScriptable));
            }
        }

        public void SetItem(int row, int column, ItemTemplate item)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                levelRows[row].cells[column] = item;
            }

            if (item == null)
            {
                SetBonus(row, column, false);
            }
        }

        public bool IsDisabled(int row, int column)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                if (levelRows[row].disabled == null || column >= levelRows[row].disabled.Length)
                {
                    return false;
                }

                return levelRows[row].disabled[column];
            }

            return false;
        }

        public void DisableCellToggle(int row, int column)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                if (levelRows[row].disabled == null || column >= levelRows[row].disabled.Length)
                {
                    levelRows[row].disabled = new bool[columns];
                }

                levelRows[row].disabled[column] = !levelRows[row].disabled[column];
            }
        }

        //for tutorial
        public void HighlightCellToggle(int row, int column)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                if (levelRows[row].highlighted == null || column >= levelRows[row].highlighted.Length)
                {
                    levelRows[row].highlighted = new bool[columns];
                }

                levelRows[row].highlighted[column] = !levelRows[row].highlighted[column];
            }
        }

        public bool IsCellHighlighted(int row, int column)
        {
            if (row >= 0 && row < rows && column >= 0 && column < columns)
            {
                if (levelRows[row].highlighted == null || column >= levelRows[row].highlighted.Length)
                {
                    return false;
                }

                return levelRows[row].highlighted[column];
            }

            return false;
        }
    }
}