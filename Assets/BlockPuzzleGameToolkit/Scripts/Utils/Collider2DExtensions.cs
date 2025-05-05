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

namespace BlockPuzzleGameToolkit.Scripts.Utils
{
    public static class Collider2DExtensions
    {
        public static void TryUpdateShapeToAttachedSprite(this PolygonCollider2D collider)
        {
            collider.UpdateShapeToSprite(collider.GetComponent<SpriteRenderer>().sprite);
        }

        public static void UpdateShapeToSprite(this PolygonCollider2D collider, Sprite sprite)
        {
            if (collider != null && sprite != null)
            {
                collider.pathCount = sprite.GetPhysicsShapeCount();
                var path = new List<Vector2>();
                for (var i = 0; i < collider.pathCount; i++)
                {
                    path.Clear();
                    sprite.GetPhysicsShape(i, path);
                    collider.SetPath(i, path.ToArray());
                }
            }
        }
    }
}