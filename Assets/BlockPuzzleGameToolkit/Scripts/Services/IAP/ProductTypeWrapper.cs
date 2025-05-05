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

namespace BlockPuzzleGameToolkit.Scripts.Services.IAP
{
    /// <summary>
    ///     Wrapper for the ProductType in case the Unity Purchasing system is not available.
    /// </summary>
    public class ProductTypeWrapper
    {
        // Enum definition to be used when the purchasing library is not available.
        public enum ProductType
        {
            Consumable,
            NonConsumable,
            Subscription
        }

        #if UNITY_PURCHASING
        public static UnityEngine.Purchasing.ProductType GetProductType(ProductType productType)
        {
            return (UnityEngine.Purchasing.ProductType)(int)productType;
        }
        #else
        public static ProductType GetProductType(ProductType productType)
        {
            return productType;
        }
        #endif
    }
}