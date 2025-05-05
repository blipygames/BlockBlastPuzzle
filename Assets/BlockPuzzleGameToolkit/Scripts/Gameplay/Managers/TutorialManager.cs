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
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay.FX;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.Settings;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay.Managers
{
    public class TutorialManager : MonoBehaviour
    {
        private const float offsethand = .5f;

        [SerializeField]
        private TutorialSettings tutorialSettings;

        [SerializeField]
        private CellDeckManager cellDeckManager;

        [SerializeField]
        private ItemFactory itemFactory;

        [SerializeField]
        private LevelManager levelManager;

        [SerializeField]
        private Transform handSprite;

        private ShapeTemplate[] tutorialShapesQueue;
        private int currentPhase;

        public Outline outline;

        private Vector3 deckPosition;
        private Vector3 centerPosition;

        public bool IsTutorialActive { get; private set; }

        private Coroutine handAnimationCoroutine;
        private bool subscribed;

        private void OnEnable()
        {
            if (GameManager.instance.IsTutorialMode())
            {
                subscribed = true;
                EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Subscribe(OnShapePlaced);
                EventManager.GetEvent<Shape>(EGameEvent.LineDestroyed).Subscribe(OnLineDestroyed);
            }
        }

        private void OnDisable()
        {
            if (!subscribed)
            {
                return;
            }

            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Unsubscribe(OnShapePlaced);
            EventManager.GetEvent<Shape>(EGameEvent.LineDestroyed).Unsubscribe(OnLineDestroyed);
        }

        public void StartTutorial()
        {
            FillCellDecks();
            StartCoroutine(DelayedBoundsCalculation());
        }

        private void FillCellDecks()
        {
            if (cellDeckManager == null || tutorialSettings == null)
            {
                return;
            }

            tutorialShapesQueue = tutorialSettings.tutorialShapes
                .Skip(currentPhase * 3).Take(3).ToArray();
            cellDeckManager.ClearCellDecks();
            cellDeckManager.FillCellDecksWithShapes(tutorialShapesQueue);
        }

        public void EndTutorial()
        {
            IsTutorialActive = false;
            GameManager.instance.SetTutorialCompleted();
            StopHandAnimation();
            outline.gameObject.SetActive(false);
            GameManager.instance.SetTutorialMode(false);
            
            // Trigger tutorial completed event before restarting level
            EventManager.GetEvent(EGameEvent.TutorialCompleted).Invoke();
            
            GameManager.instance.RestartLevel();
            gameObject.SetActive(false);
        }

        private void OnShapePlaced(Shape obj)
        {
            StopHandAnimation();
            cellDeckManager.AddShapeToFreeCell(tutorialShapesQueue[0]);
            StopHandAnimation();
        }

        private void OnLineDestroyed(Shape obj)
        {
            currentPhase++;
            StartCoroutine(DelayedNextPhase());
        }

        private IEnumerator DelayedNextPhase()
        {
            yield return new WaitForSeconds(0.5f);
            if (currentPhase <= tutorialSettings.tutorialLevels.Length - 1)
            {
                CheckPhase(currentPhase);
            }
            else
            {
                EndTutorial();
            }
        }

        public void CheckPhase(int phase)
        {
            if (phase > 0)
            {
                GameManager.instance.RestartLevel();
            }
        }

        private IEnumerator DelayedBoundsCalculation()
        {
            yield return new WaitForSeconds(0.1f);
            deckPosition = cellDeckManager.cellDecks[1].shape.GetActiveItems()[0].transform.position + Vector3.right * offsethand;
            var fieldManager = FindObjectOfType<FieldManager>();
            centerPosition = fieldManager.GetCenterCell().item.transform.position + Vector3.right * offsethand + Vector3.down * offsethand;
            StartHandAnimation();
            outline.gameObject.SetActive(true);
            var value = RectTransformUtils.GetMinMaxAndSizeForCanvas(FindObjectOfType<FieldManager>().GetTutorialLine(), transform.parent.GetComponent<Canvas>());
            value.size += new Vector2(50, 50);
            Color hexColor;
            if (ColorUtility.TryParseHtmlString("#609FFF", out hexColor))
            {
                outline.Play(value.center, value.size, hexColor);
            }
        }

        public Level GetLevelForPhase()
        {
            return tutorialSettings.tutorialLevels[currentPhase];
        }

        private void StartHandAnimation()
        {
            handSprite.gameObject.SetActive(true);
            if (handAnimationCoroutine != null)
            {
                StopCoroutine(handAnimationCoroutine);
            }

            handAnimationCoroutine = StartCoroutine(HandAnimationCoroutine());
        }

        private IEnumerator HandAnimationCoroutine()
        {
            while (true)
            {
                handSprite.position = deckPosition;
                var elapsedTime = 0f;
                var duration = 1f;

                while (elapsedTime < duration)
                {
                    handSprite.position = Vector3.Lerp(deckPosition, centerPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                handSprite.position = centerPosition;
                yield return new WaitForSeconds(0.5f); // Pause before restarting the animation
            }
        }

        private void StopHandAnimation()
        {
            if (handAnimationCoroutine != null)
            {
                StopCoroutine(handAnimationCoroutine);
                handAnimationCoroutine = null;
            }

            handSprite.gameObject.SetActive(false);
        }
    }
}