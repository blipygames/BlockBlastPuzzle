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

namespace BlockPuzzleGameToolkit.Scripts.Services.Ads.Networks
{
    [CreateAssetMenu(fileName = "IronsourceBannerHandler", menuName = "BlockPuzzleGameToolkit/Ads/IronsourceBannerHandler")]
    public class IronsourceBannerHandler : AdsHandlerBase
    {
        private IAdsListener _listener;

        private void Init(string _id)
        {
            #if IRONSOURCE
            IronSource.Agent.validateIntegration();
            IronSource.Agent.init(_id);
            #endif
        }

        private void SetListener(IAdsListener listener)
        {
            _listener = listener;
            #if IRONSOURCE
            IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
            #endif
        }

        #if IRONSOURCE
        private void BannerAdLoadedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource Banner ad loaded");
            _listener?.OnAdsLoaded(adInfo.instanceId);
        }

        private void BannerAdLoadFailedEvent(IronSourceError error)
        {
            Debug.Log($"IronSource Banner ad load failed. Error: {error.getCode()} - {error.getDescription()}");
            _listener?.OnAdsLoadFailed();
        }

        private void BannerAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource Banner ad clicked");
        }

        private void BannerAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource Banner ad screen presented");
        }

        private void BannerAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource Banner ad screen dismissed");
        }

        private void BannerAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IronSource Banner ad caused app to leave");
        }
        #endif

        public override void Init(string _id, bool adSettingTestMode, IAdsListener listener)
        {
            Debug.Log("IronSource Banner Init");
            Init(_id);
            SetListener(listener);
        }

        public override void Show(AdUnit adUnit)
        {
            #if IRONSOURCE
            if (adUnit.AdReference.adType == EAdType.Banner)
            {
                IronSource.Agent.displayBanner();
                _listener?.Show(adUnit);
            }
            #endif
        }

        public override void Load(AdUnit adUnit)
        {
            #if IRONSOURCE
            if (adUnit.AdReference.adType == EAdType.Banner)
            {
                IronSourceBannerSize bannerSize = IronSourceBannerSize.BANNER;
                IronSource.Agent.loadBanner(bannerSize, IronSourceBannerPosition.BOTTOM);
            }
            #endif
        }

        public override bool IsAvailable(AdUnit adUnit)
        {
            // IronSource doesn't provide a direct method to check if a banner is available
            // You might want to implement your own logic to track banner availability
            return true;
        }

        public override void Hide(AdUnit adUnit)
        {
            #if IRONSOURCE
            if (adUnit.AdReference.adType == EAdType.Banner)
            {
                IronSource.Agent.hideBanner();
            }
            #endif
        }
    }
}