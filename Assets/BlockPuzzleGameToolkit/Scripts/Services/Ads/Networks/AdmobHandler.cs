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
#if ADMOB
using GoogleMobileAds.Api;
#endif

namespace BlockPuzzleGameToolkit.Scripts.Services.Ads.Networks
{
    [CreateAssetMenu(fileName = "AdmobHandler", menuName = "BlockPuzzleGameToolkit/Ads/AdmobHandler")]
    public class AdmobHandler : AdsHandlerBase
    {
        private IAdsListener _listener;
        #if ADMOB
        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        #endif

        public override void Init(string _id, bool adSettingTestMode, IAdsListener listener)
        {
            #if ADMOB
            _listener = listener;
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            MobileAds.Initialize(initstatus =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Ads initialization failed.");
                    return;
                }

                // If you use mediation, you can check the status of each adapter.
                var adapterStatusMap = initstatus.getAdapterStatusMap();
                if (adapterStatusMap != null)
                {
                    foreach (var item in adapterStatusMap)
                    {
                        Debug.Log(string.Format("Adapter {0} is {1}",
                            item.Key,
                            item.Value.InitializationState));
                    }
                }

                Debug.Log("Google Mobile Ads initialization complete.");
                _listener?.OnAdsInitialized();
            });
            #endif
        }

        public override void Show(AdUnit adUnit)
        {
            #if ADMOB
            _listener?.Show(adUnit);

            if (adUnit.AdReference.adType == EAdType.Interstitial)
            {
                if (_interstitialAd != null && _interstitialAd.CanShowAd())
                {
                    Debug.Log("Showing interstitial ad.");
                    _interstitialAd.Show();
                }
                else
                {
                    Debug.LogError("Interstitial ad is not ready yet.");
                }
            }
            else if (adUnit.AdReference.adType == EAdType.Rewarded)
            {
                if (_rewardedAd != null && _rewardedAd.CanShowAd())
                {
                    _rewardedAd.Show(reward =>
                    {
                        Debug.Log(string.Format("Rewarded ad granted a reward: {0} {1}",
                            reward.Amount,
                            reward.Type));
                    });
                }
                else
                {
                    Debug.LogError("Rewarded ad is not ready yet.");
                }
            }
            #endif
        }

        public override void Load(AdUnit adUnit)
        {
            #if ADMOB
            var adRequest = new AdRequest();

            if (adUnit.AdReference.adType == EAdType.Interstitial)
            {
                InterstitialAd.Load(adUnit.PlacementId, adRequest, (ad, error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                        return;
                    }

                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                        return;
                    }

                    _interstitialAd = ad;
                    Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());

                    ad.OnAdFullScreenContentClosed += () => _listener.OnAdsShowComplete();

                    _listener?.OnAdsLoaded(adUnit.PlacementId);
                });
            }
            else if (adUnit.AdReference.adType == EAdType.Rewarded)
            {
                RewardedAd.Load(adUnit.PlacementId, adRequest, (ad, error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                        return;
                    }

                    if (ad == null)
                    {
                        Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                        return;
                    }

                    _rewardedAd = ad;

                    ad.OnAdFullScreenContentClosed += () => _listener.OnAdsShowComplete();

                    Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                    _listener?.OnAdsLoaded(adUnit.PlacementId);
                });
            }
            #endif
        }

        public override bool IsAvailable(AdUnit adUnit)
        {
            return false;
        }

        public override void Hide(AdUnit adUnit)
        {
        }
    }
}