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
    public static class TransformUtils
    {
        public static void SetParentPosition(this Transform mainTR, Vector3 v)
        {
            var list = new List<Transform>();
            var parent = mainTR.parent;
            for (var i = parent.childCount - 1; i >= 0; --i)
            {
                var child = parent.GetChild(i);
                child.SetParent(parent.parent);
                list.Add(child);
            }

            parent.transform.position = v;

            foreach (var child in list)
            {
                child.SetParent(parent, true);
            }
        }
    }
}