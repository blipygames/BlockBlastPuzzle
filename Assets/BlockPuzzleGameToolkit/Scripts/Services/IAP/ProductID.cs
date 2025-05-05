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

namespace BlockPuzzleGameToolkit.Scripts.Services.IAP
{
    [CreateAssetMenu(fileName = "ProductID", menuName = "BlockPuzzleGameToolkit/IAP/ProductID", order = 0)]
    public class ProductID : ScriptableObject
    {
        public ProductTypeWrapper.ProductType productType;

        public string androidId;
        public string iosId;

        public string ID {
            get {
                if (Application.platform == RuntimePlatform.Android) {
                    return androidId;
                } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                    return iosId;
                } else {
                    return androidId; // existing 'id' field
                }
            }
        }
    }
}