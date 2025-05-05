using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI.Orientation
{
    [global::System.Serializable]
    public class ObjectConfiguration
    {
        // Basic Transform properties
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        // RectTransform properties
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 sizeDelta;
        public Vector2 anchoredPosition;

        // Additional RectTransform properties
        public bool fillParent;
        public float width;
        public float height;

        // Flag to determine if this is a RectTransform configuration
        public bool isRectTransform;
    }

    public enum ScreenOrientation
    {
        Portrait,
        Landscape
    }

    public interface IOrientationTransformable
    {
        void ApplyConfiguration(ObjectConfiguration config);
        Transform GetTransform();
    }

    [ExecuteInEditMode]
    public class OrientationManager : MonoBehaviour
    {
        // Remove singleton instance

        [Header("Settings")]
        [SerializeField] private bool checkOnUpdate = true;
        [SerializeField] private bool findTransformableObjectsOnStart = true;
        [SerializeField] private bool detectInEditor = true;

        private ScreenOrientation currentOrientation;
        private bool initialized = false;

        // Dictionary mapping objects to their orientation-specific configurations
        private Dictionary<IOrientationTransformable, Dictionary<ScreenOrientation, ObjectConfiguration>> objectConfigurations =
            new Dictionary<IOrientationTransformable, Dictionary<ScreenOrientation, ObjectConfiguration>>();

        #if UNITY_EDITOR
        // Store the last known game view size for comparison
        private Vector2 lastViewSize = Vector2.zero;
        private float lastAspectRatio = 0f;

        // This method runs in edit mode
        private void OnEnable()
        {
            EditorApplication.update += EditorUpdate;

            // Force an initial update of our orientation
            if (!Application.isPlaying && detectInEditor)
            {
                lastViewSize = GetMainGameViewSize();
                lastAspectRatio = lastViewSize.x / lastViewSize.y;
                currentOrientation = lastAspectRatio > 1 ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;

                // Find and update all transformable objects in the scene
                if (!initialized)
                {
                    FindAndRegisterTransformables();
                    ApplyConfigurations();
                    initialized = true;
                }
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        private void EditorUpdate()
        {
            if (!detectInEditor || Application.isPlaying)
                return;

            // Check if game view size has changed
            Vector2 currentViewSize = GetMainGameViewSize();
            float currentAspectRatio = currentViewSize.x / currentViewSize.y;

            if (currentViewSize != lastViewSize)
            {
                lastViewSize = currentViewSize;
                lastAspectRatio = currentAspectRatio;

                // Determine new orientation based on aspect ratio
                ScreenOrientation newOrientation = currentAspectRatio > 1 ?
                    ScreenOrientation.Landscape : ScreenOrientation.Portrait;

                // If orientation changed, apply the configurations
                if (newOrientation != currentOrientation)
                {
                    currentOrientation = newOrientation;
                    ApplyConfigurations();
                    Debug.Log($"[EDITOR] Game view orientation changed to {currentOrientation}. Aspect ratio: {currentAspectRatio:F2}");

                    // Force the scene view to update
                    SceneView.RepaintAll();
                }
            }
        }

        // Get the current Game view size using EditorWindow API
        private Vector2 GetMainGameViewSize()
        {
            global::System.Type T = global::System.Type.GetType("UnityEditor.GameView,UnityEditor");
            global::System.Reflection.MethodInfo GetSizeOfMainGameView = T?.GetMethod("GetSizeOfMainGameView",
                global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);

            if (GetSizeOfMainGameView != null)
            {
                object res = GetSizeOfMainGameView.Invoke(null, null);
                return (Vector2)res;
            }

            // Fallback if reflection fails
            return new Vector2(Screen.width, Screen.height);
        }
        #endif

        private void Awake()
        {
            // No singleton setup needed
        }

        void Start()
        {
            if (!Application.isPlaying)
                return;

            // Initialize orientation on start
            currentOrientation = DetermineCurrentOrientation();

            if (findTransformableObjectsOnStart)
            {
                FindAndRegisterTransformables();
            }

            ApplyConfigurations();
            initialized = true;
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;

            if (checkOnUpdate)
            {
                ScreenOrientation newOrientation = DetermineCurrentOrientation();
                if (newOrientation != currentOrientation || !initialized)
                {
                    currentOrientation = newOrientation;
                    ApplyConfigurations();
                }
            }
        }

        private ScreenOrientation DetermineCurrentOrientation()
        {
            return Screen.width > Screen.height ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;
        }

        public bool IsLandscape()
        {
            return currentOrientation == ScreenOrientation.Landscape;
        }

        public ScreenOrientation GetCurrentOrientation()
        {
            return currentOrientation;
        }

        public void RegisterConfiguration(IOrientationTransformable obj, ScreenOrientation orientation, ObjectConfiguration config)
        {
            if (!objectConfigurations.ContainsKey(obj))
            {
                objectConfigurations[obj] = new Dictionary<ScreenOrientation, ObjectConfiguration>();
            }

            // Store the configuration for this orientation
            objectConfigurations[obj][orientation] = config;

            // Apply immediately if this matches the current orientation
            if (orientation == currentOrientation)
            {
                obj.ApplyConfiguration(config);
            }
        }

        public void RegisterConfiguration(GameObject obj, ScreenOrientation orientation, ObjectConfiguration config)
        {
            var transformable = obj.GetComponent<OrientationTransformableObject>();
            if (transformable == null)
            {
                transformable = obj.AddComponent<OrientationTransformableObject>();
            }
            RegisterConfiguration(transformable, orientation, config);
        }

        private void ApplyConfigurations()
        {
            foreach (var objEntry in objectConfigurations)
            {
                var obj = objEntry.Key;
                var configs = objEntry.Value;

                if (configs.TryGetValue(currentOrientation, out ObjectConfiguration config))
                {
                    obj.ApplyConfiguration(config);
                }
            }

            string mode = Application.isPlaying ? "PLAY" : "EDITOR";
            Debug.Log($"[{mode}] Applied configurations for {currentOrientation} orientation");
        }

        private void FindAndRegisterTransformables()
        {
            OrientationTransformableObject[] transformables = FindObjectsOfType<OrientationTransformableObject>();
            foreach (var transformable in transformables)
            {
                transformable.RegisterWithManager();
            }
        }

        // Force reapplication of all configurations
        public void RefreshAllConfigurations()
        {
            ApplyConfigurations();
        }

        // Static method to force all orientation-aware objects to update
        public static void UpdateAllTransformables()
        {
            var managers = FindObjectsOfType<OrientationManager>();
            if (managers.Length > 0)
            {
                foreach (var manager in managers)
                {
                    manager.RefreshAllConfigurations();
                }
            }
            else
            {
                Debug.LogWarning("No OrientationManager instance found to update transformables.");
            }
        }
    }
}