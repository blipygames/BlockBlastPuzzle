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
using BlockPuzzleGameToolkit.Scripts.Popups;
using BlockPuzzleGameToolkit.Scripts.Services.Ads.AdUnits;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Settings
{
    public class AdsSettings : ScriptableObject
    {
        public AdSetting[] adProfiles;
    }

    [Serializable]
    public class AdSetting
    {
        public string name;
        public bool enable = true;
        public bool testInEditor;
        public EPlatforms platforms;
        public string appId;
        public AdsHandlerBase adsHandler;
        public AdElement[] adElements;
    }

    [Serializable]
    public class AdElement
    {
        public string placementId;
        public AdReference adReference;

        [Header("Popup that triggers Interstitial ads")]
        public PopupEventAdsSetting popup;
    }

    [Serializable]
    public class PopupEventAdsSetting
    {
        public Popup popup;
        public bool showOnOpen;
        public bool showOnClose;
    }

    public enum EPlatforms
    {
        Android,
        IOS,
        Windows,
        WebGL
    }
}