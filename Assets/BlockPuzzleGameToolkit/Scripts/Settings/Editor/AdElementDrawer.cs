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

#if UNITY_EDITOR
using BlockPuzzleGameToolkit.Scripts.Services.Ads.AdUnits;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Settings.Editor
{
    [CustomPropertyDrawer(typeof(AdElement))]
    public class AdElementDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create a new VisualElement
            var root = new VisualElement();

            // Add placementId field
            var placementIdField = new PropertyField(property.FindPropertyRelative("placementId"));
            root.Add(placementIdField);

            // Add adTypeScriptable field
            var adTypeScriptableProperty = property.FindPropertyRelative("adReference");
            var adTypeScriptableField = new PropertyField(adTypeScriptableProperty);
            root.Add(adTypeScriptableField);

            // Add popup field
            var popupField = new PropertyField(property.FindPropertyRelative("popup"));
            root.Add(popupField);

            adTypeScriptableField.RegisterValueChangeCallback(evt =>
            {
                // Retrieve the selected adType from the AdTypeScriptable
                var adTypeScriptableObject = (AdReference)adTypeScriptableProperty.objectReferenceValue;
                if (adTypeScriptableObject != null && adTypeScriptableObject.adType == EAdType.Interstitial)
                {
                    popupField.visible = true;
                }
                else
                {
                    if (root.Contains(popupField))
                    {
                        popupField.visible = false;
                    }
                }
            });

            // Add popup field only if adType is not Rewarded
            if (adTypeScriptableProperty != null)
            {
                var adTypeScriptableObject = (AdReference)adTypeScriptableProperty.objectReferenceValue;
                if (adTypeScriptableObject != null && adTypeScriptableObject.adType == EAdType.Rewarded)
                {
                    root.Remove(popupField);
                }
            }

            // Return the root VisualElement
            return root;
        }
    }
}
#endif