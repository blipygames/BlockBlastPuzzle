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

using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization.Editor
{
    [CustomPropertyDrawer(typeof(LocalizationIndex))]
    public class LocalizationDrawerUIE : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            var r1 = position;
            r1.width = 1;

            var r2 = position;
            r2.xMin = r1.xMax + 1;
            r2.width = 50;

            EditorGUI.BeginProperty(position, label, property);
            // EditorGUI.PropertyField(r1, property.FindPropertyRelative("text"),new GUIContent("","Default text"));
            EditorGUI.PropertyField(r2, property.FindPropertyRelative("index"), new GUIContent("", "Localization line index"));
            r2.x += 50;
            r2.width = 300;
            EditorGUI.LabelField(r2, "Change text here: Resources/Localization/");
            EditorGUI.EndProperty();
        }
    }
}