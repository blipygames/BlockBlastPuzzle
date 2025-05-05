#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI.Orientation.Editor
{
    [CustomEditor(typeof(OrientationTransformableObject))]
    public class OrientationTransformableObjectEditor : UnityEditor.Editor
    {
        private OrientationTransformableObject targetObj;
        private bool showLandscapePreview = true;
        private bool isRectTransform;
        private bool showLayoutHelpers = false;

        private void OnEnable()
        {
            targetObj = (OrientationTransformableObject)target;
            isRectTransform = targetObj.GetComponent<RectTransform>() != null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Configuration Tools", EditorStyles.boldLabel);

            // Display component type info
            EditorGUILayout.LabelField($"Component Type: {(isRectTransform ? "RectTransform" : "Transform")}",
                EditorStyles.boldLabel);

            // Preview buttons
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Preview Landscape"))
            {
                targetObj.PreviewLandscapeConfiguration();
                showLandscapePreview = true;
            }

            if (GUILayout.Button("Preview Portrait"))
            {
                targetObj.PreviewPortraitConfiguration();
                showLandscapePreview = false;
            }

            EditorGUILayout.EndHorizontal();

            // Capture current transform
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Set Landscape from Current"))
            {
                targetObj.UpdateLandscapeFromCurrentTransform();
                EditorUtility.SetDirty(targetObj);
            }

            if (GUILayout.Button("Set Portrait from Current"))
            {
                targetObj.UpdatePortraitFromCurrentTransform();
                EditorUtility.SetDirty(targetObj);
            }

            EditorGUILayout.EndHorizontal();

            // RectTransform specific helpers
            if (isRectTransform)
            {
                EditorGUILayout.Space(5);
                showLayoutHelpers = EditorGUILayout.Foldout(showLayoutHelpers, "RectTransform Quick Settings", true);

                if (showLayoutHelpers)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    // Current fill status
                    var rt = targetObj.GetComponent<RectTransform>();
                    bool isCurrentlyFilling =
                        rt.anchorMin == Vector2.zero &&
                        rt.anchorMax == Vector2.one &&
                        rt.sizeDelta == Vector2.zero;

                    EditorGUILayout.LabelField($"Current Status: {(isCurrentlyFilling ? "Filling Parent" : "Custom Size/Position")}",
                        EditorStyles.boldLabel);

                    // Landscape Setup
                    EditorGUILayout.LabelField("Landscape Layout", EditorStyles.boldLabel);

                    if (GUILayout.Button("Make Fill Parent (Landscape)"))
                    {
                        SetFillParent("landscapeConfig", true);
                        targetObj.PreviewLandscapeConfiguration();
                        showLandscapePreview = true;
                        EditorUtility.SetDirty(targetObj);
                    }

                    if (GUILayout.Button("Make Custom Size (Landscape)"))
                    {
                        SetFillParent("landscapeConfig", false);
                        targetObj.UpdateLandscapeFromCurrentTransform(); // Capture current
                        targetObj.PreviewLandscapeConfiguration();
                        showLandscapePreview = true;
                        EditorUtility.SetDirty(targetObj);
                    }

                    // Portrait Setup
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Portrait Layout", EditorStyles.boldLabel);

                    if (GUILayout.Button("Make Fill Parent (Portrait)"))
                    {
                        SetFillParent("portraitConfig", true);
                        targetObj.PreviewPortraitConfiguration();
                        showLandscapePreview = false;
                        EditorUtility.SetDirty(targetObj);
                    }

                    if (GUILayout.Button("Make Custom Size (Portrait)"))
                    {
                        SetFillParent("portraitConfig", false);
                        targetObj.UpdatePortraitFromCurrentTransform(); // Capture current
                        targetObj.PreviewPortraitConfiguration();
                        showLandscapePreview = false;
                        EditorUtility.SetDirty(targetObj);
                    }

                    EditorGUILayout.EndVertical();
                }
            }

            // Register with manager
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Register with Manager"))
            {
                targetObj.RegisterWithManager();
            }

            // Show current preview state
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Currently previewing: {(showLandscapePreview ? "Landscape" : "Portrait")}",
                EditorStyles.boldLabel);
        }

        private void SetFillParent(string presetFieldName, bool fillParent)
        {
            // Get the preset field
            var presetField = targetObj.GetType().GetField(presetFieldName, global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance);
            if (presetField == null) return;

            var preset = presetField.GetValue(targetObj);
            if (preset == null) return;

            // Get the fillParent field
            var fillParentField = preset.GetType().GetField("fillParent");
            if (fillParentField == null) return;

            // Set the fill parent value
            fillParentField.SetValue(preset, fillParent);

            if (fillParent)
            {
                // If filling parent, just set the anchors - don't change sizeDelta
                preset.GetType().GetField("anchorMin").SetValue(preset, Vector2.zero);
                preset.GetType().GetField("anchorMax").SetValue(preset, Vector2.one);

                // Preserve the existing sizeDelta and anchoredPosition values
                var rt = targetObj.GetComponent<RectTransform>();
                if (rt != null)
                {
                    preset.GetType().GetField("sizeDelta").SetValue(preset, rt.sizeDelta);
                    preset.GetType().GetField("anchoredPosition").SetValue(preset, rt.anchoredPosition);
                }
            }
        }
    }
}
#endif
