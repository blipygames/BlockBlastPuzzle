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
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    [Serializable]
    public class ShapeRow
    {
        public Item[] cells = new Item[5];
    }

    public class Shape : MonoBehaviour
    {
        public ShapeTemplate shapeTemplate;
        public ShapeRow[] row;
        public Action OnShapeUpdated;
        public Item topLeftItem;

        private readonly List<Item> activeItems = new();
        private ItemTemplate initialTemplate;
        private Sequence _sequence;
        private readonly Dictionary<Vector2Int, Item> originalItems = new();
        private readonly Dictionary<Vector2Int, Item> customItems = new();
        private static readonly Dictionary<Item, ObjectPool<Item>> CustomItemPools = new();

        private void Awake()
        {
            initialTemplate = Resources.Load<ItemTemplate>("Items/ItemTemplate 0");
            UpdateShape(shapeTemplate);
        }

        private void OnDisable()
        {
            foreach (var kvp in customItems)
            {
                if (kvp.Value != null)
                {
                    GetOrCreatePool(kvp.Value.GetComponent<Item>()).Release(kvp.Value);
                }
            }
            
            foreach (var kvp in originalItems)
            {
                var position = kvp.Key;
                var i = position.y;
                var j = position.x;
                
                row[i].cells[j] = kvp.Value;
                kvp.Value.gameObject.SetActive(true);
                
                if (activeItems.Contains(customItems[position]))
                {
                    activeItems[activeItems.IndexOf(customItems[position])] = kvp.Value;
                }
            }
            
            customItems.Clear();
            originalItems.Clear();
        }

        public void UpdateShape(ShapeTemplate shapeTemplate)
        {
            activeItems.Clear();
            for (var i = 0; i < row.Length; i++)
            {
                for (var j = 0; j < row[i].cells.Length; j++)
                {
                    var item = row[i].cells[j];
                    item.gameObject.SetActive(shapeTemplate.rows[i].cells[j]);
                    item.SetPosition(new Vector2Int(j, i));
                    item.ClearBonus();
                    if (item.gameObject.activeSelf)
                    {
                        activeItems.Add(item);
                    }
                }
            }

            var centroid = CalculateCentroid();
            transform.GetChild(0).localPosition -= centroid;

            topLeftItem = GetTopLeftItem();

            OnShapeUpdated?.Invoke();
        }

        private Item GetTopLeftItem()
        {
            for (var i = 0; i < row.Length; i++)
            {
                for (var j = 0; j < row[i].cells.Length; j++)
                {
                    var item = row[i].cells[j];
                    if (item.gameObject.activeSelf)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public void UpdateColor(ItemTemplate itemTemplate)
        {
            for (var i = 0; i < row.Length; i++)
            {
                for (var j = 0; j < row[i].cells.Length; j++)
                {
                    var item = row[i].cells[j];
                    if (!item.HasBonusItem())
                    {
                        var position = new Vector2Int(j, i);
                        if (itemTemplate.customItemPrefab != null)
                        {
                            if (!originalItems.ContainsKey(position))
                            {
                                originalItems[position] = item;
                            }
                            
                            if (customItems.TryGetValue(position, out var existingCustomItem))
                            {
                                GetOrCreatePool(existingCustomItem.GetComponent<Item>()).Release(existingCustomItem);
                            }
                            
                            var newItem = GetOrCreatePool(itemTemplate.customItemPrefab).Get();
                            newItem.transform.SetParent(item.transform.parent);
                            newItem.transform.position = item.transform.position;
                            newItem.transform.localScale = item.transform.localScale;
                            newItem.SetPosition(position);
                            newItem.gameObject.SetActive(item.gameObject.activeSelf);
                            customItems[position] = newItem;
                            row[i].cells[j] = newItem;
                            item.gameObject.SetActive(false);

                            if (activeItems.Contains(item))
                            {
                                activeItems[activeItems.IndexOf(item)] = newItem;
                            }
                        }
                        else {item.UpdateColor(itemTemplate);}
                    }
                }
            }
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

        public List<Item> GetActiveItems()
        {
            return activeItems;
        }

        public void SetBonus(BonusItemTemplate bonus, int maxValue)
        {
            maxValue = Mathf.Min(maxValue, 2);
            var bonusesAssigned = 0;
            var lastBonusIndex = -2; // Initialize to -2 to ensure the first item can be assigned a bonus
            var bonusAssigned = false;

            for (var i = 0; i < activeItems.Count; i++)
            {
                if (bonusesAssigned >= maxValue)
                {
                    break;
                }

                if (i - lastBonusIndex > 1 && Random.Range(0, 3) == 0)
                {
                    SetBonus(activeItems[i], bonus);
                    bonusesAssigned++;
                    lastBonusIndex = i;
                    bonusAssigned = true;
                }
            }

            // Ensure at least one item gets a bonus if none were assigned
            if (!bonusAssigned && activeItems.Count > 0)
            {
                SetBonus(activeItems[0], bonus);
            }
        }

        private void SetBonus(Item item, BonusItemTemplate bonus)
        {
            item.UpdateColor(initialTemplate);
            item.SetBonus(bonus);
        }

        public bool HasBonusItem()
        {
            return activeItems.Any(item => item.HasBonusItem());
        }

        public Vector3 CalculateCentroid()
        {
            var centroid = Vector3.zero;
            var items = GetActiveItems();

            foreach (var item in items)
            {
                centroid += item.transform.position;
            }

            if (items.Count > 0)
            {
                centroid /= items.Count;
            }

            return transform.InverseTransformPoint(centroid);
        }
    }
}