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
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Utils.ImageCreator.Editor
{
    [InitializeOnLoad]
    public class ImageCreator : UnityEditor.Editor
    {
        static ImageCreator()
        {
            EditorApplication.hierarchyChanged += OnChanged;
        }

        private static void OnChanged()
        {
            if (Application.isPlaying)
            {
                return;
            }

            var obj = Selection.activeGameObject;
            if (obj == null || obj.transform.parent == null)
            {
                return;
            }

            if ((obj.transform.parent.GetComponent<CanvasRenderer>() != null || obj.transform.parent.GetComponent<Canvas>() != null || obj.transform.parent.GetComponent<RectTransform>() != null) &&
                obj.GetComponent<SpriteRenderer>() != null)
            {
                Undo.RegisterCompleteObjectUndo(obj, "Convert SpriteRenderer to Image");

                var spriteRenderer = obj.GetComponent<SpriteRenderer>();
                var sprite = spriteRenderer.sprite;
                var color = spriteRenderer.color;

                // Use Undo.AddComponent instead of obj.AddComponent
                var rectTransform = Undo.AddComponent<RectTransform>(obj);
                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.localScale = Vector3.one;

                var image = Undo.AddComponent<Image>(obj);
                image.sprite = sprite;
                image.color = color;
                image.SetNativeSize();

                // Use Undo.DestroyObjectImmediate instead of Object.DestroyImmediate
                Undo.DestroyObjectImmediate(spriteRenderer);
            }
        }
    }
}