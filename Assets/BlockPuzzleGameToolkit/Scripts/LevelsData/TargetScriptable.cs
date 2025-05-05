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
    public class TargetScriptable : ScriptableObject
    {
        public BonusItemTemplate bonusItem;
        public bool descending = true;
    }

    [Serializable]
    public class Target
    {
        public TargetScriptable targetScriptable;
        public int amount;
        public int totalAmount;

        public Target(TargetScriptable targetScriptableTemplate)
        {
            targetScriptable = targetScriptableTemplate;
        }

        public Target Clone()
        {
            return new Target(targetScriptable)
            {
                amount = amount
            };
        }

        public bool OnCompleted()
        {
            return amount <= 0;
        }
    }
}