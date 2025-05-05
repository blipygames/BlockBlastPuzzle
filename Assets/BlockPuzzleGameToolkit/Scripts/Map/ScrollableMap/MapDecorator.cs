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
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap
{
    public class MapDecorator : MonoBehaviour
    {
        [Header("Decorative Images")]
        [SerializeField] private Sprite[] decorativeSprites;
        [SerializeField] private float decorationStartOffset = 0f;
        [SerializeField] private float decorationStepDistance = 100f;
        [SerializeField] private float leftPadding = 20f;
        [SerializeField] private float rightPadding = 20f;
        
        [SerializeField] private Transform targetContainer;
        
        private float _runtimeDecorationStartOffset;
        private float _runtimeDecorationStep;
        private float _runtimeLeftPadding;
        private float _runtimeRightPadding;
        private bool _decorationsInitialized = false;
        private Rect _entireMapBounds;
        
        private void Start()
        {
            if (targetContainer == null)
            {
                targetContainer = transform;
            }
            
            _runtimeDecorationStartOffset = decorationStartOffset;
            _runtimeDecorationStep = decorationStepDistance;
            _runtimeLeftPadding = leftPadding;
            _runtimeRightPadding = rightPadding;
            
            _decorationsInitialized = true;
        }
        
        public void Initialize(Transform container)
        {
            targetContainer = container;
            _runtimeDecorationStartOffset = decorationStartOffset;
            _runtimeDecorationStep = decorationStepDistance;
            _runtimeLeftPadding = leftPadding;
            _runtimeRightPadding = rightPadding;
            _decorationsInitialized = true;
        }

        public float DecorationStartOffset
        { 
            get => _runtimeDecorationStartOffset;
            set
            {
                if (_runtimeDecorationStartOffset != value)
                {
                    _runtimeDecorationStartOffset = value;
                    if (_decorationsInitialized)
                    {
                        PlaceDecorativeImages();
                    }
                }
            }
        }

        public float DecorationStepDistance
        { 
            get => _runtimeDecorationStep;
            set
            {
                if (_runtimeDecorationStep != value)
                {
                    _runtimeDecorationStep = Mathf.Max(10f, value);
                    if (_decorationsInitialized)
                    {
                        PlaceDecorativeImages();
                    }
                }
            }
        }
        
        public float LeftPadding
        { 
            get => _runtimeLeftPadding;
            set
            {
                if (_runtimeLeftPadding != value)
                {
                    _runtimeLeftPadding = Mathf.Max(0f, value);
                    if (_decorationsInitialized)
                    {
                        PlaceDecorativeImages();
                    }
                }
            }
        }
        
        public float RightPadding
        { 
            get => _runtimeRightPadding;
            set
            {
                if (_runtimeRightPadding != value)
                {
                    _runtimeRightPadding = Mathf.Max(0f, value);
                    if (_decorationsInitialized)
                    {
                        PlaceDecorativeImages();
                    }
                }
            }
        }
        
        public void SetMapBounds(Rect bounds)
        {
            _entireMapBounds = bounds;
        }
        
        public void PlaceDecorativeImages()
        {
            if (targetContainer == null || decorativeSprites == null || decorativeSprites.Length == 0 || _entireMapBounds.width <= 0)
                return;
                
            var existingImages = targetContainer.GetComponentsInChildren<Transform>()
                .Where(t => t.name.StartsWith("DecorativeImage_"))
                .ToList();
                
            foreach (var img in existingImages)
            {
                Destroy(img.gameObject);
            }
            
            float startOffset = _runtimeDecorationStartOffset;
            float stepDistance = _runtimeDecorationStep;
            float leftPad = _runtimeLeftPadding;
            float rightPad = _runtimeRightPadding;
            
            float containerMinY = targetContainer.position.y;
            float startY = containerMinY + startOffset;
            float leftX = _entireMapBounds.xMin + leftPad;
            float rightX = _entireMapBounds.xMax - rightPad;
            
            float currentY = startY;
            int imageIndex = 0;
            
            float mapMaxY = _entireMapBounds.yMax;
            
            while (currentY <= mapMaxY)
            {
                bool placeOnRight = (imageIndex % 2 == 0);
                float positionX = placeOnRight ? rightX : leftX;
                Vector3 imagePosition = new Vector3(positionX, currentY, 0f);
                
                int spriteIndex = imageIndex % decorativeSprites.Length;
                Sprite sprite = decorativeSprites[spriteIndex];
                
                if (sprite != null)
                {
                    GameObject imageObj = new GameObject($"DecorativeImage_{imageIndex}", typeof(RectTransform), typeof(Image));
                    imageObj.transform.SetParent(targetContainer, false);
                    imageObj.transform.position = imagePosition;
                    
                    RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    
                    Image image = imageObj.GetComponent<Image>();
                    image.sprite = sprite;
                    image.SetNativeSize();
                    
                    imageObj.AddComponent<MapObjectAppearance>();
                }
                
                currentY += stepDistance;
                imageIndex++;
            }
        }
        
        public void UpdateDecorations(float startOffset, float stepDistance, float leftPadding, float rightPadding)
        {
            _runtimeDecorationStartOffset = startOffset;
            _runtimeDecorationStep = Mathf.Max(10f, stepDistance);
            _runtimeLeftPadding = Mathf.Max(0f, leftPadding);
            _runtimeRightPadding = Mathf.Max(0f, rightPadding);
            
            if (_decorationsInitialized)
            {
                PlaceDecorativeImages();
            }
        }
        
        public void SetSprites(Sprite[] sprites)
        {
            if (sprites != null && sprites.Length > 0)
            {
                decorativeSprites = sprites;
                if (_decorationsInitialized)
                {
                    PlaceDecorativeImages();
                }
            }
        }
        
#if UNITY_EDITOR
        void OnValidate()
        {
            if (Application.isPlaying && _decorationsInitialized)
            {
                _runtimeDecorationStartOffset = decorationStartOffset;
                _runtimeDecorationStep = decorationStepDistance;
                _runtimeLeftPadding = leftPadding;
                _runtimeRightPadding = rightPadding;
                PlaceDecorativeImages();
            }
        }
#endif
    }
} 