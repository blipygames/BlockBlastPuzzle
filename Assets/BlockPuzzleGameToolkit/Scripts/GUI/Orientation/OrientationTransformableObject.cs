using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI.Orientation
{
    [RequireComponent(typeof(Transform)), ExecuteInEditMode]
    public class OrientationTransformableObject : MonoBehaviour, IOrientationTransformable
    {
        [global::System.Serializable]
        public class OrientationPreset
        {
            public string name = "Configuration";
        
            // Basic Transform properties
            public Vector3 position;
            public Vector3 rotationEuler;
            public Vector3 scale = Vector3.one;
        
            // RectTransform properties
            public Vector2 anchorMin = Vector2.zero;
            public Vector2 anchorMax = Vector2.one;
            public Vector2 pivot = new Vector2(0.5f, 0.5f);
            public Vector2 sizeDelta = Vector2.zero;
            public Vector2 anchoredPosition = Vector2.zero;
        
            // Additional RectTransform settings
            [Tooltip("If true, this object will fill its parent container")]
            public bool fillParent = false;
        
            [Tooltip("Use this to force a specific width (only applies when not filling parent)")]
            public float width = 0;
        
            [Tooltip("Use this to force a specific height (only applies when not filling parent)")]
            public float height = 0;

            public ObjectConfiguration ToObjectConfiguration()
            {
                return new ObjectConfiguration
                {
                    // Basic transform properties
                    position = position,
                    rotation = Quaternion.Euler(rotationEuler),
                    scale = scale,
                
                    // RectTransform properties
                    anchorMin = anchorMin,
                    anchorMax = anchorMax,
                    pivot = pivot,
                    sizeDelta = sizeDelta,
                    anchoredPosition = anchoredPosition,
                
                    // Additional properties
                    fillParent = fillParent,
                    width = width,
                    height = height,
                
                    // Flag for RectTransform
                    isRectTransform = true
                };
            }
        }

        private OrientationManager orientationManager;
        private RectTransform rectTransform;
        private bool isRectTransform;

        [SerializeField] private bool registerOnEnable = true;
    
        [Header("Configurations")]
        [SerializeField] private OrientationPreset landscapeConfig = new OrientationPreset { name = "Landscape" };
        [SerializeField] private OrientationPreset portraitConfig = new OrientationPreset { name = "Portrait" };
    
        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color landscapeGizmoColor = Color.blue;
        [SerializeField] private Color portraitGizmoColor = Color.red;
        [SerializeField] private float gizmoSize = 0.1f;

        void Awake()
        {
            // Check if this is a RectTransform
            rectTransform = GetComponent<RectTransform>();
            isRectTransform = rectTransform != null;
        }

        // Helper method to ensure we have a valid RectTransform reference
        private bool EnsureRectTransform()
        {
            if (rectTransform == null)
            {
                // Try to get the reference again
                rectTransform = GetComponent<RectTransform>();
                isRectTransform = rectTransform != null;
            }
            return isRectTransform;
        }

        void OnEnable()
        {
            if (registerOnEnable)
            {
                RegisterWithManager();
            }
        }

        void Start()
        {
            // Ensure we register if we weren't already registered in OnEnable
            if (!registerOnEnable)
            {
                RegisterWithManager();
            }
        }

        public void RegisterWithManager()
        {
            orientationManager = FindOrientationManager();
            if (orientationManager == null)
            {
                Debug.LogError("No OrientationManager found in scene!");
                return;
            }

            // Register both configurations
            orientationManager.RegisterConfiguration(this, ScreenOrientation.Landscape, landscapeConfig.ToObjectConfiguration());
            orientationManager.RegisterConfiguration(this, ScreenOrientation.Portrait, portraitConfig.ToObjectConfiguration());
        }

        private OrientationManager FindOrientationManager()
        {
            // Try to find in scene
            var manager = FindObjectOfType<OrientationManager>();
            if (manager != null)
                return manager;

            // Create one if needed
            GameObject managerObj = new GameObject("OrientationManager");
            return managerObj.AddComponent<OrientationManager>();
        }

        public void ApplyConfiguration(ObjectConfiguration config)
        {
            if (isRectTransform && config.isRectTransform)
            {
                // Apply RectTransform properties
                rectTransform.pivot = config.pivot;
            
                // Apply size and anchors based on fill mode
                if (config.fillParent)
                {
                    // Fill mode: use anchors from config instead of hardcoded values
                    rectTransform.anchorMin = config.anchorMin;
                    rectTransform.anchorMax = config.anchorMax;
                
                    // Use the stored sizeDelta values, not Vector2.zero
                    rectTransform.sizeDelta = config.sizeDelta;
                    rectTransform.anchoredPosition = config.anchoredPosition;
                }
                else
                {
                    // Normal mode: apply anchors and size as configured
                    rectTransform.anchorMin = config.anchorMin;
                    rectTransform.anchorMax = config.anchorMax;
                    rectTransform.sizeDelta = config.sizeDelta;
                    rectTransform.anchoredPosition = config.anchoredPosition;
                
                    // Apply explicit width/height if needed
                    if (config.width > 0 || config.height > 0)
                    {
                        if (config.width > 0 && Mathf.Approximately(rectTransform.anchorMin.x, rectTransform.anchorMax.x))
                        {
                            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, config.width);
                        }
                    
                        if (config.height > 0 && Mathf.Approximately(rectTransform.anchorMin.y, rectTransform.anchorMax.y))
                        {
                            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, config.height);
                        }
                    }
                }
            
                rectTransform.localScale = config.scale;
                rectTransform.localRotation = config.rotation;
            }
            else
            {
                // Apply regular Transform properties
                transform.position = config.position;
                transform.rotation = config.rotation;
                transform.localScale = config.scale;
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }

        // Editor methods for previewing configurations
        public void PreviewLandscapeConfiguration()
        {
            ApplyConfiguration(landscapeConfig.ToObjectConfiguration());
        }
    
        public void PreviewPortraitConfiguration()
        {
            ApplyConfiguration(portraitConfig.ToObjectConfiguration());
        }
    
        // Update configuration with current transform values
        public void UpdateLandscapeFromCurrentTransform()
        {
            landscapeConfig.position = transform.position;
            landscapeConfig.rotationEuler = transform.rotation.eulerAngles;
            landscapeConfig.scale = transform.localScale;
        
            if (EnsureRectTransform())
            {
                // Always store the actual RectTransform properties regardless of fill status
                landscapeConfig.anchorMin = rectTransform.anchorMin;
                landscapeConfig.anchorMax = rectTransform.anchorMax;
                landscapeConfig.pivot = rectTransform.pivot;
                landscapeConfig.sizeDelta = rectTransform.sizeDelta;
                landscapeConfig.anchoredPosition = rectTransform.anchoredPosition;
            
                // Also store the current width and height
                landscapeConfig.width = rectTransform.rect.width;
                landscapeConfig.height = rectTransform.rect.height;
            
                // Set the fillParent flag based on detection
                landscapeConfig.fillParent = IsRectTransformFilling(rectTransform);
            }
        }
    
        public void UpdatePortraitFromCurrentTransform()
        {
            portraitConfig.position = transform.position;
            portraitConfig.rotationEuler = transform.rotation.eulerAngles;
            portraitConfig.scale = transform.localScale;
        
            if (EnsureRectTransform())
            {
                // Always store the actual RectTransform properties regardless of fill status
                portraitConfig.anchorMin = rectTransform.anchorMin;
                portraitConfig.anchorMax = rectTransform.anchorMax;
                portraitConfig.pivot = rectTransform.pivot;
                portraitConfig.sizeDelta = rectTransform.sizeDelta;
                portraitConfig.anchoredPosition = rectTransform.anchoredPosition;
            
                // Also store the current width and height
                portraitConfig.width = rectTransform.rect.width;
                portraitConfig.height = rectTransform.rect.height;
            
                // Set the fillParent flag based on detection
                portraitConfig.fillParent = IsRectTransformFilling(rectTransform);
            }
        }
    
        // Helper method to determine if a RectTransform is set to fill its parent
        private bool IsRectTransformFilling(RectTransform rt)
        {
            // A rect transform is "filling" if:
            // 1. Its anchors are set to fill (0,0) to (1,1)
            // 2. Its sizeDelta is zero (no additional size)
            // 3. Its anchored position is zero (centered)
        
            bool anchorsAreFilling = 
                Mathf.Approximately(rt.anchorMin.x, 0) && 
                Mathf.Approximately(rt.anchorMin.y, 0) &&
                Mathf.Approximately(rt.anchorMax.x, 1) && 
                Mathf.Approximately(rt.anchorMax.y, 1);
            
            bool sizeIsZero = 
                Mathf.Approximately(rt.sizeDelta.x, 0) && 
                Mathf.Approximately(rt.sizeDelta.y, 0);
            
            bool positionIsZero = 
                Mathf.Approximately(rt.anchoredPosition.x, 0) && 
                Mathf.Approximately(rt.anchoredPosition.y, 0);
            
            return anchorsAreFilling && (sizeIsZero || positionIsZero);
        }

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // Draw indicators for both configurations
            if (isRectTransform)
            {
                // For RectTransforms, draw rectangles representing the UI element
                DrawRectTransformGizmo(landscapeConfig, landscapeGizmoColor);
                DrawRectTransformGizmo(portraitConfig, portraitGizmoColor);
            }
            else
            {
                // For regular transforms, draw spheres
                Gizmos.color = landscapeGizmoColor;
                Gizmos.DrawWireSphere(landscapeConfig.position, gizmoSize);
            
                Gizmos.color = portraitGizmoColor;
                Gizmos.DrawWireSphere(portraitConfig.position, gizmoSize);
            }
        }
    
        private void DrawRectTransformGizmo(OrientationPreset config, Color color)
        {
            Gizmos.color = color;
        
            // Calculate the four corners based on anchored position and size
            Vector2 size = config.sizeDelta;
            Vector2 pos = config.anchoredPosition;
        
            // This is a simplification - in a real UI, the actual position would depend
            // on the canvas and parent rect transforms
            Vector3 center = new Vector3(pos.x, pos.y, 0);
            Vector3 halfSize = new Vector3(size.x * 0.5f, size.y * 0.5f, 0);
        
            Vector3 topLeft = center + new Vector3(-halfSize.x, halfSize.y, 0);
            Vector3 topRight = center + new Vector3(halfSize.x, halfSize.y, 0);
            Vector3 bottomLeft = center + new Vector3(-halfSize.x, -halfSize.y, 0);
            Vector3 bottomRight = center + new Vector3(halfSize.x, -halfSize.y, 0);
        
            // Draw rect wireframe
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        
            // Draw pivot indicator
            Vector3 pivotPos = center + new Vector3(
                (config.pivot.x - 0.5f) * size.x, 
                (config.pivot.y - 0.5f) * size.y, 
                0);
            Gizmos.DrawWireSphere(pivotPos, gizmoSize * 0.5f);
        }
        #endif
    }
}
