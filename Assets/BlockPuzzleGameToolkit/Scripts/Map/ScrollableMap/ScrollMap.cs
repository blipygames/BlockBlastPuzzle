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
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap
{
    public class ScrollMap : MonoBehaviour
    {
         
        [SerializeField]
        private ScrollRect scrollRect;

        public void ScrollToAvatar(Vector2 vector2)
        {
            Vector3 position = scrollRect.content.position;
            Vector2 contentPositionInLocalSpace = scrollRect.transform.InverseTransformPoint(position);
            Vector2 avatarPositionInLocalSpace = scrollRect.transform.InverseTransformPoint(vector2);

            Vector2 contentAnchoredPosition = contentPositionInLocalSpace - avatarPositionInLocalSpace;

            float aspectRatio = Screen.height / Screen.width;
            float centerOffset = aspectRatio * 1000f;

            scrollRect.content.anchoredPosition = new Vector2(0, contentAnchoredPosition.y + centerOffset);
        }
    }
}