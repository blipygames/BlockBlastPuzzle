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

namespace BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap
{
    public class MapObjectAppearance : MonoBehaviour
    {
        [SerializeField]
        private RectTransform animateTransform;

        private Canvas canvas;
        private Animator animator;
        private bool _isVisible;

        void Awake()
        {
            // Get the components
            animateTransform = GetComponent<RectTransform>();
            animator = GetComponent<Animator>();
            canvas = GetComponentInParent<Canvas>();
        }

        void Update()
        {
            if (animator != null && !_isVisible && IsVisible())
            {
                _isVisible = true;
                animator.Play("appear");
            }
        }

        private bool IsVisible() {
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, animateTransform.position);
            return screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
                   screenPoint.y >= 0 && screenPoint.y <= Screen.height;
        }
    }
}