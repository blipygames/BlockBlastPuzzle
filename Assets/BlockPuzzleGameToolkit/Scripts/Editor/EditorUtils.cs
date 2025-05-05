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
using System.Reflection;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    public static class EditorUtils
    {
        public static Texture2D GetPrefabPreview(GameObject prefab)
        {
            var previewRender = new PreviewRenderUtility();
            previewRender.camera.backgroundColor = Color.black;
            previewRender.camera.clearFlags = CameraClearFlags.SolidColor;
            previewRender.camera.cameraType = CameraType.Game;
            previewRender.camera.farClipPlane = 1000f;
            previewRender.camera.nearClipPlane = 0.1f;

            var obj = previewRender.InstantiatePrefabInScene(prefab);
            var rect = obj.GetComponent<RectTransform>().rect;
            previewRender.BeginStaticPreview(new Rect(0.0f, 0.0f, rect.width*1.5f, rect.height*1.5f));
            
            SetupPreviewCanvas(obj, previewRender.camera);
            
            previewRender.Render();
            var texture = previewRender.EndStaticPreview();
            
            previewRender.camera.targetTexture = null;
            previewRender.Cleanup();
            return texture;
        }

        private static void SetupPreviewCanvas(GameObject obj, Camera camera)
        {
            var canvasInstance = obj.AddComponent<Canvas>();
            canvasInstance.renderMode = RenderMode.ScreenSpaceCamera;
            canvasInstance.worldCamera = camera;

            var canvasScaler = obj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            var scaleFactorX = Screen.width / canvasScaler.referenceResolution.x;
            var scaleFactorY = Screen.height / canvasScaler.referenceResolution.y;
            canvasScaler.scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY) * 7;
        }

        public static Texture2D GetCanvasPreviewVisualElement<T>(T prefab, Action<T> action) where T : FillAndPreview
        {
            var previewRender = new PreviewRenderUtility();
            previewRender.camera.backgroundColor = Color.black;
            previewRender.camera.clearFlags = CameraClearFlags.SolidColor;
            previewRender.camera.cameraType = CameraType.Game;
            previewRender.camera.farClipPlane = 1000f;
            previewRender.camera.nearClipPlane = 0.1f;

            var obj = previewRender.InstantiatePrefabInScene(prefab.gameObject);
            action.Invoke(obj.GetComponent<T>());
            var rect = obj.GetComponent<RectTransform>().rect;
            previewRender.BeginStaticPreview(new Rect(0.0f, 0.0f, rect.width, rect.height));
            
            SetupPreviewCanvas(obj, previewRender.camera);
            
            previewRender.Render();
            var texture = previewRender.EndStaticPreview();
            
            previewRender.camera.targetTexture = null;
            previewRender.Cleanup();
            return texture;
        }

        public static SerializedProperty GetPropertyFromValue(Object targetObject)
        {
            var serializedObject = new SerializedObject(targetObject);
            var property = serializedObject.GetIterator();

            // Go through each property in the object
            while (property.Next(true))
            {
                // Skip properties with child properties (e.g., arrays, structs)
                if (property.hasVisibleChildren)
                {
                    continue;
                }

                // Check if the property value matches the desired field value
                // if (fieldValue.Equals(GetFieldValue(targetObject, property.name)))
                {
                    // Create a copy of the property
                    var copiedProperty = property.Copy();
                    // Make sure the serializedObject is up to date
                    copiedProperty.serializedObject.Update();
                    // Apply the modified properties
                    copiedProperty.serializedObject.ApplyModifiedProperties();
                    return copiedProperty;
                }
            }

            return null; // Field value not found
        }

        private static object GetFieldValue(Object targetObject, string fieldName)
        {
            var field = targetObject.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                return field.GetValue(targetObject);
            }

            Debug.LogError($"Field {fieldName} not found in object {targetObject.GetType().Name}");
            return null;
        }

        public static VisualElement GetObjectFields(SerializedObject serializedObject, Action<SerializedProperty> onChange = null)
        {
            var visualElement = new VisualElement();
            // Iterate through the fields of the Icon class
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                // Exclude the "m_Script" field
                if (iterator.name == "m_Script")
                {
                    continue;
                }

                // Create a PropertyField for each field
                var propertyField = new PropertyField(iterator.Copy());
                propertyField.Bind(serializedObject);
                propertyField.style.flexShrink = 0;
                propertyField.style.flexGrow = 0;
                propertyField.style.width = 400;
                propertyField.RegisterValueChangeCallback(evt => { onChange?.Invoke(evt.changedProperty); });

                visualElement.Add(propertyField);
                enterChildren = false;
            }

            return visualElement;
        }

        public static VisualElement GetPropertyFields(SerializedProperty property, bool children, Action<SerializedProperty> onChange = null)
        {
            var visualElement = new VisualElement();
            var methods = TypeCache.GetMethodsWithAttribute<CustomSerializeTypePropertyAttribute>();
            foreach (var m in methods)
            {
                foreach (var customAttributeData in m.CustomAttributes)
                {
                    foreach (var customAttributeTypedArgument in customAttributeData.ConstructorArguments)
                    {
                        if (property.managedReferenceValue != null && (Type)customAttributeTypedArgument.Value != property.managedReferenceValue.GetType())
                        {
                            continue;
                        }

                        if (m.IsStatic)
                        {
                            return m.Invoke(null, new object[] { property }) as VisualElement;
                        }

                        var instance = Activator.CreateInstance(m.DeclaringType);
                        return m.Invoke(instance, new object[] { property }) as VisualElement;
                    }
                }
            }

            // Iterate through the fields of the Icon class
            var iterator = property.Copy();
            while (iterator.NextVisible(children))
            {
                // Exclude the "m_Script" field
                if (iterator.name == "m_Script")
                {
                    continue;
                }

                if (iterator.depth == property.depth + 1)
                {
                    // Create a PropertyField for each field
                    var propertyField = new PropertyField(iterator.Copy());
                    propertyField.Bind(property.serializedObject);
                    propertyField.style.flexShrink = 0;
                    propertyField.style.flexGrow = 0;
                    propertyField.style.width = 400;
                    propertyField.RegisterValueChangeCallback(evt => { onChange?.Invoke(evt.changedProperty); });

                    visualElement.Add(propertyField);
                }

                children = false;
            }

            return visualElement;
        }

        public static DropdownField GetTypesDropdown(SerializedProperty property)
        {
            var fieldInfo = TypeCache.GetFieldsWithAttribute<SerializeTypePropertyAttribute>()[0];
            var typesDerivedFrom = TypeCache.GetTypesDerivedFrom(fieldInfo.FieldType);
            var typeNames = typesDerivedFrom.Select(t => t.Name).ToList();
            var typeNamesFormatted = typesDerivedFrom.Select(t => t.Name.CamelCaseSplit()).ToList();
            var parentTypeName = fieldInfo.Name.CamelCaseSplit();
            // get the index of the current type
            var currentIndex = Mathf.Max(typeNames.IndexOf(property.managedReferenceValue?.GetType().Name), 0);
            var dropdown = new DropdownField(parentTypeName, typeNamesFormatted, typeNamesFormatted[currentIndex]);
            dropdown.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                var selectedTypeIndex = typeNamesFormatted.IndexOf(evt.newValue);
                if (selectedTypeIndex >= 0 && selectedTypeIndex < typesDerivedFrom.Count)
                {
                    property.managedReferenceValue = Activator.CreateInstance(typesDerivedFrom[selectedTypeIndex]);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError($"Selected type index {selectedTypeIndex} is out of range.");
                }
            });
            return dropdown;
        }

        public static Vector2 GetAbsolutePosition(List<VisualElement> elements, VisualElement parent)
        {
            var position = Vector2.zero;
            foreach (var element in elements)
            {
                position += element.LocalToWorld(element.layout.position);
            }

            return parent.WorldToLocal(position / elements.Count);
        }
    }
}