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
using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.Services.IAP;
using TMPro;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class ItemPurchase : MonoBehaviour
    {
        public CustomButton BuyItemButton;
        public TextMeshProUGUI price;
        public TextMeshProUGUI count;
        public TextMeshProUGUI discountPercent;

        [HideInInspector]
        public KeyValuePair<ProductID, int> settingsShopItem;

        public ProductID productID;

        [SerializeField]
        public ResourceObject resource;

        private void OnEnable()
        {
            BuyItemButton?.onClick.AddListener(BuyCoins);
            if (productID != null)
            {
                var priceValue = IAPManager.instance.GetProductLocalizedPrice(productID.ID);
                if (priceValue > 0.01m)
                {
                    price.text = IAPManager.instance.GetProductLocalizedPriceString(productID.ID);
                }
            }
        }

        private void BuyCoins()
        {
            if (productID != null)
            {
                GetComponentInParent<CoinsShop>().BuyCoins(productID.ID);
            }
        }
    }

    internal class NoAdsItemPurchase : ItemPurchase
    {
    }
}