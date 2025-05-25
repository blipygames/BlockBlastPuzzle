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
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Audio;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.System.Haptic;
using BlockPuzzleGameToolkit.Scripts.Gameplay.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class ShapeDraggable : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector2 originalPosition;
        private Vector2 touchOffset;
        private readonly float verticalOffset = 300;
        private Vector3 originalScale;
        private bool isDragging;
        private int activeTouchId = -1;
        private Canvas canvas;
        private Camera eventCamera;

        private Shape shape;
        private List<Item> _items = new();
        private HighlightManager highlightManager;
        private FieldManager field;
        private ItemFactory itemFactory;
        private VirtualMouseInput virtualMouseInput;
        private bool wasVirtualMousePressed = false;
        private TimerManager timerManager;

        private void OnEnable()
        {
            itemFactory = FindObjectOfType<ItemFactory>();
            rectTransform = GetComponent<RectTransform>();
            shape = GetComponent<Shape>();
            shape.OnShapeUpdated += UpdateItems;
            UpdateItems();
            highlightManager = FindObjectOfType<HighlightManager>();
            field = FindObjectOfType<FieldManager>();
            timerManager = FindObjectOfType<TimerManager>();

            // Get canvas and camera reference
            canvas = GetComponentInParent<Canvas>();
            eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            
            // Find virtual mouse if available
            virtualMouseInput = FindObjectOfType<VirtualMouseInput>();

            // Subscribe to events that can cancel dragging
            EventManager.GetEvent(EGameEvent.TimerExpired).Subscribe(CancelDragIfActive);
            EventManager.GetEvent(EGameEvent.LevelAboutToComplete).Subscribe(CancelDragIfActive);
            EventManager.GetEvent(EGameEvent.TutorialCompleted).Subscribe(CancelDragIfActive);
        }

        private void OnDisable()
        {
            shape.OnShapeUpdated -= UpdateItems;
            EventManager.GetEvent(EGameEvent.TimerExpired).Unsubscribe(CancelDragIfActive);
            EventManager.GetEvent(EGameEvent.LevelAboutToComplete).Unsubscribe(CancelDragIfActive);
            EventManager.GetEvent(EGameEvent.TutorialCompleted).Unsubscribe(CancelDragIfActive);
            EndDrag();
        }

        private void CancelDragIfActive()
        {
            if (isDragging)
            {
                CancelDragWithReturn();
            }
        }

        private void Update()
        {
            if (EventManager.GameStatus != EGameState.Playing && EventManager.GameStatus != EGameState.Tutorial)
            {
                return;
            }
            
            // Handle touch input with new Input System
            if (Touchscreen.current != null)
            {
                // Handle existing active touch
                if (isDragging && activeTouchId != -1)
                {
                    bool foundActiveTouch = false;
                    for (int i = 0; i < Touchscreen.current.touches.Count; i++)
                    {
                        var touch = Touchscreen.current.touches[i];
                        if (touch.touchId.ReadValue() == activeTouchId)
                        {
                            HandleDrag(touch.position.ReadValue());
                            foundActiveTouch = true;
                    
                            // Check if touch has ended
                            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended || 
                                touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled)
                            {
                                EndDrag();
                            }
                            break;
                        }
                    }
            
                    if (!foundActiveTouch)
                    {
                        EndDrag();
                    }
                }
                // Check for new touches if not already dragging
                else if (!isDragging)
                {
                    for (int i = 0; i < Touchscreen.current.touches.Count; i++)
                    {
                        var touch = Touchscreen.current.touches[i];
                        if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                        {
                            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, touch.position.ReadValue(), eventCamera))
                            {
                                activeTouchId = touch.touchId.ReadValue();
                                BeginDrag(touch.position.ReadValue());
                                break;
                            }
                        }
                    }
                }
            }
            
            // Track virtual mouse state if available - works on ALL platforms
            bool virtualMouseHandled = false;
            bool isVirtualMousePressed = false;
            Vector2 virtualMousePosition = Vector2.zero;
            
            if (virtualMouseInput != null && virtualMouseInput.virtualMouse != null)
            {
                isVirtualMousePressed = virtualMouseInput.virtualMouse.leftButton.isPressed;
                virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
                
                // Handle virtual mouse input
                if (activeTouchId == -1)
                {
                    // Virtual mouse button down this frame
                    if (isVirtualMousePressed && !wasVirtualMousePressed && !isDragging)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, virtualMousePosition, eventCamera))
                        {
                            BeginDrag(virtualMousePosition);
                            virtualMouseHandled = true;
                        }
                    }
                    // Continue dragging with virtual mouse
                    else if (isVirtualMousePressed && isDragging)
                    {
                        HandleDrag(virtualMousePosition);
                        virtualMouseHandled = true;
                    }
                    // Release with virtual mouse
                    else if (!isVirtualMousePressed && wasVirtualMousePressed && isDragging)
                    {
                        EndDrag();
                        virtualMouseHandled = true;
                    }
                }
                
                wasVirtualMousePressed = isVirtualMousePressed;
            }
            
            // Handle regular mouse input using the new Input System if not already handled
            if (!virtualMouseHandled && activeTouchId == -1)
            {
                if (Mouse.current != null)
                {
                    if (Mouse.current.leftButton.wasPressedThisFrame && !isDragging)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Mouse.current.position.ReadValue(), eventCamera))
                        {
                            BeginDrag(Mouse.current.position.ReadValue());
                        }
                    }
                    else if (Mouse.current.leftButton.isPressed && isDragging)
                    {
                        HandleDrag(Mouse.current.position.ReadValue());
                    }
                    else if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
                    {
                        EndDrag();
                    }
                }
            }

            // Additional safety check to ensure EndDrag is called if dragging unexpectedly stops
            if (isDragging && activeTouchId == -1 && 
                (Mouse.current == null || !Mouse.current.leftButton.isPressed) &&
                !(virtualMouseInput != null && virtualMouseInput.virtualMouse != null && 
                  virtualMouseInput.virtualMouse.leftButton.isPressed))
            {
                EndDrag();
            }
        }

        private void UpdateItems()
        {
            _items = shape.GetActiveItems();
        }

        private void BeginDrag(Vector2 position)
        {
            isDragging = true;
            originalPosition = rectTransform.anchoredPosition;
            originalScale = transform.localScale;

            transform.SetAsLastSibling();
            transform.localScale = Vector3.one;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, position, eventCamera, out touchOffset);
        }

        private void CancelDragWithReturn()
        {
            rectTransform.anchoredPosition = originalPosition;
            transform.localScale = originalScale;
            highlightManager.ClearAllHighlights();
            highlightManager.OnDragEndedWithoutPlacement();
            isDragging = false;
        }

        private void HandleDrag(Vector2 position)
        {
            if (!isDragging)
            {
                return;
            }

            var cellSize = field.GetCellSize();
            var shapeOriginalWidth = 102f;
            var scaleFactor = cellSize / shapeOriginalWidth;

            transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform.parent as RectTransform, position, eventCamera, out var localPoint))
            {
                var canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
                var normalizedX = localPoint.x / canvasWidth;
                var scaleFactorY = rectTransform.rect.height / canvas.GetComponent<RectTransform>().rect.height * 2.5f;

                rectTransform.anchoredPosition = new Vector2(
                    normalizedX * canvasWidth,
                    localPoint.y / scaleFactorY + verticalOffset + scaleFactorY
                );
            }

            if (AnyBusyCellsOrNoneCells())
            {
                if (IsDistancesToHighlightedCellsTooHigh())
                {
                    highlightManager.ClearAllHighlights();
                }

                return;
            }

            UpdateCellHighlights();
        }

        private void EndDrag()
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            activeTouchId = -1;

            if (highlightManager.GetHighlightedCells().Count == 0)
            {
                rectTransform.anchoredPosition = originalPosition;
                transform.localScale = originalScale;
                highlightManager.ClearAllHighlights();
                highlightManager.OnDragEndedWithoutPlacement();
                return;
            }

            HapticFeedback.TriggerHapticFeedback(HapticFeedback.HapticForce.Light);
            SoundBase.instance.PlaySound(SoundBase.instance.placeShape);

            foreach (var kvp in highlightManager.GetHighlightedCells())
            {
                kvp.Key.FillCell(kvp.Value.itemTemplate);
                kvp.Key.AnimateFill();
                if (kvp.Value.bonusItemTemplate != null)
                {
                    kvp.Key.SetBonus(kvp.Value.bonusItemTemplate);
                }
            }

            EventManager.GetEvent<Shape>(EGameEvent.ShapePlaced).Invoke(shape);
        }

        private bool IsDistancesToHighlightedCellsTooHigh()
        {
            var firstOrDefault = highlightManager.GetHighlightedCells().FirstOrDefault();
            return firstOrDefault.Key != null &&
                   Vector3.Distance(_items[0].transform.position, firstOrDefault.Key.transform.position) > 1f;
        }

        private bool AnyBusyCellsOrNoneCells()
        {
            return _items.Any(item =>
            {
                var cell = GetCellUnderShape(item);
                var cellComponent = cell?.GetComponent<Cell>();
                return cellComponent == null || !cellComponent.IsEmpty() || cellComponent.IsDestroying();
            });
        }

        private void UpdateCellHighlights()
        {
            highlightManager.ClearAllHighlights();

            foreach (var item in _items)
            {
                var cell = GetCellUnderShape(item);
                if (cell != null)
                {
                    highlightManager.HighlightCell(cell, item);
                }
            }

            if (itemFactory._oneColorMode)
            {
                highlightManager.HighlightFill(field.GetFilledLines(true), itemFactory.GetColor());
            }
            else
            {
                highlightManager.HighlightFill(field.GetFilledLines(true), _items[0].itemTemplate);
            }
        }

        private Transform GetCellUnderShape(Item item)
        {
            var hit = Physics2D.Raycast(item.transform.position, Vector2.zero, 1);

            return hit.collider != null && hit.collider.CompareTag("Cell") ? hit.collider.transform : null;
        }
    }
}