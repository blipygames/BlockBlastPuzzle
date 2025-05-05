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

using TMPro;
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization.Editor
{
    public class LocalizedTextMeshProUGUICreator
    {
        [MenuItem("GameObject/UI/Localized TextMeshPro - Text (UI)")]
        private static void CreateLocalizedTextMeshProUGUI()
        {
            // Create a new GameObject
            var go = new GameObject("Localized TextMeshPro");

            // Add the LocalizedTextMeshProUGUI component
            var localizedText = go.AddComponent<LocalizedTextMeshProUGUI>();

            localizedText.fontSize = 32;
            localizedText.enableAutoSizing = true;
            localizedText.fontSizeMin = 16;
            localizedText.fontSizeMax = 200;
            localizedText.alignment = TextAlignmentOptions.Center;

            // Set the parent of the new object
            if (Selection.activeGameObject != null)
            {
                go.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            // Register the creation for undo
            Undo.RegisterCreatedObjectUndo(go, "Create Localized TextMeshPro");

            // Select the newly created object
            Selection.activeGameObject = go;
        }
    }
}