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
using BlockPuzzleGameToolkit.Scripts.Services.Ads.AdUnits;

#if ADMOB
using GoogleMobileAds.Api;
#endif

namespace BlockPuzzle.Scripts.Services.Ads.Networks
{
    [CreateAssetMenu(fileName = "AdmobBannerHandler", menuName = "BlockPuzzleGameToolkit/Ads/AdmobBannerHandler")]
    public class AdmobBannerHandler : AdsHandlerBase
    {
        private IAdsListener _listener;
        #if ADMOB
        private BannerView _bannerView;
        #endif
        private bool _isInitialized = false;
        private bool _isBannerLoaded = false;

        public override void Init(string _id, bool adSettingTestMode, IAdsListener listener)
        {
            #if ADMOB
            _listener = listener;
            _isInitialized = true;
            Debug.Log("AdMob Banner Handler initialized.");
            _listener?.OnAdsInitialized();
            #endif
        }

        public override void Show(AdUnit adUnit)
        {
            #if ADMOB
            _listener?.Show(adUnit);
            _bannerView?.Show();
            Debug.Log("Showing banner ad.");
            #endif
        }

        public override void Load(AdUnit adUnit)
        {
            #if ADMOB
            if (!_isInitialized)
            {
                Debug.LogError("AdMob Banner Handler is not initialized.");
                return;
            }

            // Destroy any existing banner before creating a new one
            if (_bannerView != null)
            {
                _bannerView.Destroy();
            }

            _bannerView = new BannerView(adUnit.PlacementId, AdSize.Banner, AdPosition.Bottom);

            // Add event handlers
            _bannerView.OnBannerAdLoaded += () =>
            {
                _isBannerLoaded = true;
                Debug.Log("Banner ad loaded successfully.");
                _listener?.OnAdsLoaded(adUnit.PlacementId);
            };
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                _isBannerLoaded = false;
                Debug.LogError($"Banner ad failed to load with error : {error.GetMessage()}");
                _listener?.OnAdsLoadFailed();
            };
            _bannerView.OnAdPaid += (AdValue adValue) => { Debug.Log($"Banner ad paid {adValue.Value} {adValue.CurrencyCode}"); };
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("Banner ad clicked.");
                _listener?.OnAdsShowClick();
            };
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Banner ad impression recorded.");
                _listener?.OnAdsShowStart();
            };

            // Create an empty ad request
            AdRequest adRequest = new AdRequest();

            // Load the banner with the request
            _bannerView.LoadAd(adRequest);
            Debug.Log("Requested banner ad load.");
            #endif
        }

        public override bool IsAvailable(AdUnit adUnit)
        {
            #if ADMOB
            return _isInitialized && _isBannerLoaded;
            #else
            return false;
            #endif
        }

        public override void Hide(AdUnit adUnit)
        {
            #if ADMOB
            if (_bannerView != null)
            {
                _bannerView.Hide();
                Debug.Log("Banner ad hidden.");
            }
            #endif
        }

        public void DestroyBanner()
        {
            #if ADMOB
            if (_bannerView != null)
            {
                _bannerView.Destroy();
                _bannerView = null;
                _isBannerLoaded = false;
                Debug.Log("Banner ad destroyed.");
            }
            #endif
        }

        private void OnDisable()
        {
            DestroyBanner();
        }
    }
}