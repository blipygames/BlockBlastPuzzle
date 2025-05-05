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

using System.Reflection;
using TMPro;
using UnityEditor;

namespace BlockPuzzleGameToolkit.Scripts.Localization.Editor
{
    public class TextMeshProReplacer : EditorWindow
    {
        [MenuItem("Tools/Replace TMP with LocalizedTMP")]
        private static void ReplaceTextMeshProUGUI()
        {
            var selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select at least one GameObject.", "OK");
                return;
            }

            var replacedCount = 0;

            foreach (var obj in selectedObjects)
            {
                var tmproComponents = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var tmproComponent in tmproComponents)
                {
                    if (tmproComponent.GetType() == typeof(TextMeshProUGUI)) // Ensure we're not replacing already customized scripts
                    {
                        var localizeText = tmproComponent.GetComponent<LocalizeText>();
                        var instanceID = localizeText != null ? localizeText.instanceID : "";

                        // Store all relevant properties
                        var text = tmproComponent.text;
                        var font = tmproComponent.font;
                        var fontMaterial = tmproComponent.fontMaterial;
                        var color = tmproComponent.color;
                        var fontStyle = tmproComponent.fontStyle;
                        var fontSize = tmproComponent.fontSize;
                        var autoSizeTextContainer = tmproComponent.autoSizeTextContainer;
                        var enableAutoSizing = tmproComponent.enableAutoSizing;
                        var characterSpacing = tmproComponent.characterSpacing;
                        var wordSpacing = tmproComponent.wordSpacing;
                        var lineSpacing = tmproComponent.lineSpacing;
                        var paragraphSpacing = tmproComponent.paragraphSpacing;
                        var alignment = tmproComponent.alignment;
                        var enableWordWrapping = tmproComponent.enableWordWrapping;
                        var overflowMode = tmproComponent.overflowMode;
                        var isRightToLeftText = tmproComponent.isRightToLeftText;
                        var enableKerning = tmproComponent.enableKerning;
                        var extraPadding = tmproComponent.extraPadding;
                        var richText = tmproComponent.richText;

                        // Remove old component
                        DestroyImmediate(tmproComponent);
                        if (localizeText != null)
                        {
                            DestroyImmediate(localizeText);
                        }

                        // Add new component
                        var newComponent = obj.AddComponent<LocalizedTextMeshProUGUI>();

                        // Restore properties
                        newComponent.text = text;
                        newComponent.font = font;
                        newComponent.fontMaterial = fontMaterial;
                        newComponent.color = color;
                        newComponent.fontStyle = fontStyle;
                        newComponent.fontSize = fontSize;
                        newComponent.autoSizeTextContainer = autoSizeTextContainer;
                        newComponent.enableAutoSizing = enableAutoSizing;
                        newComponent.characterSpacing = characterSpacing;
                        newComponent.wordSpacing = wordSpacing;
                        newComponent.lineSpacing = lineSpacing;
                        newComponent.paragraphSpacing = paragraphSpacing;
                        newComponent.alignment = alignment;
                        newComponent.enableWordWrapping = enableWordWrapping;
                        newComponent.overflowMode = overflowMode;
                        newComponent.isRightToLeftText = isRightToLeftText;
                        newComponent.enableKerning = enableKerning;
                        newComponent.extraPadding = extraPadding;
                        newComponent.richText = richText;

                        // Set the instanceID using reflection (since it's private)
                        var fieldInfo = typeof(LocalizedTextMeshProUGUI).GetField("instanceID", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldInfo != null)
                        {
                            fieldInfo.SetValue(newComponent, instanceID);
                        }

                        replacedCount++;
                    }
                }
            }

            EditorUtility.DisplayDialog("Replacement Complete", $"Replaced {replacedCount} TextMeshProUGUI component(s) with LocalizedTextMeshProUGUI.", "OK");
        }
    }
}