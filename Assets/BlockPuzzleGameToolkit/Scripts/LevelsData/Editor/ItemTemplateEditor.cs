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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData.Editor
{
    [CustomEditor(typeof(ItemTemplate))]
    public class ItemTemplateEditor : UnityEditor.Editor
    {
        private ItemTemplate[] itemTemplates;
        private int currentIndex;
        private ItemTemplate _target;

        private void OnEnable()
        {
            _target = (ItemTemplate)target;
            LoadAvailableTemplates();
            FindCurrentIndex();
        }

        private void LoadAvailableTemplates()
        {
            itemTemplates = Resources.LoadAll<ItemTemplate>("");
            Array.Sort(itemTemplates, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        }

        private void FindCurrentIndex()
        {
            for (var i = 0; i < itemTemplates.Length; i++)
            {
                if (itemTemplates[i] == _target)
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // Create navigation container with arrows
            var navContainer = new VisualElement();
            navContainer.style.flexDirection = FlexDirection.Row;
            navContainer.style.marginBottom = 10;
            navContainer.style.justifyContent = Justify.Center;

            var prevButton = new Button(() => NavigateTemplates(-1)) { text = "<<" };
            prevButton.style.width = 40;
            
            var label = new Label($"{currentIndex + 1}/{itemTemplates.Length}");
            label.style.paddingLeft = 10;
            label.style.paddingRight = 10;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            
            var nextButton = new Button(() => NavigateTemplates(1)) { text = ">>" };
            nextButton.style.width = 40;
            
            var addButton = new Button(AddTemplate) { text = "+" };
            addButton.style.width = 30;
            
            var removeButton = new Button(RemoveTemplate) { text = "-" };
            removeButton.style.width = 30;

            navContainer.Add(prevButton);
            navContainer.Add(label);
            navContainer.Add(nextButton);
            navContainer.Add(addButton);
            navContainer.Add(removeButton);

            root.Add(navContainer);

            // Add the prefab field that will use the IconDrawer
            var prefabProperty = serializedObject.FindProperty("prefab");
            if (prefabProperty != null)
            {
                var iconDrawer = new BlockPuzzleGameToolkit.Scripts.Editor.Drawers.IconDrawer();
                var prefabField = iconDrawer.CreatePropertyGUI(prefabProperty);
                root.Add(prefabField);
            }

            // Add the remaining properties
            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                // Skip prefab property as we've already added it
                if (iterator.propertyPath == "prefab")
                    continue;
                    
                var propertyField = new PropertyField(iterator.Copy());
                propertyField.Bind(serializedObject);
                root.Add(propertyField);
            }

            return root;
        }

        private void NavigateTemplates(int direction)
        {
            if (itemTemplates.Length <= 1)
                return;

            currentIndex += direction;
            
            // Wrap around
            if (currentIndex < 0)
                currentIndex = itemTemplates.Length - 1;
            else if (currentIndex >= itemTemplates.Length)
                currentIndex = 0;

            // Select the new template
            Selection.activeObject = itemTemplates[currentIndex];
        }

        private void AddTemplate()
        {
            // Find a unique name for the new template
            string newName = "ItemTemplate New";
            int counter = 0;
            while (AssetDatabase.FindAssets($"t:ItemTemplate {newName}{(counter == 0 ? "" : " " + counter)}").Length > 0)
            {
                counter++;
            }
            newName = $"{newName}{(counter == 0 ? "" : " " + counter)}";

            // Create new template by duplicating the current one
            string path = AssetDatabase.GetAssetPath(_target);
            string directory = Path.GetDirectoryName(path);
            string newPath = $"{directory}/{newName}.asset";

            AssetDatabase.CopyAsset(path, newPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Select the new template
            var newTemplate = AssetDatabase.LoadAssetAtPath<ItemTemplate>(newPath);
            Selection.activeObject = newTemplate;
        }

        private void RemoveTemplate()
        {
            if (itemTemplates.Length <= 1)
            {
                EditorUtility.DisplayDialog("Cannot Remove", "Cannot remove the last item template.", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Remove Template", 
                $"Are you sure you want to remove '{_target.name}'?", "Remove", "Cancel"))
            {
                string path = AssetDatabase.GetAssetPath(_target);
                int newIndex = currentIndex > 0 ? currentIndex - 1 : 0;
                
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                // Reload available templates and select the new one
                LoadAvailableTemplates();
                Selection.activeObject = itemTemplates[newIndex];
            }
        }
    }
} 