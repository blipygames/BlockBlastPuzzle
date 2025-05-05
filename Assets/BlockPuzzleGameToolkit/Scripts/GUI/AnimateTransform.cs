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
using DG.Tweening;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public class AnimateTransform : MonoBehaviour
    {
        public Move position;
        public Rotate rotation;
        public Scale scale;
        public Size sizeDelta;

        private RectTransform rectTransform;

        private void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();

            if (position.targetValue != Vector3.zero)
            {
                rectTransform.anchoredPosition = position.startValue;
                position.Animate(rectTransform);
            }

            if (rotation.targetValue != Vector3.zero)
            {
                rectTransform.eulerAngles = rotation.startValue;
                rotation.Animate(rectTransform);
            }

            if (scale.targetValue != Vector3.zero)
            {
                rectTransform.localScale = scale.startValue;
                scale.Animate(rectTransform);
            }

            if (sizeDelta.targetValue != Vector3.zero)
            {
                rectTransform.sizeDelta = sizeDelta.startValue;
                sizeDelta.Animate(rectTransform);
            }
        }

        private void OnDisable()
        {
            rectTransform.DOKill();
        }
    }

    [Serializable]
    public abstract class TweenAnimationParameters
    {
        public Vector3 startValue;
        public Vector3 targetValue;
        public float duration = 1f;
        public float delay;
        public Ease easeType = Ease.Linear;
        public bool loop;
        public LoopType loopType;

        public void Animate(RectTransform rectTransform)
        {
            GetTween(rectTransform).SetEase(easeType).SetDelay(delay).SetLoops(loop ? -1 : 1, loopType);
        }

        protected abstract Tweener GetTween(RectTransform rectTransform);
    }

    [Serializable]
    public class Move : TweenAnimationParameters
    {
        protected override Tweener GetTween(RectTransform rectTransform)
        {
            return rectTransform.DOLocalMove(targetValue, duration);
        }
    }

    [Serializable]
    public class Rotate : TweenAnimationParameters
    {
        protected override Tweener GetTween(RectTransform rectTransform)
        {
            return rectTransform.DOLocalRotate(targetValue, duration);
        }
    }

    [Serializable]
    public class Scale : TweenAnimationParameters
    {
        protected override Tweener GetTween(RectTransform rectTransform)
        {
            return rectTransform.DOScale(targetValue, duration);
        }
    }

    [Serializable]
    public class Size : TweenAnimationParameters
    {
        protected override Tweener GetTween(RectTransform rectTransform)
        {
            return rectTransform.DOSizeDelta(targetValue, duration);
        }
    }
}