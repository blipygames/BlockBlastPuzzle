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

using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.GUI.Labels;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEngine;
using System.Collections;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class PreWinScore : PreWin
    {
        public TargetScriptable scoreTarget;
        public TargetScoreGUIElement scoreSlider;
        private bool animationCompleted = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            var targetManager = FindObjectOfType<TargetManager>();

            if (!targetManager.GetTargetGuiElements().TryGetValue(scoreTarget, out var targetGuiElement))
            {
                scoreSlider.UpdateCount(0, false);
                return;
            }

            // Get the final score value
            if (!int.TryParse(targetGuiElement.countText.text, out int finalScore))
            {
                return;
            }

            // Set up the total score and animate
            scoreSlider.totalText.text = finalScore.ToString();
            scoreSlider.scoreSlider.maxValue = finalScore;
            scoreSlider.scoreSlider.value = 0;
            scoreSlider.duration = 1f;
            animationCompleted = false;

            StartCoroutine(AnimateWithDelay(finalScore));
        }

        private IEnumerator AnimateWithDelay(int finalScore)
        {
            yield return new WaitForSeconds(.3f);
            
            scoreSlider.UpdateCount(finalScore, true);
            
            float startTime = Time.time;
            float elapsedTime = 0f;
            
            // Wait until the slider animation is complete or timeout
            while (Mathf.Abs(scoreSlider.scoreSlider.value - finalScore) > 0.01f && elapsedTime < scoreSlider.duration * 1.5f)
            {
                elapsedTime = Time.time - startTime;
                yield return null;
            }

            // Force the final value if we timed out
            if (Mathf.Abs(scoreSlider.scoreSlider.value - finalScore) > 0.01f)
            {
                scoreSlider.scoreSlider.value = finalScore;
            }
            
            animationCompleted = true;
            yield return new WaitForSeconds(.1f);
            base.AfterShowAnimation();
            yield return new WaitForSeconds(.5f);
            Close();

        }

        public override void AfterShowAnimation()
        {
        }
    }
}