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

using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI
{
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectCanvasTransform;

        private Rect lastSafeArea = Rect.zero;
        private Vector2 lastScreenSize = Vector2.zero;

        private void Awake()
        {
            ApplySafeArea();
        }

        private void OnRectTransformDimensionsChange()
        {
            ApplySafeArea();
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            ApplySafeArea();
        }
        #endif

        private void ApplySafeArea()
        {
            if (rectCanvasTransform == null)
            {
                return;
            }

            var safeArea = Screen.safeArea;
            var screenSize = new Vector2(Screen.width, Screen.height);
            if (safeArea != lastSafeArea || screenSize != lastScreenSize)
            {
                lastSafeArea = safeArea;
                lastScreenSize = screenSize;

                var anchorMin = safeArea.position;
                var anchorMax = safeArea.position + safeArea.size;

                if (screenSize.x == 0 || screenSize.y == 0)
                {
                    Debug.LogError($"Screen size is zero: {screenSize}");
                    return;
                }

                anchorMin.x /= screenSize.x;
                anchorMin.y /= screenSize.y;
                anchorMax.x /= screenSize.x;
                anchorMax.y /= screenSize.y;

                rectCanvasTransform.anchorMin = anchorMin;
                rectCanvasTransform.anchorMax = anchorMax;
            }
        }
    }
}