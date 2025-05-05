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
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay.Pool
{
    public class PoolObject : MonoBehaviour
    {
        protected static readonly Dictionary<string, PoolObject> pools = new();

        public GameObject prefab;

        protected Queue<GameObject> pool = new();

        public virtual void Awake()
        {
            if (prefab != null)
            {
                SetPrefab(prefab);
            }
        }

        public void SetPrefab(GameObject newPrefab)
        {
            pools[newPrefab.name] = this;
            prefab = newPrefab;
        }

        private void OnDestroy()
        {
            pools.Clear();
        }

        protected GameObject Create()
        {
            var item = Instantiate(prefab, transform);
            item.name = prefab.name;
            return item;
        }

        private GameObject Get()
        {
            if (!GameManager.instance.GameSettings.enablePool)
            {
                return Create();
            }

            var item = pool.Count == 0 ? Create() : pool.Dequeue();
            if (item.activeSelf && pool.Count > 0)
            {
                item = pool.Dequeue();
            }

            item.SetActive(true);
            return item;
        }

        public static void Return(GameObject item)
        {
            if (item == null)
            {
                return;
            }

            if (!GameManager.instance?.GameSettings.enablePool ?? true)
            {
                Destroy(item);
                return;
            }

            item.SetActive(false);
            if (pools.TryGetValue(item.name, out var pool))
            {
                pool.pool.Enqueue(item);
            }
        }

        public static GameObject GetObject(GameObject prefab)
        {
            return GetPool(prefab);
        }

        public static GameObject GetObject(GameObject prefab, Vector3 position)
        {
            var item = GetPool(prefab);
            item.transform.position = position;
            return item;
        }

        private static GameObject GetPool(GameObject prefab)
        {
            var prefabName = prefab.name;
            if (pools.TryGetValue(prefabName, out var pool))
            {
                return pool.Get();
            }

            var poolObject = new GameObject(prefabName).AddComponent<PoolObject>();
            poolObject.transform.SetParent(GameObject.Find("FXPool").transform);
            poolObject.prefab = prefab;
            poolObject.transform.localScale = Vector3.one;
            pools.Add(prefabName, poolObject);
            return poolObject.Get();
        }

        public static GameObject GetObject(string prefabName)
        {
            if (pools.TryGetValue(prefabName, out var pool))
            {
                return pool.Get();
            }

            Debug.LogError($"Pool with name {prefabName} not found");
            return null;
        }
    }
}