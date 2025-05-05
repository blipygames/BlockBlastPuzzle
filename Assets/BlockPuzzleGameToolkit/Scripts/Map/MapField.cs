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
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.Localization;
using BlockPuzzleGameToolkit.Scripts.Popups;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Map
{
    public class MapField : MonoBehaviour
    {
        public HorizontalLayoutGroup rowPrefab;
        public Cell cellPrefab;
        private int lastRow = 6;
        private int maxLevelsInRow = 8;
        private int maxRows = 8;
        private int top;

        public CustomButton backButton;

        public ItemTemplate[] mapItemTemplate;

        public CustomButton levelButton;

        private List<Cell> cells = new();

        private void OnEnable()
        {
            maxLevelsInRow = GameManager.instance.GameSettings.maxLevelsInRow;
            maxRows = GameManager.instance.GameSettings.maxRows;
            // get levels count
            var length = Resources.LoadAll<Level>("Levels").Length;
            StartCoroutine(SetLevels(length));
            levelButton.onClick.AddListener(MapStart);
            backButton.onClick.AddListener(() => SceneLoader.instance.GoMain());
        }

        private static void MapStart()
        {
            GameDataManager.SetLevel(Resources.Load<Level>("Levels/Level_" + GameDataManager.GetLevelNum()));
            GameManager.instance.OpenGame();
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private IEnumerator SetLevels(int levels)
        {
            cells = new List<Cell>(levels);
            var currentLevel = GameDataManager.GetLevelNum();
            var segmentSize = maxRows * maxLevelsInRow;
            var segmentStartLevel = (currentLevel - 1) / segmentSize * segmentSize + 1;
            var segmentEndLevel = Mathf.Min(segmentStartLevel + segmentSize - 1, levels);
            var rows = Mathf.Min(Mathf.CeilToInt((float)(segmentEndLevel - segmentStartLevel + 1) / maxLevelsInRow), maxRows);
            lastRow = rows - 1;
            var levelsCreated = 0;

            for (var i = 0; i < rows; i++)
            {
                var row = Instantiate(rowPrefab, transform);
                var bottom = i == 0 ? 20 : 0;
                if (top != 0)
                {
                    bottom = 20;
                }

                top = i < lastRow ? 0 : 20;
                top = i == lastRow - 1 && (segmentEndLevel - segmentStartLevel + 1) % maxLevelsInRow != 0 ? 20 : top;

                row.padding = new RectOffset(20, 20, top, bottom);

                for (var j = 0; j < maxLevelsInRow && levelsCreated < segmentEndLevel - segmentStartLevel + 1; j++)
                {
                    var cell = Instantiate(cellPrefab, row.transform);
                    cell.ClearCell();
                    cells.Insert(i * maxLevelsInRow + j, cell);

                    // Name the cell by its level number
                    var levelNum = segmentStartLevel + levelsCreated;
                    cell.name = "Level_" + levelNum;

                    levelsCreated++;
                }
            }

            yield return new WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

            for (var i = 0; i < segmentEndLevel - segmentStartLevel + 1; i++)
            {
                var cell = cells[i];
                var levelNum = segmentStartLevel + i;
                var level = Resources.Load<Level>("Levels/Level_" + levelNum);
                if (level != null && levelNum <= currentLevel)
                {
                    if (level.levelType.elevelType == ELevelType.CollectItems)
                    {
                        cell.FillCell(mapItemTemplate[0]);
                    }
                    else if (level.levelType.elevelType != ELevelType.CollectItems)
                    {
                        cell.FillCell(mapItemTemplate[1]);
                    }
                }

                if (levelNum == currentLevel)
                {
                    cell.FillCell(mapItemTemplate[2]);
                    cell.item.transform.Find("Background/star").gameObject.SetActive(true);
                }
            }

            PlayerPrefs.SetString("stage", GetCurrentSegment(currentLevel).ToString());
            var localizedTextMeshProUguis = FindObjectsByType<LocalizedTextMeshProUGUI>(FindObjectsSortMode.None);
            foreach (var localizedTextMeshProUgui in localizedTextMeshProUguis)
            {
                localizedTextMeshProUgui.UpdateText();
            }
        }

        private int GetCurrentSegment(int currentLevel)
        {
            var segmentSize = maxRows * maxLevelsInRow;
            var segmentNumber = (currentLevel - 1) / segmentSize + 1;
            return segmentNumber;
        }
    }
}