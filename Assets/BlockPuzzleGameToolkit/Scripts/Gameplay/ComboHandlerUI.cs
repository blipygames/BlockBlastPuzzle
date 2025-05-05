using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Enums;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class ComboHandlerUI : MonoBehaviour
    {
        [SerializeField] private Image rhombusImage;
        [SerializeField] private float pulseDuration = 0.5f;
        [SerializeField] private ParallelOptions pulseOptions;
        [SerializeField] private ParticleSystem lineDestroyParticles;
        private Sequence _pulseSequence;
        private bool _isComboActive = false;
        private LevelManager _levelManager;

        private void OnEnable()
        {
            _levelManager = FindObjectOfType<LevelManager>(true);
            if (_levelManager != null)
            {
                EventManager.GetEvent<Shape>(EGameEvent.LineDestroyed).Subscribe(OnLineDestroyed);
            }
            
            // Hide rhombus initially
            if (rhombusImage != null)
            {
                rhombusImage.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (_levelManager != null)
            {
                EventManager.GetEvent<Shape>(EGameEvent.LineDestroyed).Unsubscribe(OnLineDestroyed);
            }
            StopPulseAnimation();
        }

        private void OnLineDestroyed(Shape shape)
        {
            lineDestroyParticles.Play();

            if (_levelManager.comboCounter >= 1)
            {
                if (rhombusImage != null && !rhombusImage.gameObject.activeSelf)
                {
                    rhombusImage.gameObject.SetActive(true);
                }
                StartContinuousPulsing(_levelManager.comboCounter);
            }
        }

        public void StartContinuousPulsing(int comboCounter)
        {
            if (_isComboActive && _pulseSequence != null && _pulseSequence.IsPlaying())
            {
                return;
            }
            
            StopPulseAnimation();
            _isComboActive = true;
            
            _pulseSequence = DOTween.Sequence();
            
            float scaleFactor = 1.0f + Mathf.Min(comboCounter * 0.1f, 0.2f);
            
            if (rhombusImage != null)
            {
                Vector3 rhombusOriginalScale = rhombusImage.transform.localScale;
                Color rhombusOriginalColor = rhombusImage.color;
                
                _pulseSequence.Append(rhombusImage.transform.DOScale(rhombusOriginalScale * scaleFactor, pulseDuration).SetEase(Ease.OutQuad));
                _pulseSequence.Append(rhombusImage.transform.DOScale(rhombusOriginalScale, pulseDuration).SetEase(Ease.InQuad));
                _pulseSequence.Join(rhombusImage.DOColor(rhombusOriginalColor, pulseDuration));
            }
            
            _pulseSequence.SetLoops(-1, LoopType.Restart);
            _pulseSequence.Play();
            
            StartCoroutine(CheckComboStatus());
        }
        
        private IEnumerator CheckComboStatus()
        {
            while (_isComboActive)
            {
                yield return new WaitForSeconds(0.5f);
                
                if (_levelManager.comboCounter < 1)
                {
                    StopPulseAnimation();
                    break;
                }
            }
        }
        
        private void StopPulseAnimation()
        {
            if (_pulseSequence != null)
            {
                _pulseSequence.Kill();
                _pulseSequence = null;
                
                if (rhombusImage != null)
                {
                    rhombusImage.transform.localScale = Vector3.one;
                    rhombusImage.color = Color.white;
                    // Hide rhombus when combo ends
                    rhombusImage.gameObject.SetActive(false);
                }
            }
            
            _isComboActive = false;
        }
    }
}