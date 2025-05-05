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

using BlockPuzzleGameToolkit.Scripts.Gameplay;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData
{
    [CreateAssetMenu(fileName = "ItemTemplate", menuName = "BlockPuzzleGameToolkit/Items/ItemTemplate", order = 1)]
    public class ItemTemplate : ScriptableData
    {
        public Color backgroundColor;
        public Color underlayColor;
        public Color bottomColor;
        public Color topColor;
        public Color leftColor;
        public Color rightColor;
        public Color overlayColor;
        public Item customItemPrefab;

        public bool HasCustomPrefab() => customItemPrefab != null;
    }
}