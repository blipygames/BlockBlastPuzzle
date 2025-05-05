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

using System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData
{
    [Serializable]
    public class ShapeRow
    {
        public bool[] cells = new bool[5];
    }

    [CreateAssetMenu(fileName = "Shape", menuName = "BlockPuzzleGameToolkit/Items/Shape", order = 1)]
    public class ShapeTemplate : ScriptableObject
    {
        public ShapeRow[] rows = new ShapeRow[5];
        public int scoreForSpawn;

        public float chanceForSpawn = 1;
        public int spawnFromLevel = 1;

        private void OnEnable()
        {
            if (rows == null || rows.Length != 5)
            {
                rows = new ShapeRow[5];
                for (var i = 0; i < 5; i++)
                {
                    rows[i] = new ShapeRow();
                }
            }
        }
    }
}