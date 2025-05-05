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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Gameplay.FX;
using BlockPuzzleGameToolkit.Scripts.GUI;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Enums;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class TargetManager : MonoBehaviour
    {
        private Level level;
        private List<Target> _levelTargetInstance;
        private Dictionary<TargetScriptable, TargetGUIElement> _targetGuiElements;
        public BonusAnimation bonusAnimationPrefab;
        public TargetsUIHandler targetPanel;
        public Transform targetParent;

        private List<BonusAnimation> _bonusAnimations;
        private ObjectPool<BonusAnimation> bonusAnimationPool;
        public Transform fxPool;
        
        private Dictionary<BonusAnimation, TargetScriptable> _activeAnimationTargets;

        private void OnEnable()
        {
            bonusAnimationPool = new ObjectPool<BonusAnimation>(
                () => Instantiate(bonusAnimationPrefab, fxPool),
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                Destroy
            );
            _activeAnimationTargets = new Dictionary<BonusAnimation, TargetScriptable>();
            _bonusAnimations = new List<BonusAnimation>();
        }

        public bool IsLevelComplete()
        {
            return _levelTargetInstance.All(t => t.OnCompleted()) && _levelTargetInstance.Count > 0;
        }

        public bool WillLevelBeComplete()
        {
            if (_levelTargetInstance == null || _levelTargetInstance.Count == 0) return false;

            var pendingDeductions = new Dictionary<TargetScriptable, int>();
            foreach (var targetScriptable in _activeAnimationTargets.Values)
            {
                pendingDeductions[targetScriptable] = pendingDeductions.GetValueOrDefault(targetScriptable, 0) + 1;
            }

            foreach (var target in _levelTargetInstance)
            {
                int currentAmount = target.amount;
                int deductions = pendingDeductions.GetValueOrDefault(target.targetScriptable, 0);
                int predictedAmount = currentAmount - deductions;
                
                if (predictedAmount > 0)
                {
                    return false;
                }
            }

            // Notify that level is about to complete
            EventManager.GetEvent(EGameEvent.LevelAboutToComplete).Invoke();
            return true;
        }

        public void OnLevelLoaded(Level obj)
        {
            level = obj;
            if (level == null)
            {
                return;
            }

            _levelTargetInstance = new List<Target>(level.targetInstance.Where(t => t.amount > 0).Select(t => 
            {
                var target = t.Clone();
                target.totalAmount = t.amount;
                return target;
            }));
            _targetGuiElements = new Dictionary<TargetScriptable, TargetGUIElement>();
            _activeAnimationTargets?.Clear();
            _bonusAnimations?.Clear();
            
            var findObjectOfType = FindObjectOfType<TargetsUIHandler>();
            if (findObjectOfType != null)
            {
                Destroy(findObjectOfType.gameObject);
            }
            if (!GameManager.instance.IsTutorialMode())
            {
                var t = Instantiate(targetPanel, targetParent);
                t.OnLevelLoaded(level.levelType.elevelType);
            }
        }

        public void RegisterTargetGuiElement(TargetScriptable target, TargetGUIElement targetGuiElement)
        {
            _targetGuiElements[target] = targetGuiElement;
            var newCount = _levelTargetInstance.Find(t => t.targetScriptable == target).amount;
            targetGuiElement.UpdateCount(target.descending ? newCount : 0, false);
        }

        public void UpdateTargetCount(Target targetScriptable)
        {
            if (_targetGuiElements.TryGetValue(targetScriptable.targetScriptable, out var targetGuiElement))
            {
                targetGuiElement.UpdateCount(targetScriptable.amount, IsTargetCompleted(targetScriptable));
            }
        }

        private bool IsTargetCompleted(Target targetScriptable)
        {
            return targetScriptable.amount <= 0;
        }

        public List<Target> GetTargets()
        {
            return _levelTargetInstance;
        }

        public Dictionary<TargetScriptable, TargetGUIElement> GetTargetGuiElements()
        {
            return _targetGuiElements;
        }

        public IEnumerator AnimateTarget(List<List<Cell>> lines)
        {
            var bonusItems = new Dictionary<BonusItemTemplate, List<Vector3>>();
            foreach (var cells in lines)
            {
                foreach (var cell in cells)
                {
                    if (cell.HasBonusItem())
                    {
                        var bonusItem = cell.GetBonusItem();
                        if (!bonusItems.ContainsKey(bonusItem))
                        {
                            bonusItems[bonusItem] = new List<Vector3>();
                        }

                        bonusItems[bonusItem].Add(cell.transform.position);
                    }
                }
            }

            foreach (var target in _levelTargetInstance)
            {
                if (target.targetScriptable.bonusItem == null)
                {
                    continue;
                }

                foreach (var bonusItem in bonusItems)
                {
                    if (bonusItem.Key == target.targetScriptable.bonusItem)
                    {
                        foreach (var position in bonusItem.Value)
                        {
                            var bonus = bonusAnimationPool.Get();
                            bonus.Fill(target.targetScriptable.bonusItem);
                            bonus.transform.position = position;
                            bonus.targetPos = _targetGuiElements[target.targetScriptable].transform.position;
                            
                            _activeAnimationTargets[bonus] = target.targetScriptable;
                            _bonusAnimations.Add(bonus);
                            
                            bonus.OnFinish = _ =>
                            {
                                _activeAnimationTargets.Remove(bonus);
                                _bonusAnimations.Remove(bonus);

                                target.amount--;
                                target.amount = Mathf.Max(0, target.amount);
                                
                                var targetTransform = _targetGuiElements[target.targetScriptable].transform;
                                targetTransform.DOScale(Vector3.one * 1.2f, 0.1f)
                                    .SetEase(Ease.OutQuad)
                                    .OnComplete(() => {
                                        targetTransform.DOScale(Vector3.one, 0.1f)
                                            .SetEase(Ease.InQuad);
                                    });
                                    
                                UpdateTargetCount(target);
                                bonusAnimationPool.Release(bonus);
                            };
                        }
                    }
                }
            }

            yield return new WaitForSeconds(.1f);
            var bonusAnimationsCopy = new List<BonusAnimation>(_bonusAnimations);
            foreach (var bonusAnimation in bonusAnimationsCopy)
            {
                if(_activeAnimationTargets.ContainsKey(bonusAnimation))
                {
                    bonusAnimation.MoveTo();
                    yield return new WaitForSeconds(0.04f);
                }
            }

            yield return null;
        }

        public void UpdateScoreTarget(int score)
        {
            var targetScriptable = _levelTargetInstance.Find(t => t.targetScriptable.GetType() == typeof(ScoreTargetScriptable));
            if (targetScriptable != null && _targetGuiElements.TryGetValue(targetScriptable.targetScriptable, out var targetGuiElement))
            {
                var target = _levelTargetInstance.Find(t => t.targetScriptable == targetScriptable.targetScriptable);
                target.amount -= score;
                targetGuiElement.UpdateCount(score, IsTargetCompleted(targetScriptable));
            }
        }

        public bool IsAnimationPlaying()
        {
            return _bonusAnimations != null && _bonusAnimations.Count > 0;
        }
    }
}