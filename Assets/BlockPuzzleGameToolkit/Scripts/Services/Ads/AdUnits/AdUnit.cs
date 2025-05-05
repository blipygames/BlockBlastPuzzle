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

namespace BlockPuzzleGameToolkit.Scripts.Services.Ads.AdUnits
{
    public class AdUnit
    {
        public Action<string> OnShown;
        public Action<string> OnInitialized;
        public string PlacementId { get; set; }
        public AdReference AdReference { get; set; }

        public AdsHandlerBase AdsHandler { get; set; }
        public bool Loaded { get; set; }

        public void Complete()
        {
            OnShown?.Invoke(PlacementId);
        }

        public void Initialized()
        {
            OnInitialized?.Invoke(PlacementId);
        }

        public void Load()
        {
            AdsHandler?.Load(this);
        }

        public void Show()
        {
            AdsHandler?.Show(this);
        }

        public bool IsAvailable()
        {
            return AdsHandler != null && (AdsHandler.IsAvailable(this) || Loaded);
        }

        public void Hide()
        {
            AdsHandler?.Hide(this);
        }
    }
}