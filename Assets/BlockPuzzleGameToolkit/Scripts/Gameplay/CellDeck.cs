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

using BlockPuzzleGameToolkit.Scripts.Gameplay.Pool;
using DG.Tweening;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class CellDeck : MonoBehaviour
    {
        public Shape shape;
        public GameObject prefabFX;

        [SerializeField]
        private FieldManager field;

        public bool IsEmpty => shape == null;

        private void Update()
        {
            if (shape != null)
            {
                if (field != null)
                {
                    if (field.CanPlaceShape(shape))
                    {
                        SetShapeTransparency(shape, 1.0f); // Fully opaque
                    }
                    else
                    {
                        SetShapeTransparency(shape, 0.1f); // Semi-transparent
                    }
                }
            }
        }

        public void FillCell(Shape randomShape)
        {
            shape = randomShape;
            if (shape != null)
            {
                shape.transform.SetParent(transform);
                shape.transform.localPosition = Vector3.zero;
                shape.transform.localScale = Vector3.one * 0.5f;
                var scale = shape.transform.localScale;
                shape.transform.localScale = Vector3.zero;
                PoolObject.GetObject(prefabFX, shape.transform.position);
                shape.transform.DOScale(scale, 0.5f).SetEase(Ease.OutBack).OnComplete(() => { shape.transform.localScale = scale; });
            }
        }

        private void SetShapeTransparency(Shape shape, float alpha)
        {
            foreach (var item in shape.GetActiveItems())
            {
                item.SetTransparency(alpha);
            }
        }

        public void ClearCell()
        {
            if (shape != null)
            {
                PoolObject.Return(shape.gameObject);
                shape = null;
            }
        }

        public void RemoveShape()
        {
            if (shape != null)
            {
                Destroy(shape.gameObject);
                shape = null;
            }
        }
    }
}