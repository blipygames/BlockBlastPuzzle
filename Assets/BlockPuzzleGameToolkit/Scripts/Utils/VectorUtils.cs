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

namespace BlockPuzzleGameToolkit.Scripts.Utils
{
    public static class VectorUtils
    {
        // Snaps the Z-coordinate of a Vector3 to 0
        public static Vector3 SnapZ(this Vector3 v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        // Snaps the Z-coordinate of a Vector3 to -10 (suitable for cameras)
        public static Vector3 SnapZCamera(this Vector3 v)
        {
            return new Vector3(v.x, v.y, -10);
        }

        // Adjusts the x-coordinate of a Vector2Int if its y-coordinate is even
        public static Vector2Int SnapRow(this Vector2Int v)
        {
            if (v.y % 2 == 0)
            {
                v.x -= 1;
            }

            return v;
        }

        // Returns the next Vector2Int in a specified direction from the current Vector2Int
        private static Vector2Int GetNext(this Vector2Int v, Vector2Int dir)
        {
            if (dir.y != 0)
            {
                if (v.y % 2 == 0 && dir.x < 0)
                {
                    dir.x = 0;
                }
                else if (v.y % 2 == 1 && dir.x > 0)
                {
                    dir.x = 0;
                }
            }

            return v + dir;
        }

        // Overload of GetNext that accepts two integers as direction
        public static Vector2Int GetNext(this Vector2Int v, int x, int y)
        {
            return v.GetNext(new Vector2Int(x, y));
        }

        public static Vector2Int ToV2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }

        public static Vector2Int ToV2Int(this Vector3 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }
    }
}