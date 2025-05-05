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
using System.Collections.Generic;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Unity_Reorderable_List_master.List
{
    [Serializable]
    public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>
    {
        [SerializeField]
        private List<T> array = new();

        public ReorderableArray()
            : this(0)
        {
        }

        public ReorderableArray(int length)
        {
            array = new List<T>(length);
        }

        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public int Length => array.Count;

        public bool IsReadOnly => false;

        public int Count => array.Count;

        public object Clone()
        {
            return new List<T>(array);
        }

        public void CopyFrom(IEnumerable<T> value)
        {
            array.Clear();
            array.AddRange(value);
        }

        public bool Contains(T value)
        {
            return array.Contains(value);
        }

        public int IndexOf(T value)
        {
            return array.IndexOf(value);
        }

        public void Insert(int index, T item)
        {
            array.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            array.RemoveAt(index);
        }

        public void Add(T item)
        {
            array.Add(item);
        }

        public void Clear()
        {
            array.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.array.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return array.Remove(item);
        }

        public T[] ToArray()
        {
            return array.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }
}