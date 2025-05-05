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
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var foldout = new Foldout();
            foldout.text = property.displayName;
            container.Add(foldout);

            var listView = new ListView();
            foldout.Add(listView);
            var dictionaryType = fieldInfo.FieldType;
            var keyType = dictionaryType.GetGenericArguments()[0];
            listView.makeItem = () => new PackElementDictionaryItemElement(keyType);
            listView.bindItem = (element, index) =>
            {
                var itemElement = (PackElementDictionaryItemElement)element;
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");
                itemElement.BindProperties(keysProp.GetArrayElementAtIndex(index), valuesProp.GetArrayElementAtIndex(index), property, index);
            };

            listView.showAddRemoveFooter = true;
            listView.showFoldoutHeader = false;
            listView.showBorder = true;
            listView.showBoundCollectionSize = false;
            listView.reorderable = true;

            void RefreshList()
            {
                var keysProp = property.FindPropertyRelative("keys");
                listView.itemsSource = new List<int>(Enumerable.Range(0, keysProp.arraySize));
                listView.RefreshItems();
            }

            RefreshList();

            listView.itemsAdded += indexes =>
            {
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");

                property.serializedObject.Update();
                foreach (var index in indexes)
                {
                    keysProp.InsertArrayElementAtIndex(index);
                    valuesProp.InsertArrayElementAtIndex(index);

                    var keyProp = keysProp.GetArrayElementAtIndex(index);
                    keyProp.objectReferenceValue = null;

                    var valueProp = valuesProp.GetArrayElementAtIndex(index);
                    // Reset the value based on its type
                    ResetValue(valueProp);
                }

                property.serializedObject.ApplyModifiedProperties();

                RefreshList();
            };

            listView.itemsRemoved += indexes =>
            {
                var keysProp = property.FindPropertyRelative("keys");
                var valuesProp = property.FindPropertyRelative("values");

                property.serializedObject.Update();
                foreach (var index in indexes.OrderByDescending(i => i))
                {
                    keysProp.DeleteArrayElementAtIndex(index);
                    valuesProp.DeleteArrayElementAtIndex(index);
                }

                property.serializedObject.ApplyModifiedProperties();

                RefreshList();
            };

            return container;
        }

        private void ResetValue(SerializedProperty valueProp)
        {
            switch (valueProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    valueProp.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    valueProp.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    valueProp.floatValue = 0f;
                    break;
                case SerializedPropertyType.String:
                    valueProp.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.ObjectReference:
                    valueProp.objectReferenceValue = null;
                    break;
                // Add more cases as needed for other types
            }
        }
    }

    public class PackElementDictionaryItemElement : VisualElement
    {
        private readonly ObjectField keyField;
        private readonly PropertyField valueField;

        public PackElementDictionaryItemElement(Type keyType)
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;
            style.width = Length.Percent(100);

            keyField = new ObjectField
            {
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0,
                    marginRight = 5,
                    marginLeft = 5
                }
            };
            keyField.objectType = keyType;
            Add(keyField);

            valueField = new PropertyField
            {
                style =
                {
                    flexGrow = 1,
                    flexBasis = 0,
                    marginRight = 5
                }
            };
            Add(valueField);
        }

        public void BindProperties(SerializedProperty keyProp, SerializedProperty valueProp, SerializedProperty parentProp, int index)
        {
            keyField.BindProperty(keyProp);
            valueField.BindProperty(valueProp);

            keyField.label = string.Empty;
            valueField.label = string.Empty;
        }
    }
}