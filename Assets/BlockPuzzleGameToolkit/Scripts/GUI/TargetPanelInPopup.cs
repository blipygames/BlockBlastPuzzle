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
using BlockPuzzleGameToolkit.Scripts.Popups;
using BlockPuzzleGameToolkit.Scripts.System;
using DG.Tweening;
using UnityEngine;
using System;

namespace BlockPuzzleGameToolkit.Scripts.GUI
{
    public class TargetPanelInPopup : TargetPanelBase
    {
        private Popup _popup;
        [SerializeField] private bool animate = true;

        protected override void OnEnableInternal()
        {
            _popup = GetComponentInParent<Popup>();
            if (_popup != null)
            {
                ShowTargets();
                _popup.OnShowAction += AnimateTargets;
            }
        }

        protected override void OnDisableInternal()
        {
            if (_popup != null)
            {
                _popup.OnShowAction -= AnimateTargets;
            }
        }

        private void ShowTargets()
        {
            var targets = targetManager.GetTargetGuiElements();
            foreach (var target in targets)
            {
                var targetElement = Instantiate(target.Value, transform);
                targetElement.transform.localScale = animate ? Vector3.zero : Vector3.one * 1.5f;
                if (EventManager.GameStatus == EGameState.PreWin || EventManager.GameStatus == EGameState.Win)
                {
                    targetElement.GetComponent<TargetBonusGUIElement>().TargetCheck();
                }
            }
        }

        public void AnimateTargets()
        {
            if (!animate) return;
            
            float delay = 0f;
            var childCount = transform.childCount;
            var currentChild = 0;
            
            foreach (Transform child in transform)
            {
                currentChild++;
                var sequence = DOTween.Sequence();
                sequence.Append(child.DOScale(Vector3.one * 1.5f, 0.1f).SetEase(Ease.OutBack).SetDelay(delay));
                
                if (currentChild == childCount)
                {
                    sequence.OnComplete(() => OnAnimationComplete?.Invoke());
                }
                
                delay += 0.01f;
            }
        }

        public Action OnAnimationComplete;
    }
} 