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

using BlockPuzzleGameToolkit.Scripts.Enums;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Settings
{

    public class GameSettings : SettingsBase
    {
        [Header("On start")]
        public int coins;

        [Header("Monetization")]
        public int coinsForAd = 25;
        public bool enableAds = true;
        public bool enableInApps = true;
        public bool enableLuckySpin = true;
        public bool enablePreFailedPopup = true;

        [Header("GDPR settings")]
        public string privacyPolicyUrl;

        [Header("Timed mode")]
        public bool enableTimedMode = false;
        public int globalTimedModeSeconds = 60; // Default time value for timed mode in seconds
        public int continueTimerBonus = 30;

        [Header("Gameplay")]
        public int ScorePerLine = 10;
        public bool enablePool;
        public int ResetComboAfterMoves = 3;

        public int continuePrice = 15;
        public int failedTimerStart = 5;

        [Header("Map settings")]
        public EMapType mapType = EMapType.Tiled;
        public int maxLevelsInRow = 8;
        public int maxRows = 8;
    }
}