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
using BlockPuzzleGameToolkit.Scripts.Services.IAP;
using BlockPuzzleGameToolkit.Scripts.System;
using TMPro;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class NoAds : Popup
    {
        public CustomButton removeAdsButton;
        public ProductID productID;
        public TextMeshProUGUI priceText;

        private void OnEnable()
        {
            removeAdsButton.onClick.AddListener(RemoveAds);
            GameManager.instance.purchaseSucceded += PurchaseSucceeded;
            
            if (priceText != null)
            {
                string price = IAPManager.instance.GetProductLocalizedPriceString(productID.ID);
                if (!string.IsNullOrEmpty(price))
                {
                    priceText.text = price;
                }
            }
        }

        private void OnDisable()
        {
            GameManager.instance.purchaseSucceded -= PurchaseSucceeded;
        }

        private void PurchaseSucceeded(string obj)
        {
            if (obj == productID.ID)
            {
                AdsManager.instance.RemoveAds();
                Close();
            }
        }

        private void RemoveAds()
        {
            IAPManager.instance.BuyProduct(productID.ID);
        }
    }
}