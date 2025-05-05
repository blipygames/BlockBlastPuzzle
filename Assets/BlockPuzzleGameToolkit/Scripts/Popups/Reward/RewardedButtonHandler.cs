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

using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.Services;
using BlockPuzzleGameToolkit.Scripts.Services.Ads.AdUnits;
using UnityEngine;
using UnityEngine.Events;

namespace BlockPuzzleGameToolkit.Scripts.Popups.Reward
{
    public class RewardedButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private AdReference adReference;

        [SerializeField]
        private CustomButton rewardedButton;

        [SerializeField]
        private UnityEvent onRewardedAdComplete;

        [SerializeField]
        private UnityEvent onRewardedShow;

        private void Awake()
        {
            rewardedButton.onClick.AddListener(ShowRewardedAd);
        }

        private void ShowRewardedAd()
        {
            if (AdsManager.instance.IsRewardedAvailable(adReference))
            {
                onRewardedShow?.Invoke();
                AdsManager.instance.ShowAdByType(adReference, _ => onRewardedAdComplete?.Invoke());
            }
            else
            {
                Debug.Log("Rewarded ad is not available");
            }
        }
    }
}