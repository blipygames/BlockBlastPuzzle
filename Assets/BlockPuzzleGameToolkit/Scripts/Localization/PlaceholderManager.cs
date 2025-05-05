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

using System.Text.RegularExpressions;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Managers;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization
{
    // This class is used to replace placeholders in localized strings with actual values.
    public static class PlaceholderManager
    {
        public static string GetPlaceholderValue(string placeholderKey)
        {
            switch (placeholderKey)
            {
                case "level":
                    return GetCurrentLevel().ToString();
                case "continueTimerBonus":
                    return GameManager.instance.GameSettings.continueTimerBonus.ToString();
                case "timeLeft":
                    var timerManager = Object.FindObjectOfType<TimerManager>();
                    if (timerManager != null)
                    {
                        int minutes = Mathf.FloorToInt(timerManager.RemainingTime / 60);
                        int seconds = Mathf.FloorToInt(timerManager.RemainingTime % 60);
                        return $"{minutes:00}:{seconds:00}";
                    }
                    return "00:00";
                default:
                    if (PlayerPrefs.HasKey(placeholderKey))
                    {
                        return PlayerPrefs.GetString(placeholderKey);
                    }

                    return "{" + placeholderKey + "}";
            }
        }

        public static string ReplacePlaceholders(string input)
        {
            return Regex.Replace(input, @"\{(\w+)\}", match =>
            {
                var placeholderKey = match.Groups[1].Value;
                return GetPlaceholderValue(placeholderKey);
            });
        }

        private static int GetCurrentLevel()
        {
            return GameDataManager.GetLevelNum();
        }
    }
}