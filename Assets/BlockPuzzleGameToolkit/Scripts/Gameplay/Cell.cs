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
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class Cell : MonoBehaviour
    {
        private static readonly Dictionary<Item, ObjectPool<Item>> CustomItemPools = new();
        public Item item;
        private CanvasGroup group;
        public bool busy;
        private ItemTemplate saveTemplate;
        private BoxCollider2D _boxCollider2D;
        private bool isDestroying;
        private Item originalItem;
        private Item customItem;

        public Image image;

        private bool isEmpty => !busy;
        private bool IsEmptyPreview => group.alpha == 0;

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            group = item.GetComponent<CanvasGroup>();
        }

        private ObjectPool<Item> GetOrCreatePool(Item prefab)
        {
            if (!CustomItemPools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool<Item>(
                    createFunc: () => Instantiate(prefab),
                    actionOnGet: item => item.gameObject.SetActive(true),
                    actionOnRelease: item => item.gameObject.SetActive(false),
                    actionOnDestroy: item => Destroy(item.gameObject)
                );
                CustomItemPools[prefab] = pool;
            }
            return pool;
        }

        private void ReplaceWithCustomItem(ItemTemplate itemTemplate)
        {
            if (originalItem == null)
            {
                originalItem = item;
                originalItem.gameObject.SetActive(false);
            }

            if (customItem != null)
            {
                GetOrCreatePool(itemTemplate.customItemPrefab).Release(customItem);
            }
            
            customItem = GetOrCreatePool(itemTemplate.customItemPrefab).Get();
            customItem.transform.SetParent(transform);
            customItem.transform.position = originalItem.transform.position;
            customItem.transform.localScale = originalItem.transform.localScale;
            var rectTransform = customItem.GetComponent<RectTransform>();
            var originalRect = originalItem.GetComponent<RectTransform>();
            if (rectTransform != null && originalRect != null)
            {
                rectTransform.sizeDelta = originalRect.sizeDelta;
                rectTransform.anchoredPosition = originalRect.anchoredPosition;
            }
            item = customItem;
            group = item.GetComponent<CanvasGroup>();
        }

        public void FillCell(ItemTemplate itemTemplate)
        {
            if (itemTemplate.customItemPrefab != null)
            {
                ReplaceWithCustomItem(itemTemplate);
            }
            else 
            {
                if (originalItem != null)
                {
                    if (customItem != null)
                    {
                        Destroy(customItem.gameObject);
                        customItem = null;
                    }
                    item = originalItem;
                    item.gameObject.SetActive(true);
                    originalItem = null;
                    group = item.GetComponent<CanvasGroup>();
                }
            }

            item.FillIcon(itemTemplate);
            group.alpha = 1;
            busy = true;
        }

        public void FillCellFailed(ItemTemplate itemTemplate)
        {
            item.FillIcon(itemTemplate);
            group.alpha = 1;
        }

        public bool IsEmpty(bool preview = false)
        {
            return preview ? IsEmptyPreview || isDestroying: isEmpty;
        }

        public void ClearCell()
        {
            if (customItem != null)
            {
                GetOrCreatePool(customItem.GetComponent<Item>()).Release(customItem);
                customItem = null;
            }
            if (originalItem != null)
            {
                item = originalItem;
                item.gameObject.SetActive(true);
                originalItem = null;
                group = item.GetComponent<CanvasGroup>();
            }
            
            item.transform.localScale = Vector3.one;
            if (saveTemplate == null && !busy)
            {
                group.alpha = 0;
                busy = false;
            }
            else if (saveTemplate != null && busy)
            {
                FillCell(saveTemplate);
                saveTemplate = null;
            }
        }

        public void HighlightCell(ItemTemplate itemTemplate)
        {
            if (itemTemplate.customItemPrefab != null)
            {
                ReplaceWithCustomItem(itemTemplate);
            }
            else 
            {
                if (originalItem != null)
                {
                    if (customItem != null)
                    {
                        Destroy(customItem.gameObject);
                        customItem = null;
                    }
                    item = originalItem;
                    item.gameObject.SetActive(true);
                    originalItem = null;
                    group = item.GetComponent<CanvasGroup>();
                }
            }

            item.FillIcon(itemTemplate);
            group.alpha = 0.05f; // Make it semi-transparent to indicate it's a highlight
        }

        public void HighlightCellTutorial()
        {
            image.color = new Color(43f / 255f, 59f / 255f, 120f / 255f, 1f);
        }

        public void HighlightCellFill(ItemTemplate itemTemplate)
        {
            saveTemplate = item.itemTemplate;
            if (!item.HasBonusItem())
            {
                item.FillIcon(itemTemplate);
            }

            group.alpha = 1f;
        }

        public void DestroyCell()
        {
            saveTemplate = null;
            busy = false;
            item.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
            {
                isDestroying = false;
                ClearCell();
                item.ClearBonus();
            });
        }

        public Bounds GetBounds()
        {
            return _boxCollider2D.bounds;
        }

        public void InitItem()
        {
            item.name = "Item " + name;
            StartCoroutine(UpdateItem());
        }

        private IEnumerator UpdateItem()
        {
            yield return new WaitForSeconds(0.1f);
            _boxCollider2D.size = transform.GetComponent<RectTransform>().sizeDelta;
            // item.transform.SetParent(GameObject.Find("ItemsCanvas/Items").transform);
            item.transform.position = transform.position;
        }

        public void SetBonus(BonusItemTemplate bonusItemTemplate)
        {
            item.SetBonus(bonusItemTemplate);
        }

        public bool HasBonusItem()
        {
            return item.HasBonusItem();
        }

        public BonusItemTemplate GetBonusItem()
        {
            return item.bonusItemTemplate;
        }

        public void AnimateFill()
        {
            item.transform.DOScale(Vector3.one * 0.5f, 0.1f).OnComplete(() => { item.transform.DOScale(Vector3.one, 0.1f); });
        }

        public void DisableCell()
        {
            _boxCollider2D.enabled = false;
        }

        public bool IsDisabled()
        {
            return !_boxCollider2D.enabled;
        }

        public bool IsHighlighted()
        {
            return !IsDisabled();
        }

        public void SetDestroying(bool destroying)
        {
            isDestroying = destroying;
        }

        public bool IsDestroying()
        {
            return isDestroying;
        }

        private void OnDestroy()
        {
            if (customItem != null)
            {
                GetOrCreatePool(customItem.GetComponent<Item>()).Release(customItem);
            }
        }
    }
}