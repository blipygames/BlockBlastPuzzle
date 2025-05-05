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
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Unity_Reorderable_List_master.List.Attributes
{
    public class ReorderableAttribute : PropertyAttribute
    {
        public bool add;
        public bool remove;
        public bool draggable;
        public bool singleLine;
        public bool paginate;
        public bool sortable;
        public int pageSize;
        public string elementNameProperty;
        public string elementNameOverride;
        public string elementIconPath;
        public Type surrogateType;
        public string surrogateProperty;

        public ReorderableAttribute()
            : this(null)
        {
        }

        public ReorderableAttribute(string elementNameProperty)
            : this(true, true, true, elementNameProperty, null, null)
        {
        }

        public ReorderableAttribute(string elementNameProperty, string elementIconPath)
            : this(true, true, true, elementNameProperty, null, elementIconPath)
        {
        }

        public ReorderableAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
            : this(true, true, true, elementNameProperty, elementNameOverride, elementIconPath)
        {
        }

        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementIconPath = null)
            : this(add, remove, draggable, elementNameProperty, null, elementIconPath)
        {
        }

        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementNameOverride = null, string elementIconPath = null)
        {
            this.add = add;
            this.remove = remove;
            this.draggable = draggable;
            this.elementNameProperty = elementNameProperty;
            this.elementNameOverride = elementNameOverride;
            this.elementIconPath = elementIconPath;

            sortable = true;
        }
    }
}