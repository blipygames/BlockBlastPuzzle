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

using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Audio;
using BlockPuzzleGameToolkit.Scripts.Data;
using BlockPuzzleGameToolkit.Scripts.GUI.Labels;
using BlockPuzzleGameToolkit.Scripts.Services.IAP;
using BlockPuzzleGameToolkit.Scripts.Settings;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class CoinsShop : PopupWithCurrencyLabel
    {
        public ItemPurchase[] packs;
        private CoinsShopSettings shopSettings;

        [SerializeField]
        private ItemPurchase watchAd;

        private void OnEnable()
        {
            shopSettings = Resources.Load<CoinsShopSettings>("Settings/CoinsShopSettings");
            foreach (var itemPurchase in packs)
            {
                if (shopSettings.coinsProducts.Count > 0)
                {
                    var productID = itemPurchase.productID;
                    if (shopSettings.coinsProducts.TryToGetPair(kvp => kvp.Key == productID, out var settingsShopItem))
                    {
                        itemPurchase.settingsShopItem = settingsShopItem;
                        itemPurchase.count.text = settingsShopItem.Value.ToString();
                    }

                    if (productID.productType == ProductTypeWrapper.ProductType.NonConsumable &&
                        PlayerPrefs.GetInt("Purchased_" + productID.ID, 0) == 1)
                    {
                        itemPurchase.gameObject.SetActive(false);
                    }
                }
            }

            watchAd.count.text = GameManager.instance.GameSettings.coinsForAd.ToString();

            GameManager.instance.purchaseSucceded += PurchaseSucceded;
        }

        private void OnDisable()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.purchaseSucceded -= PurchaseSucceded;
            }
        }

        private void PurchaseSucceded(string id)
        {
            var shopItem = packs.First(i => i.productID.ID == id);
            var count = shopItem.settingsShopItem.Value;
            LabelAnim.AnimateForResource(shopItem.resource, shopItem.BuyItemButton.transform.position, "+" + count, SoundBase.instance.coins, () =>
            {
                ResourceManager.instance.GetResource("Coins").Add(count);
                GetComponentInParent<Popup>().CloseDelay();
            });

            // If the item is non-consumable, mark it as purchased
            if (shopItem.productID.productType == ProductTypeWrapper.ProductType.NonConsumable)
            {
                PlayerPrefs.SetInt("Purchased_" + id, 1);
                PlayerPrefs.Save();

                // Disable the button for this item
                var pack = shopItem;
                if (pack.BuyItemButton != null)
                {
                    pack.BuyItemButton.interactable = false;
                }
            }
        }

        public void BuyCoins(string id)
        {
            // StopInteration();
#if UNITY_WEBGL
            GameManager.instance.PurchaseSucceeded(id);
#else
            IAPManager.instance.BuyProduct(id);
#endif
        }

        public void AwawrdCoins()
        {
            var coins = GameManager.instance.GameSettings.coinsForAd;
            var resourceObject = ResourceManager.instance.GetResource("Coins");
            LabelAnim.AnimateForResource(resourceObject, watchAd.BuyItemButton.transform.position, "+" + coins, SoundBase.instance.coins, () =>
            {
                resourceObject.Add(coins);
                GetComponentInParent<Popup>().Close();
            });
        }
    }
}