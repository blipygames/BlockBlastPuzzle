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

using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.GUI.Labels;
using BlockPuzzleGameToolkit.Scripts.Settings;
using TMPro;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Popups.Reward
{
    public class RewardPopup : PopupWithCurrencyLabel
    {
        public Transform iconPos;
        private int _count;
        private ResourceObject _resource;
        private RewardSettingSpin rewardVisual;

        public TextMeshProUGUI countText;

        public void SetReward(RewardSettingSpin rewardVisual)
        {
            this.rewardVisual = rewardVisual;
            _count = rewardVisual.count;
            countText.text = _count.ToString();
            _resource = rewardVisual.resource;
        }

        public override void Close()
        {
            StopInteration();

            LabelAnim.AnimateForResource(_resource, iconPos.position, "+" + _count, _resource.sound, () =>
            {
                rewardVisual.resource.Add(rewardVisual.count);
                base.Close();
            });
        }
    }
}