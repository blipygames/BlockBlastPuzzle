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

using BlockPuzzleGameToolkit.Scripts.Services.Ads.AdUnits;
using UnityEngine;
using UnityEngine.Advertisements;

namespace BlockPuzzleGameToolkit.Scripts.Services.Ads.Networks
{
    #if UNITY_ADS
    [CreateAssetMenu(fileName = "UnityBannerAdsHandler", menuName = "BlockPuzzleGameToolkit/Ads/UnityBannerAdsHandler")]
    public class UnityBannerAdsHandler : AdsHandlerBase
    {
        private bool initialized;
        private IAdsListener listener;
        private readonly BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

        public override void Init(string _id, bool adSettingTestMode, IAdsListener listener)
        {
            this.listener = listener;
            initialized = true;
        }

        public override void Show(AdUnit adUnit)
        {
            if (!initialized)
            {
                DebugLog("Unity Ads not initialized. Cannot show banner.");
                return;
            }

            Advertisement.Banner.SetPosition(bannerPosition);
            Advertisement.Banner.Show(adUnit.PlacementId, new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            });
            listener?.Show(adUnit);
        }

        public override void Load(AdUnit adUnit)
        {
            if (!initialized)
            {
                DebugLog("Unity Ads not initialized. Cannot load banner.");
                return;
            }

            Advertisement.Banner.Load(adUnit.PlacementId, new BannerLoadOptions
            {
                loadCallback = () => OnBannerLoaded(adUnit.PlacementId),
                errorCallback = OnBannerError
            });
        }

        public override bool IsAvailable(AdUnit adUnit)
        {
            return initialized && Advertisement.Banner.isLoaded;
        }

        public override void Hide(AdUnit adUnit)
        {
            Advertisement.Banner.Hide();
        }

        private void OnBannerLoaded(string placementId)
        {
            DebugLog($"Banner loaded: {placementId}");
            listener?.OnAdsLoaded(placementId);
        }

        private void OnBannerError(string message)
        {
            DebugLog($"Banner load error: {message}");
            listener?.OnAdsLoadFailed();
        }

        private void OnBannerClicked()
        {
            DebugLog("Banner clicked");
            listener?.OnAdsShowClick();
        }

        private void OnBannerShown()
        {
            DebugLog("Banner shown");
            listener?.OnAdsShowStart();
        }

        private void OnBannerHidden()
        {
            DebugLog("Banner hidden");
        }

        private void DebugLog(string msg)
        {
            Debug.Log($"[UnityBannerAdsHandler] {msg}");
        }
    }
    #endif
}