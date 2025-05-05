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
    public class CloudAnimation : MonoBehaviour
    {
        public float speed = 2f;
        public float leftThreshold = -5f;
        public float rightThreshold = 5f;

        private bool moveRight = true;

        void Update()
        {
            if (moveRight)
            {
                transform.Translate(Vector3.right * (speed * Time.deltaTime));
            }
            else
            {
                transform.Translate(Vector3.left * (speed * Time.deltaTime));
            }

            if (transform.position.x > rightThreshold && moveRight)
            {
                transform.position = new Vector3(rightThreshold, transform.position.y, transform.position.z);
                moveRight = false;
            }
            else if (transform.position.x < leftThreshold && !moveRight)
            {
                transform.position = new Vector3(leftThreshold, transform.position.y, transform.position.z);
                moveRight = true;
            }
        }
    }
}