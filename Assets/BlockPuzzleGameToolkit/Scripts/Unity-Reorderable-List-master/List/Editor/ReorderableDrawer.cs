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
using BlockPuzzleGameToolkit.Scripts.Unity_Reorderable_List_master.List.Attributes;
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Unity_Reorderable_List_master.List.Editor
{
    [CustomPropertyDrawer(typeof(ReorderableAttribute))]
    public class ReorderableDrawer : PropertyDrawer
    {
        public const string ARRAY_PROPERTY_NAME = "array";

        private static readonly Dictionary<int, ReorderableList> lists = new();

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var list = GetList(property, attribute as ReorderableAttribute, ARRAY_PROPERTY_NAME);

            return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var list = GetList(property, attribute as ReorderableAttribute, ARRAY_PROPERTY_NAME);

            if (list != null)
            {
                list.DoList(EditorGUI.IndentedRect(position), label);
            }
            else
            {
                GUI.Label(position, "Array must extend from ReorderableArray", EditorStyles.label);
            }
        }

        public static int GetListId(SerializedProperty property)
        {
            if (property != null)
            {
                var h1 = property.serializedObject.targetObject.GetHashCode();
                var h2 = property.propertyPath.GetHashCode();

                return ((h1 << 5) + h1) ^ h2;
            }

            return 0;
        }

        public static ReorderableList GetList(SerializedProperty property, string arrayPropertyName)
        {
            return GetList(property, null, GetListId(property), arrayPropertyName);
        }

        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, string arrayPropertyName)
        {
            return GetList(property, attrib, GetListId(property), arrayPropertyName);
        }

        public static ReorderableList GetList(SerializedProperty property, int id, string arrayPropertyName)
        {
            return GetList(property, null, id, arrayPropertyName);
        }

        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, int id, string arrayPropertyName)
        {
            if (property == null)
            {
                return null;
            }

            ReorderableList list = null;
            var array = property.FindPropertyRelative(arrayPropertyName);

            if (array != null && array.isArray)
            {
                if (!lists.TryGetValue(id, out list))
                {
                    if (attrib != null)
                    {
                        var icon = !string.IsNullOrEmpty(attrib.elementIconPath) ? AssetDatabase.GetCachedIcon(attrib.elementIconPath) : null;

                        var displayType = attrib.singleLine ? ReorderableList.ElementDisplayType.SingleLine : ReorderableList.ElementDisplayType.Auto;

                        list = new ReorderableList(array, attrib.add, attrib.remove, attrib.draggable, displayType, attrib.elementNameProperty, attrib.elementNameOverride, icon);
                        list.paginate = attrib.paginate;
                        list.pageSize = attrib.pageSize;
                        list.sortable = attrib.sortable;

                        //handle surrogate if any

                        if (attrib.surrogateType != null)
                        {
                            var callback = new SurrogateCallback(attrib.surrogateProperty);

                            list.surrogate = new ReorderableList.Surrogate(attrib.surrogateType, callback.SetReference);
                        }
                    }
                    else
                    {
                        list = new ReorderableList(array, true, true, true);
                    }

                    lists.Add(id, list);
                }
                else
                {
                    list.List = array;
                }
            }

            return list;
        }

        private struct SurrogateCallback
        {
            private readonly string property;

            internal SurrogateCallback(string property)
            {
                this.property = property;
            }

            internal void SetReference(SerializedProperty element, Object objectReference, ReorderableList list)
            {
                var prop = !string.IsNullOrEmpty(property) ? element.FindPropertyRelative(property) : null;

                if (prop != null && prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    prop.objectReferenceValue = objectReference;
                }
            }
        }
    }
}