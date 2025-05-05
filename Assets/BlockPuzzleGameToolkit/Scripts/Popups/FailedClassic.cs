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
using TMPro;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class FailedClassic : Failed
    {
        public GameObject failedStuff;
        public GameObject bestScoreStuff;

        public TextMeshProUGUI[] scoreText;
        public TextMeshProUGUI bestScoreText;
        protected BaseModeHandler modeHandler;

        protected override void OnEnable()
        {
            base.OnEnable();
            modeHandler = FindObjectOfType<BaseModeHandler>(false);
            var score = modeHandler.score;
            var bestScore = modeHandler.bestScore;
            scoreText[0].text = score.ToString();
            scoreText[1].text = score.ToString();
            bestScoreText.text = bestScore.ToString();
            if (score > bestScore)
            {
                bestScoreStuff.SetActive(true);
                failedStuff.SetActive(false);
            }
            else
            {
                failedStuff.SetActive(true);
                bestScoreStuff.SetActive(false);
            }
        }
    }
}