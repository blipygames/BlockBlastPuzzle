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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace BlockPuzzleGameToolkit.Scripts.Services.IAP
{
    #if UNITY_PURCHASING
    public class IAPController : IDetailedStoreListener, IIAPService
    {
        private static IStoreController storeController;
        private IExtensionProvider extensionProvider;

        public static event Action<string> OnSuccessfulPurchase;
        public static event Action<bool, List<string>> OnRestorePurchasesFinished;

        public void InitializePurchasing(IEnumerable<(string productId, ProductTypeWrapper.ProductType productType)> products)
        {
            if (IsInitialized())
            {
                return;
            }

            var standardPurchasingModule = StandardPurchasingModule.Instance();
            // #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            // standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            // standardPurchasingModule.useFakeStoreAlways = true;
            // #endif
            var builder = ConfigurationBuilder.Instance(standardPurchasingModule);

            foreach (var (productId, productType) in products)
            {
                builder.AddProduct(productId, ProductTypeWrapper.GetProductType(productType));
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsInitialized()
        {
            return storeController != null;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            extensionProvider = extensions;
            storeController = controller;
            RestorePurchases((success, restoredProducts) =>
            {
                if (success)
                {
                    Debug.Log($"Restore purchases succeeded. Restored products: {string.Join(", ", restoredProducts)}");
                    foreach (var productId in restoredProducts)
                    {
                        MarkProductAsPurchased(productId);
                    }
                }
            });
        }


        public void Restore(Action<bool, List<string>> action)
        {
            RestorePurchases((success, restoredProducts) =>
            {
                if (success)
                {
                    action?.Invoke(true, restoredProducts);
                    Debug.Log($"Restore purchases succeeded. Restored products: {string.Join(", ", restoredProducts)}");
                    foreach (var productId in restoredProducts)
                    {
                        MarkProductAsPurchased(productId);
                    }
                }
            });
        }

        private void MarkProductAsPurchased(string productId)
        {
            PlayerPrefs.SetInt("Purchased_" + productId, 1);
            PlayerPrefs.Save();
        }

        public bool IsProductPurchased(string productId)
        {
            return PlayerPrefs.GetInt("Purchased_" + productId, 0) == 1;
        }

        public void BuyProduct(string productId)
        {
            try
            {
                if (IsInitialized())
                {
                    var product = storeController.products.WithID(productId);

                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                        storeController.InitiatePurchase(product);
                    }
                    else
                    {
                        Debug.Log($"BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase {productId}");
                    }
                }
                else
                {
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            catch (Exception e)
            {
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }

        public decimal GetProductLocalizedPrice(string productId)
        {
            if (IsInitialized())
            {
                var product = storeController.products.WithID(productId);
                if (product != null)
                {
                    return product.metadata.localizedPrice;
                }
            }

            return 0m;
        }

        public string GetProductLocalizedPriceString(string productId)
        {
            if (IsInitialized())
            {
                var product = storeController.products.WithID(productId);
                if (product != null)
                {
                    return product.metadata.localizedPriceString;
                }
            }

            return string.Empty;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log("OnPurchaseFailed: FAIL. Product: " + product.definition.id + " PurchaseFailureDescription: " + failureDescription);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.Log($"OnPurchaseFailed: FAIL. Product: '{i.definition.id}', PurchaseFailureReason: {p}");
        }

        public void OnInitializeFailed(InitializationFailureReason reason)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + reason);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error + " message: " + message);
        }

        public void RestorePurchases(Action<bool, List<string>> onRestore)
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                onRestore(false, new List<string>());
                return;
            }

            var restoredProducts = new List<string>();

            Action<bool> restoreCallback = success =>
            {
                if (success)
                {
                    foreach (var product in storeController.products.all)
                    {
                        if (product.hasReceipt)
                        {
                            restoredProducts.Add(product.definition.id);
                        }
                    }
                }

                Debug.Log($"RestorePurchases finished. Success: {success}, Restored products: {string.Join(", ", restoredProducts)}");
                onRestore(success, restoredProducts);
                OnRestorePurchasesFinished?.Invoke(success, restoredProducts);
            };

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");

                var apple = extensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result, message) =>
                {
                    Debug.Log("RestorePurchases continuing: " + message);
                    restoreCallback(result);
                });
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                var googlePlayStoreExtensions = extensionProvider.GetExtension<IGooglePlayStoreExtensions>();
                googlePlayStoreExtensions.RestoreTransactions((success, message) =>
                {
                    if (success)
                    {
                        Debug.Log("Transactions restored successfully: " + message);
                    }
                    else
                    {
                        Debug.LogError("Failed to restore transactions: " + message);
                    }
                    restoreCallback(success);
                });
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
                restoreCallback(false);
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");

            if (args.purchasedProduct.definition.type == ProductType.NonConsumable)
            {
                MarkProductAsPurchased(args.purchasedProduct.definition.id);
            }

            OnSuccessfulPurchase?.Invoke(args.purchasedProduct.definition.id);

            return PurchaseProcessingResult.Complete;
        }
    }
    #endif
}