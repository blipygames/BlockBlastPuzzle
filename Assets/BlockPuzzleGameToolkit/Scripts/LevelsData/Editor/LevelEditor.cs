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
using BlockPuzzleGameToolkit.Scripts.Editor.Drawers;
using BlockPuzzleGameToolkit.Scripts.Enums;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData.Editor
{
    [CustomEditor(typeof(Level))]
    public class LevelEditor : UnityEditor.Editor
    {
        private List<ItemTemplate> availableTemplates;
        private Level level;
        private VisualElement root;
        private VisualElement matrixContainer;
        private IntegerField rowsField;
        private IntegerField columnsField;

        private VisualElement targetParameters;
        private string brush;
        private LevelTypeScriptable _previousELevelType;
        private Toggle symmetricalGenerationToggle;
        private PopupField<string> levelTypeDropdown;
        private Button cellGreyWithBonus;
        private readonly Color _highlightColor = new(0.6f, 0.6f, 0.6f);
        private readonly Color _disableColor = new(0.3f, 0.3f, 0.3f);

        private void OnEnable()
        {
            level = (Level)target;
            level.InitializeIfNeeded();
            LoadAvailableTemplates();
            if (level.bonusItemColors == null)
            {
                level.bonusItemColors = new Dictionary<Color, int>();
            }

            _previousELevelType = level.levelType;
        }

        private void LoadAvailableTemplates()
        {
            availableTemplates = new List<ItemTemplate>(Resources.LoadAll<ItemTemplate>(""));
            availableTemplates.Insert(0, null); // Add null as the first option (empty cell)
        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            // Load and apply USS
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BlockPuzzleGameToolkit/UIBuilder/LevelEditorStyles.uss");
            root.styleSheets.Add(styleSheet);

            root.Add(new Label(level.name) { name = "title" });
            root.Add(new LevelSwitcher(serializedObject, level, this));
            root.Add(new Label(""));

            var dimensionContainer = new VisualElement { name = "dimension-container" };
            rowsField = new IntegerField("Rows") { value = level.rows };
            columnsField = new IntegerField("Columns") { value = level.columns };
            var resizeButton = CreateButton("Resize", Color.white, "", false, ResizeMatrix);
            rowsField.RegisterValueChangedCallback(evt => level.rows = evt.newValue);
            columnsField.RegisterValueChangedCallback(evt => level.columns = evt.newValue);

            dimensionContainer.Add(rowsField);
            dimensionContainer.Add(columnsField);
            dimensionContainer.Add(resizeButton);
            root.Add(new Label(""));

            root.Add(dimensionContainer);
            root.Add(new Label(""));

            // Add Timer Settings Section
            var timerContainer = new VisualElement { name = "timer-container" };
            timerContainer.style.marginTop = 10;
            timerContainer.style.marginBottom = 10;
            timerContainer.style.paddingTop = 10;
            timerContainer.style.paddingBottom = 10;
            timerContainer.style.borderTopWidth = 1;
            timerContainer.style.borderBottomWidth = 1;
            timerContainer.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f);
            timerContainer.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);

            var timerLabel = new Label("Timer Settings");
            timerLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            timerLabel.style.marginBottom = 5;
            timerContainer.Add(timerLabel);


            var timerDurationField = new FloatField("Duration (seconds)") { value = level.timerDuration };
            timerDurationField.SetEnabled(level.enableTimer);
            timerDurationField.RegisterValueChangedCallback(evt =>
            {
                level.timerDuration = (int)evt.newValue;
                EditorUtility.SetDirty(target);
            });
            var enableTimerToggle = new Toggle("Enable Timer") { value = level.enableTimer };
            enableTimerToggle.RegisterValueChangedCallback(evt => 
            {
                level.enableTimer = evt.newValue;
                timerDurationField.SetEnabled(evt.newValue);
                EditorUtility.SetDirty(target);
            });
            timerContainer.Add(enableTimerToggle);
            timerContainer.Add(timerDurationField);

            root.Add(timerContainer);
            root.Add(new Label(""));

            var levelTypes = Resources.LoadAll<LevelTypeScriptable>("").Where(i => i.selectable);
            var levelTypeNames = new List<string>();
            foreach (var levelType in levelTypes)
            {
                levelTypeNames.Add(levelType.name);
            }

            if (level.levelType.elevelType != ELevelType.Classic)
            {
                // Register callback for level type change
                levelTypeDropdown = new PopupField<string>("Level Type", levelTypeNames, level.levelType.name);
                levelTypeDropdown.RegisterValueChangedCallback(evt =>
                {
                    var selectedLevelType = levelTypes.FirstOrDefault(lt => lt.name == evt.newValue);
                    if (selectedLevelType != null)
                    {
                        level.levelType = selectedLevelType;
                        OnLevelTypeChanged();
                    }
                });
                root.Add(levelTypeDropdown);
            }

            root.Add(new Label(""));

            var targetField = new PropertyField(serializedObject.FindProperty("target"));
            root.Add(targetField);

            // Create bonus item color container
            targetParameters = new VisualElement { name = "bonus-item-color-container" };
            root.Add(targetParameters);
            root.Add(new Label(""));

            ToolPanel();
            root.Add(new Label("Target Parameters"));
            CreateBonusItemColorUI();
            root.Add(new Label("Click on cells to cycle through available ItemTemplates"));

            matrixContainer = new VisualElement { name = "grid-container" };
            root.Add(matrixContainer);

            // Add slider for empty cell percentage
            var emptyCellSlider = new Slider("Empty Cell %", 0, 100) { value = level.emptyCellPercentage };
            emptyCellSlider.style.width = 300;
            emptyCellSlider.RegisterValueChangedCallback(evt =>
            {
                level.emptyCellPercentage = evt.newValue;
                Randomize();
            });
            root.Add(emptyCellSlider);
            var randomButton = new Button(Randomize) { text = "Randomize" };
            randomButton.style.width = 150;
            randomButton.style.backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f));
            randomButton.RegisterCallback<ClickEvent>(evt => Randomize());
            root.Add(randomButton);

            // Add checkbox for symmetrical generation
            symmetricalGenerationToggle = new Toggle("Symmetrical Generation");
            symmetricalGenerationToggle.value = true;
            root.Add(symmetricalGenerationToggle);

            UpdateMatrixUI();

            return root;
        }

        private void OnLevelTypeChanged()
        {
            if (level.levelType != _previousELevelType)
            {
                level.UpdateTargets();

                CreateBonusItemColorUI();
                Save();
                _previousELevelType = level.levelType;
                UpdateToolPanel();
            }
        }

        private void UpdateToolPanel()
        {
            var isBonusItemLevelType = level.levelType.elevelType == ELevelType.CollectItems;
            cellGreyWithBonus.style.display = isBonusItemLevelType ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void CreateBonusItemColorUI()
        {
            targetParameters.Clear();
            targetParameters.style.flexDirection = FlexDirection.Row;

            for (var index = 0; index < level.targetInstance.Count; index++)
            {
                var targetInstance = level.targetInstance[index];

                if (targetInstance.targetScriptable.bonusItem == null)
                {
                    var label = new Label("Score");
                    targetParameters.Add(label);
                }

                var amountField = new IntegerField();
                amountField.style.width = 50;
                amountField.value = targetInstance.amount;
                amountField.RegisterValueChangedCallback(evt =>
                {
                    targetInstance.amount = evt.newValue;
                    EditorUtility.SetDirty(target);
                });

                var container = new VisualElement { style = { flexDirection = FlexDirection.Column } };

                if (targetInstance.targetScriptable != null && targetInstance.targetScriptable.bonusItem != null)
                {
                    var bonusItemSerializedObject = new SerializedObject(targetInstance.targetScriptable.bonusItem);
                    var prefabProperty = bonusItemSerializedObject.FindProperty("prefab");

                    var iconDrawer = new IconDrawer();
                    var iconField = iconDrawer.CreatePropertyGUI(prefabProperty);
                    iconField.style.width = 25;
                    iconField.style.height = 25;
                    iconField.style.marginLeft = 25;

                    container.Add(iconField);
                }

                amountField.style.marginLeft = 25;
                container.Add(amountField);
                targetParameters.Add(container);
            }
        }

        private void ToolPanel()
        {
            var actionContainer = new VisualElement { name = "action-container" };
            actionContainer.Add(CreateButton("Clear All", Color.white, "", false, ClearAll));

            var cellGrey = CreateButton("", Color.white, "tool-cell", true, () => SwitchBrush("Grey"));
            cellGrey.style.marginLeft = 50;
            actionContainer.Add(cellGrey);
            cellGreyWithBonus = CreateButton("O", Color.black, "tool-cell", true, () => SwitchBrush("GreyWithBonus"));
            actionContainer.Add(cellGreyWithBonus);
            actionContainer.Add(CreateButton("X", Color.black, "tool-cell", true, DeleteItem));
            root.Add(actionContainer);
            UpdateToolPanel();
        }

        private void SwitchBrush(string grey)
        {
            brush = brush != grey ? grey : "";
        }

        private void Randomize()
        {
            var random = new Random();
            ItemTemplate randomTemplate = null;

            if (level.levelType.singleColorMode)
            {
                randomTemplate = availableTemplates[random.Next(1, availableTemplates.Count)];
            }

            if (level.levelType.elevelType == ELevelType.CollectItems)
            {
                RandomizeCollectItemsLevel(random);
            }
            else
            {
                RandomizeOtherLevelTypes(random);
            }

            if (!symmetricalGenerationToggle.value)
            {
                RandomizeMatrix(random, level.levelType.singleColorMode, randomTemplate);
            }
            else
            {
                RandomizeSymmetricalMatrix(random, level.levelType.singleColorMode, randomTemplate);
            }

            EnsureNoFullRowsOrColumns(random);

            // Check if the matrix is empty and regenerate if necessary
            if (IsMatrixEmpty())
            {
                Randomize();
            }

            UpdateMatrixUI();

            Save();
        }

        private bool IsMatrixEmpty()
        {
            for (var i = 0; i < level.rows; i++)
            {
                for (var j = 0; j < level.columns; j++)
                {
                    if (level.GetItem(i, j) != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void RandomizeCollectItemsLevel(Random random)
        {
            var targetCount = random.Next(1, 6);
            var selectedIndexes = new HashSet<int>();

            for (var i = 0; i < level.targetInstance.Count; i++)
            {
                level.targetInstance[i].amount = 0;
            }

            while (selectedIndexes.Count < targetCount)
            {
                var index = random.Next(0, level.targetInstance.Count);
                selectedIndexes.Add(index);
            }

            var totalAmount = 0;
            foreach (var index in selectedIndexes)
            {
                int[] possibleAmounts = { 5, 10, 15 };
                var amount = possibleAmounts[random.Next(0, possibleAmounts.Length)];

                if (totalAmount + amount > 15)
                {
                    amount = 15 - totalAmount;
                }

                level.targetInstance[index].amount = amount;
                totalAmount += amount;

                if (totalAmount >= 15)
                {
                    break;
                }
            }

            CreateBonusItemColorUI();
        }

        private void RandomizeOtherLevelTypes(Random random)
        {
            foreach (var t in level.targetInstance)
            {
                var randomValue = random.Next(50, 500);
                var roundedTo50 = (int)Math.Round(randomValue / 50.0) * 50;
                t.amount = roundedTo50;
            }

            CreateBonusItemColorUI();
        }

        private void RandomizeMatrix(Random random, bool singleColorMode, ItemTemplate randomTemplate)
        {
            for (var i = 0; i < level.rows; i++)
            {
                for (var j = 0; j < level.columns; j++)
                {
                    SetRandomItem(random, singleColorMode, randomTemplate, i, j);
                }
            }
        }

        private void RandomizeSymmetricalMatrix(Random random, bool gameSettings, ItemTemplate randomTemplate)
        {
            for (var i = 0; i < level.rows / 2; i++)
            {
                for (var j = 0; j < level.columns / 2; j++)
                {
                    SetRandomItem(random, gameSettings, randomTemplate, i, j);
                }
            }

            MirrorMatrix();
        }

        private void SetRandomItem(Random random, bool singleColorMode, ItemTemplate randomTemplate, int i, int j)
        {
            if (random.Next(0, 100) > level.emptyCellPercentage)
            {
                level.SetItem(i, j, null);
                level.SetBonus(i, j, false);
            }
            else
            {
                var template = singleColorMode ? randomTemplate : availableTemplates[random.Next(1, availableTemplates.Count)];
                var bonus = random.Next(0, 2) == 0;

                if (bonus && level.levelType.targets[0].bonusItem != null)
                {
                    level.SetItem(i, j, availableTemplates[1]);
                    level.SetBonus(i, j, true);
                }
                else
                {
                    level.SetItem(i, j, template);
                    level.SetBonus(i, j, false);
                }
            }
        }

        private void MirrorMatrix()
        {
            for (var i = 0; i < level.rows / 2; i++)
            {
                for (var j = 0; j < level.columns / 2; j++)
                {
                    level.SetItem(i, level.columns - j - 1, level.GetItem(i, j));
                    level.SetBonus(i, level.columns - j - 1, level.GetBonus(i, j));

                    level.SetItem(level.rows - i - 1, j, level.GetItem(i, j));
                    level.SetBonus(level.rows - i - 1, j, level.GetBonus(i, j));

                    level.SetItem(level.rows - i - 1, level.columns - j - 1, level.GetItem(i, j));
                    level.SetBonus(level.rows - i - 1, level.columns - j - 1, level.GetBonus(i, j));
                }
            }
        }

        private void EnsureNoFullRowsOrColumns(Random random)
        {
            for (var i = 0; i < level.rows; i++)
            {
                if (IsRowFull(i))
                {
                    var randomColumn = random.Next(0, level.columns);
                    level.SetItem(i, randomColumn, null);
                    level.SetBonus(i, randomColumn, false);
                }
            }

            for (var i = 0; i < level.columns; i++)
            {
                if (IsColumnFull(i))
                {
                    var randomRow = random.Next(0, level.rows);
                    level.SetItem(randomRow, i, null);
                    level.SetBonus(randomRow, i, false);
                }
            }
        }

        private bool IsRowFull(int row)
        {
            for (var j = 0; j < level.columns; j++)
            {
                if (level.GetItem(row, j) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsColumnFull(int column)
        {
            for (var j = 0; j < level.rows; j++)
            {
                if (level.GetItem(j, column) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void DeleteItem()
        {
            brush = brush == "X" ? "" : "X";
        }

        private void ResizeMatrix()
        {
            var newRows = Mathf.Max(1, rowsField.value);
            var newColumns = Mathf.Max(1, columnsField.value);
            level.Resize(newRows, newColumns);
            UpdateMatrixUI();
            Save();
        }

        private void UpdateMatrixUI()
        {
            matrixContainer.Clear();

            for (var i = 0; i < level.rows; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("grid-row");
                matrixContainer.Add(row);

                for (var j = 0; j < level.columns; j++)
                {
                    var cell = new Button();
                    cell.AddToClassList("grid-cell");
                    int x = i, y = j; // Capture loop variables
                    var item = level.GetItem(x, y);
                    var color = item != null ? (Color?)item.overlayColor : null;
                    UpdateCellColor(cell, level.GetBonus(x, y), color);

                    if (level.IsDisabled(x, y))
                    {
                        cell.style.backgroundColor = new StyleColor(_disableColor);
                    }
                    else if (level.IsCellHighlighted(x, y))
                    {
                        cell.style.backgroundColor = new StyleColor(_highlightColor);
                    }

                    cell.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        if (evt.button == 1) // Right-click
                        {
                            ShowContextMenu(x, y, cell);
                            evt.StopPropagation();
                        }
                    });

                    cell.clicked += () =>

                    {
                        if (brush == "X")
                        {
                            level.SetItem(x, y, availableTemplates[0]);
                            level.SetBonus(x, y, false);
                        }
                        else if (brush == "Grey")
                        {
                            level.SetItem(x, y, availableTemplates[1]);
                            level.SetBonus(x, y, false);
                        }
                        else if (brush == "GreyWithBonus")
                        {
                            level.SetItem(x, y, availableTemplates[1]);
                            level.SetBonus(x, y, true);
                        }
                        else
                        {
                            CycleItemTemplate(x, y);
                        }

                        var newItem = level.GetItem(x, y);
                        var newColor = newItem != null ? (Color?)newItem.overlayColor : null;
                        UpdateCellColor(cell, level.GetBonus(x, y), newColor);
                        Save();
                    };

                    row.Add(cell);
                }
            }

            // Update the IntegerFields to reflect the current level dimensions
            rowsField.SetValueWithoutNotify(level.rows);
            columnsField.SetValueWithoutNotify(level.columns);
        }

        private void ShowContextMenu(int x, int y, Button cell)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Highlight"), level.IsCellHighlighted(x, y), () => HighlightCell(x, y, cell));
            menu.AddItem(new GUIContent("Disable"), level.IsDisabled(x, y), () => DisableCell(x, y, cell));
            menu.ShowAsContext();
        }

        private void HighlightCell(int x, int y, Button cell)
        {
            level.HighlightCellToggle(x, y);
            UpdateCellColor(cell, level.GetBonus(x, y), level.IsCellHighlighted(x, y) ? _highlightColor : null);
            Save();
        }

        private void DisableCell(int x, int y, Button cell)
        {
            level.DisableCellToggle(x, y);
            UpdateCellColor(cell, level.GetBonus(x, y), level.IsDisabled(x, y) ? _disableColor : null);
            Save();
        }

        private void CycleItemTemplate(int x, int y)
        {
            var currentIndex = availableTemplates.IndexOf(level.GetItem(x, y));
            var nextIndex = currentIndex == 0 ? 1 : (currentIndex + 1) % availableTemplates.Count;
            if (nextIndex == 0)
            {
                nextIndex = 1; // Ensure we skip the 0th element
            }

            level.SetItem(x, y, availableTemplates[nextIndex]);
        }

        private void UpdateCellColor(Button cell, bool bonus, Color? templateOverlayColor)
        {
            if (templateOverlayColor != null)
            {
                cell.style.backgroundColor = templateOverlayColor.Value;
                if (bonus)
                {
                    cell.Clear();
                    cell.Add(new Label("O") { style = { color = Color.black } });
                    cell.style.justifyContent = Justify.Center;
                }
                else
                {
                    cell.Clear();
                }
            }
            else
            {
                cell.style.backgroundColor = StyleKeyword.Null;
                cell.Clear();
            }
        }

        private Button CreateButton(string text, StyleColor colorLabel, string styleClass, bool pressedState, Action clickEvent)
        {
            var button = new Button(clickEvent);
            // label
            var label = new Label(text);
            label.style.color = colorLabel;
            button.style.flexGrow = 1;
            button.style.justifyContent = Justify.Center;
            button.Add(label);

            if (!string.IsNullOrEmpty(styleClass))
            {
                button.AddToClassList(styleClass);
            }

            if (pressedState)
            {
                button.RegisterCallback<ClickEvent>(_ => ToggleActiveState());
            }

            void ToggleActiveState()
            {
                if (button.ClassListContains("pressed"))
                {
                    button.RemoveFromClassList("pressed");
                }
                else
                {
                    //remove all other active buttons
                    foreach (var child in button.parent.Children())
                    {
                        child.RemoveFromClassList("pressed");
                    }

                    button.AddToClassList("pressed");
                }
            }

            return button;
        }

        private void ClearAll()
        {
            for (var i = 0; i < level.rows; i++)
            {
                for (var j = 0; j < level.columns; j++)
                {
                    level.SetItem(i, j, null);
                    level.levelRows[i].disabled[j] = false;
                    level.levelRows[i].highlighted[j] = false;
                }
            }

            UpdateMatrixUI();
            Save();
        }

        public void Save()
        {
            EditorUtility.SetDirty(target);
            // AssetDatabase.SaveAssetIfDirty(target);
        }
    }
}