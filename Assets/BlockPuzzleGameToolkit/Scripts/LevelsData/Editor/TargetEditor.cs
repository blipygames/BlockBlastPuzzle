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

using UnityEditor;
using UnityEngine.UIElements;

namespace BlockPuzzleGameToolkit.Scripts.LevelsData.Editor
{
    [CanEditMultipleObjects]
    // [CustomEditor(typeof(Target), true)]
    public class TargetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // var target = (Target)serializedObject.targetObject;
            //
            var root = new VisualElement();
            // root.style.flexDirection = FlexDirection.Column;
            //
            // var amountField = new IntegerField("Amount")
            // {
            //     value = target.amount
            // };
            // amountField.RegisterValueChangedCallback(evt =>
            // {
            //     target.amount = evt.newValue;
            //     MarkDirtyAndSave(target);
            // });
            //
            // root.Add(amountField);


            return root;
        }

        private void MarkDirtyAndSave(TargetScriptable targetScriptable)
        {
            EditorUtility.SetDirty(targetScriptable);
            AssetDatabase.SaveAssets();
        }
    }
}