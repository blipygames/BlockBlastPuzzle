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

using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization.Editor
{
    [CustomEditor(typeof(LocalizedTextMeshProUGUI), true)]
    [CanEditMultipleObjects]
    public class LocalizedTextMeshProUGUIEditor : TMP_EditorPanelUI
    {
        private SerializedProperty instanceIDProp;
        private LocalizedTextMeshProUGUI localizedText;
        private string lastKnownName;

        protected override void OnEnable()
        {
            base.OnEnable();
            instanceIDProp = serializedObject.FindProperty("instanceID");
            localizedText = (LocalizedTextMeshProUGUI)target;
            lastKnownName = localizedText.gameObject.name;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (localizedText != null && localizedText.gameObject.name != lastKnownName)
            {
                localizedText.text = localizedText.gameObject.name;
                lastKnownName = localizedText.gameObject.name;
                EditorUtility.SetDirty(localizedText);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(instanceIDProp);
            if (EditorGUI.EndChangeCheck() || Event.current.type == EventType.Layout)
            {
                serializedObject.ApplyModifiedProperties();
                UpdateLocalizedText();
            }

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            if (serializedObject.ApplyModifiedProperties())
            {
                UpdateLocalizedText();
            }
        }

        private void UpdateLocalizedText()
        {
            if (localizedText != null && !string.IsNullOrEmpty(localizedText.instanceID))
            {
                var originalText = localizedText.text;
                var localizedString = LocalizationManager.GetText(localizedText.instanceID, originalText);
                if (localizedText.text != localizedString)
                {
                    localizedText.text = localizedString;

                    if (PrefabUtility.IsPartOfPrefabInstance(localizedText))
                    {
                        PrefabUtility.RecordPrefabInstancePropertyModifications(localizedText);
                    }

                    EditorUtility.SetDirty(localizedText);
                }
            }
        }
    }
}