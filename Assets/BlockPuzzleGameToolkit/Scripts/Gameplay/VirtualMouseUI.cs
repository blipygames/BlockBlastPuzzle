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
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class VirtualMouse : MonoBehaviour
    {
        private VirtualMouseInput virtualMouseInput;
        private bool isInitialized = false;

        [SerializeField]
        private Canvas canvas;
        
        private RectTransform canvasRectTransform;
        private Camera uiCamera;
        
        [SerializeField]
        private float cursorHideDelay = 1.0f; // Time in seconds before hiding cursor
        [SerializeField]
        private float fadeSpeed = 3.0f; // How fast the cursor fades in/out
        [SerializeField]
        private float minCursorAlpha = 0f; // Minimum alpha when hidden
        [SerializeField]
        private float maxCursorAlpha = .5f; // Maximum alpha when visible

        private float idleTimer = 0f;
        private Vector2 lastPosition;
        public Image cursorImage; // Reference to cursor image
        private bool isCursorVisible = true;
        private float targetAlpha;
        private float currentAlpha;
        
        private void Awake()
        {
            virtualMouseInput = GetComponent<VirtualMouseInput>();
            if (virtualMouseInput == null)
            {
                Debug.LogError("VirtualMouseInput component not found! Please add it to the same GameObject.");
                return;
            }
            
            canvasRectTransform = canvas.GetComponent<RectTransform>();
            
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                uiCamera = canvas.worldCamera;
            }
            
            // Initialize cursor as hidden
            targetAlpha = minCursorAlpha;
            currentAlpha = minCursorAlpha;
            
            // Set initial cursor alpha
            if (cursorImage != null)
            {
                Color cursorColor = cursorImage.color;
                cursorColor.a = minCursorAlpha;
                cursorImage.color = cursorColor;
            }
        }
        
        private void Start()
        {
            StartCoroutine(WaitForVirtualMouseInitialization());
        }
        
        private IEnumerator WaitForVirtualMouseInitialization()
        {
            float timeout = 2.0f;
            float timer = 0f;
            
            while (timer < timeout)
            {
                if (virtualMouseInput != null && virtualMouseInput.virtualMouse != null)
                {
                    lastPosition = virtualMouseInput.virtualMouse.position.value;
                    isInitialized = true;
                    Debug.Log("Virtual mouse initialized successfully.");
                    yield break;
                }
                
                timer += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.LogError("Failed to initialize virtualMouse within timeout period.");
        }
        
        private void Update()
        {
            if (!isInitialized || virtualMouseInput == null || virtualMouseInput.virtualMouse == null)
            {
                return; // Skip if not initialized
            }
            
            transform.SetAsLastSibling();
            
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                transform.localScale = new Vector3(1f / canvas.scaleFactor, 1f / canvas.scaleFactor, 1f);
            }
            
            // Check if cursor moved - with null checks
            try
            {
                Vector2 currentPosition = virtualMouseInput.virtualMouse.position.value;
                if (Vector2.Distance(currentPosition, lastPosition) > 0.5f) // Small threshold to detect real movement
                {
                    idleTimer = 0f;
                    targetAlpha = maxCursorAlpha; // Set target to maximum alpha rather than 1.0
                    lastPosition = currentPosition;
                }
                else
                {
                    // No movement, increment timer
                    idleTimer += Time.deltaTime;
                    
                    // Start fading cursor after delay
                    if (idleTimer >= cursorHideDelay)
                    {
                        targetAlpha = minCursorAlpha; // Set target to minimum alpha
                    }
                }
                
                // Smoothly fade the cursor
                if (cursorImage != null)
                {
                    // Gradually interpolate current alpha toward target alpha
                    currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
                    
                    // Apply the alpha to the cursor image
                    Color cursorColor = cursorImage.color;
                    cursorColor.a = currentAlpha;
                    cursorImage.color = cursorColor;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error accessing virtual mouse position: " + e.Message);
            }
        }
        
        private void LateUpdate()
        {
            if (!isInitialized || virtualMouseInput == null || virtualMouseInput.virtualMouse == null)
            {
                return; // Skip if not initialized
            }
            
            try
            {
                // Get the current virtual mouse position
                var virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
                
                // Clamp to screen boundaries
                virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0, Screen.width);
                virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0, Screen.height);
                
                if (canvas != null)
                {
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    {
                        float scaleFactor = canvas.scaleFactor;
                        Rect canvasRect = canvasRectTransform.rect;
                        
                        float canvasWidth = canvasRect.width * scaleFactor;
                        float canvasHeight = canvasRect.height * scaleFactor;
                        
                        float xOffset = (Screen.width - canvasWidth) / 2;
                        float yOffset = (Screen.height - canvasHeight) / 2;
                        
                        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, xOffset, xOffset + canvasWidth);
                        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, yOffset, yOffset + canvasHeight);
                    }
                    else if (canvas.renderMode == RenderMode.ScreenSpaceCamera && uiCamera != null)
                    {
                        Vector3[] corners = new Vector3[4];
                        canvasRectTransform.GetWorldCorners(corners);
                        
                        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
                        Vector2 max = new Vector2(float.MinValue, float.MinValue);
                        
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]);
                            min.x = Mathf.Min(min.x, screenPos.x);
                            min.y = Mathf.Min(min.y, screenPos.y);
                            max.x = Mathf.Max(max.x, screenPos.x);
                            max.y = Mathf.Max(max.y, screenPos.y);
                        }
                        
                        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, min.x, max.x);
                        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, min.y, max.y);
                    }
                }
                
                InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error in virtual mouse LateUpdate: " + e.Message);
            }
        }
    }
}