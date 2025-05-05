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

using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Utils
{
    public static class RectTransformUtils
    {
        public static void SetAnchors(this RectTransform This, Vector2 AnchorMin, Vector2 AnchorMax)
        {
            var OriginalPosition = This.localPosition;
            var OriginalSize = This.sizeDelta;
            var offsetMin = This.offsetMin;
            var offsetMax = This.offsetMax;

            This.anchorMin = AnchorMin;
            This.anchorMax = AnchorMax;

            This.offsetMin = offsetMin;
            This.offsetMax = offsetMax;
            This.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, OriginalSize.x);
            This.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, OriginalSize.y);
            This.localPosition = OriginalPosition;
        }

        public static (Vector3 min, Vector3 max, Vector2 size, Vector2 center) GetMinMaxAndSizeForCanvas(List<Cell> cells, Canvas canvas)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var cell in cells)
            {
                var bounds = cell.GetBounds();
                min = Vector3.Min(min, bounds.min);
                max = Vector3.Max(max, bounds.max);
            }

            // Convert min and max points from world space to screen space
            var minScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, min);
            var maxScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, max);

            // Convert screen space points to local space using the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, minScreenPoint, canvas.worldCamera, out var minLocalPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, maxScreenPoint, canvas.worldCamera, out var maxLocalPoint);

            // Calculate size in local space
            var sizeInLocalSpace = maxLocalPoint - minLocalPoint;

            // Calculate center in world space and convert to local space
            var centerWorld = (min + max) / 2;
            var centerScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, centerWorld);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, centerScreenPoint, canvas.worldCamera, out var centerLocalPoint);

            return (min, max, sizeInLocalSpace, centerLocalPoint);
        }
    }
}