#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Settings.Editor
{
    [CustomPropertyDrawer(typeof(AdSetting))]
    public class AdSettingDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create a new VisualElement
            var root = new VisualElement();

            // Create a foldout
            var foldout = new Foldout { text = property.displayName, value = false };
            root.Add(foldout);

            var nameField = new PropertyField(property.FindPropertyRelative("name"), "Name");
            var enableField = new PropertyField(property.FindPropertyRelative("enable"), "Enable");
            var testInEditorField = new PropertyField(property.FindPropertyRelative("testInEditor"), "Test In Editor");
            var platformsField = new PropertyField(property.FindPropertyRelative("platforms"), "Platforms");
            var appIdField = new PropertyField(property.FindPropertyRelative("appId"), "App ID");
            var adsHandlerField = new PropertyField(property.FindPropertyRelative("adsHandler"), "Ads Handler");
            var adElementsField = new PropertyField(property.FindPropertyRelative("adElements"), "Ad Elements");

            foldout.Add(nameField);
            foldout.Add(enableField);
            foldout.Add(testInEditorField);
            foldout.Add(platformsField);
            foldout.Add(appIdField);
            foldout.Add(adsHandlerField);
            foldout.Add(adElementsField);

            // Callback to update the state of the fields and foldout style
            void UpdateFieldsState(bool isEnabled)
            {
                nameField.SetEnabled(isEnabled);
                testInEditorField.SetEnabled(isEnabled);
                platformsField.SetEnabled(isEnabled);
                appIdField.SetEnabled(isEnabled);
                adsHandlerField.SetEnabled(isEnabled);
                adElementsField.SetEnabled(isEnabled);

                foldout.style.color = isEnabled ? Color.white : Color.grey;
            }

            // Initial state update
            UpdateFieldsState(property.FindPropertyRelative("enable").boolValue);

            // Register callback to update fields and foldout style when 'enable' changes
            enableField.RegisterValueChangeCallback(evt =>
            {
                UpdateFieldsState(evt.changedProperty.boolValue);
            });

            // Return the root VisualElement
            return root;
        }
    }
}
#endif