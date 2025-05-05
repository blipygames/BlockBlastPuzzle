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
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Utils.CommonUI
{
    public class BezierGridLayout : GridLayoutGroup
    {
        public List<Vector3> controlPoints = new List<Vector3>();

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalculatePositions();
        }
        
        public override void CalculateLayoutInputVertical()
        {
            base.CalculateLayoutInputVertical();
            CalculatePositions();
        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();
            CalculatePositions();
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();
            CalculatePositions();
        }

        public void CalculatePositions()
        {
            if (controlPoints.Count < 4 || rectChildren.Count == 0) return;

            float tStart = 0.0f;
            switch (childAlignment)
            {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                    tStart = 0.0f;
                    break;
                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    tStart = rectChildren.Count > 1 ? (0.5f / rectChildren.Count) : 0.5f;
                    break;
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    tStart = 1.0f;
                    break;
            }
            // Calculate bezier curve positions for items
            for (int i = 0; i < rectChildren.Count; ++i)
            {
                var item = rectTransform.GetChild(i) as RectTransform;
    
                // Use the cell size, spacing, and padding to adjust the 't' parameter
                float totalWidth = rectTransform.rect.width - padding.left - padding.right;
                float adjustedCellSize = (this.cellSize.x + this.spacing.x) / totalWidth;
                float t = tStart + adjustedCellSize * i + padding.left / totalWidth;

                item.localPosition = GetPointOnBezierCurve(t);

                // Set the item's rotation based on the curve's tangent
                Vector3 tangent = GetTangentOnBezierCurve(t);
                item.right = tangent;
            }
        }

        public Vector3 GetPointOnBezierCurve(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * controlPoints[0]; // (1-t)^3 * P0
            p += 3 * uu * t * controlPoints[1]; // 3(1-t)^2 * t * P1
            p += 3 * u * tt * controlPoints[2]; // 3(1-t) * t^2 * P2
            p += ttt * controlPoints[3]; // t^3 * P3

            return p;
        }
        
        public Vector3 GetTangentOnBezierCurve(float t)
        {
            float dt = 0.01f;
            Vector3 pointT = GetPointOnBezierCurve(t);
            Vector3 pointTPlusDt = GetPointOnBezierCurve(t + dt);
            return (pointTPlusDt - pointT).normalized;
        }
    }
}