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

using BlockPuzzleGameToolkit.Scripts.Attributes;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEditor;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.Editor.Drawers
{
    // Custom attribute

    // Drawer for the custom attribute
    [CustomPropertyDrawer(typeof(IconPreviewAttribute))]
    public class IconDrawer : PropertyDrawer
    {
        private Label m_Icon;
        private ScriptableData m_IconScriptable;
        private SerializedProperty m_property;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            m_property = property;
            m_Icon = new Label();
            m_Icon.style.width = 200;
            m_Icon.style.height = 200;

            // get parent of the property
            m_IconScriptable = property.serializedObject.targetObject as ScriptableData;
            if (m_IconScriptable != null)
            {
                m_IconScriptable.OnChange += UpdatePreview;
            }

            UpdatePreview();
            return m_Icon;
        }

        private void UpdatePreview()
        {
            EditorApplication.delayCall += () => 
            { 
                var itemTemplate = m_IconScriptable as ItemTemplate;
                if (itemTemplate != null && itemTemplate.HasCustomPrefab())
                {
                    m_Icon.style.backgroundImage = EditorUtils.GetPrefabPreview(itemTemplate.customItemPrefab.gameObject);
                }
                else
                {
                    m_Icon.style.backgroundImage = EditorUtils.GetCanvasPreviewVisualElement(m_IconScriptable.prefab, obj => obj.FillIcon(m_IconScriptable));
                }
            };
        }
    }
}