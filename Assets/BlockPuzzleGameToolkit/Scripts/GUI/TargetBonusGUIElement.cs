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

using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI
{
    public class TargetBonusGUIElement : TargetGUIElement
    {
        public Bonus bonus;
        public GameObject check;

        public void FillElement(BonusItemTemplate bonusItemTemplate, int targetAmount)
        {
            bonus.FillIcon(bonusItemTemplate);
            countText.text = targetAmount.ToString();
        }

        public override void UpdateCount(int newCount, bool isTargetCompleted)
        {
            base.UpdateCount(newCount, isTargetCompleted);
            if (isTargetCompleted || newCount == 0)
            {
                TargetCheck();
            }
        }

        public void TargetCheck()
        {
            countText.gameObject.SetActive(false);
            check.SetActive(true);
        }
    }
}