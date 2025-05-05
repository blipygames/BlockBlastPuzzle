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

using BlockPuzzleGameToolkit.Scripts.Localization;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Managers;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class PreFailedTitleManager : MonoBehaviour
    {
        [SerializeField] private LocalizedTextMeshProUGUI titleText;

        private void OnEnable()
        {
            UpdateTitleText();
        }

        private void UpdateTitleText()
        {
            var level = GameDataManager.GetLevel();
            if (!level.enableTimer)
            {
                titleText.instanceID = "REVIVEWITHNEWSHAPES";
            }
            else
            {
                // For timer-enabled levels, check remaining time
                var timerManager = FindObjectOfType<TimerManager>();
                if (timerManager != null && timerManager.RemainingTime > 0)
                {
                    titleText.instanceID = "REVIVEWITHNEWSHAPES";
                }
                else
                {
                    titleText.instanceID = "CONTINUEWITHEXTRASECONDS";
                }
            }
            titleText.UpdateText();
        }
    }
}