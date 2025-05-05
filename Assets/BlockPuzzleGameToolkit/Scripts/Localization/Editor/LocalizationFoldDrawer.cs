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
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationIndexFolder))]
    public class LocalizationFoldDrawer : PropertyDrawer
    {
        private int offset;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "Text");
            if (property.isExpanded)
            {
                offset = 5;
                position.y += EditorGUIUtility.singleLineHeight;
                ShowField(position, property, "description", "Description");
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                ShowField(position, property, "failed", "Fail Description");
            }

            EditorGUI.EndProperty();
        }

        private void ShowField(Rect position, SerializedProperty property, string field, string label)
        {
            position.x += offset;
            var r1 = position;
            r1.width = 100;
            EditorGUI.LabelField(r1, label);
            position.x += offset;
            var r2 = position;
            r2.xMin = r1.xMax + 10;
            EditorGUI.PropertyField(r2, property.FindPropertyRelative(field));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2 : EditorGUIUtility.singleLineHeight;
        }
    }

    [Serializable]
    public class LocalizationIndexFolder
    {
        [Tooltip("Default text")]
        public LocalizationIndex description;

        public LocalizationIndex failed;
    }
}