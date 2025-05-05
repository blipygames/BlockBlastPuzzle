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

using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.GUI.Labels
{
    public class TargetScoreGUIElement : TargetGUIElement
    {
        public Transform circleBack;
        public Slider scoreSlider;
        public TextMeshProUGUI totalText;
        private TargetManager targetInstance;
        public float duration = 0.5f;
        private Tween currentTween;

        private void OnEnable()
        {
            scoreSlider.onValueChanged.AddListener(UpdateScoreText);
            
            // Check if this element is in a popup by looking for PreWinScore component in parent hierarchy
            var preWinScore = GetComponentInParent<BlockPuzzleGameToolkit.Scripts.Popups.PreWinScore>();
            if (preWinScore != null)
            {
                return;
            }
            
            targetInstance = FindObjectOfType<TargetManager>(true);
            var targets = targetInstance.GetTargets();
            if (targets != null)
            {
                Init(targets);
            }
        }

        private void Init(List<Target> obj)
        {
            foreach (var target in obj)
            {
                if (target.amount == 0)
                {
                    continue;
                }

                if (target.targetScriptable.GetType() == typeof(ScoreTargetScriptable))
                {
                    targetInstance.RegisterTargetGuiElement(target.targetScriptable, this);
                    totalText.text = target.totalAmount.ToString();
                    SetupScoreSlider(target.amount);
                    return;
                }
            }
        }

        private void SetupScoreSlider(int maxValue)
        {
            scoreSlider.maxValue = maxValue;
            scoreSlider.value = 0;
        }

        public override void UpdateCount(int newCount, bool isTargetCompleted)
        {
            float targetValue = scoreSlider.value + newCount;
            currentTween = scoreSlider.DOValue(targetValue, duration)
                .SetEase(Ease.InOutQuad);
        }

        private void UpdateScoreText(float value)
        {
            countText.text = value.ToString("0");

            var handler = scoreSlider.handleRect;
            if (handler != null)
            {
                circleBack.position = handler.position;
            }
        }

        private void OnDisable()
        {
            scoreSlider.onValueChanged.RemoveListener(UpdateScoreText);
        }
    }
}