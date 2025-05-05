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

using BlockPuzzleGameToolkit.Scripts.LevelsData;
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Item : FillAndPreview
    {
        public ItemTemplate itemTemplate;
        public Image backgroundColor;
        public Image underlayColor;
        public Image bottomColor;
        public Image topColor;
        public Image leftColor;
        public Image rightColor;
        public Image overlayColor;
        private Vector2Int position;
        public Bonus bonus;
        public BonusItemTemplate bonusItemTemplate;

        private void Awake()
        {
            bonus?.gameObject.SetActive(false);
            if (itemTemplate != null)
            {
                UpdateColor(itemTemplate);
            }
        }

        public void UpdateColor(ItemTemplate itemTemplate)
        {
            this.itemTemplate = itemTemplate;
            backgroundColor.color = itemTemplate.backgroundColor;
            underlayColor.color = itemTemplate.underlayColor;
            bottomColor.color = itemTemplate.bottomColor;
            topColor.color = itemTemplate.topColor;
            leftColor.color = itemTemplate.leftColor;
            rightColor.color = itemTemplate.rightColor;
            overlayColor.color = itemTemplate.overlayColor;
        }

        public void SetBonus(BonusItemTemplate template)
        {
            bonusItemTemplate = template;
            bonus.gameObject.SetActive(true);
            bonus.FillIcon(template);
        }

        public override void FillIcon(ScriptableData iconScriptable)
        {
            UpdateColor((ItemTemplate)iconScriptable);
        }

        public void SetPosition(Vector2Int vector2Int)
        {
            position = vector2Int;
        }

        public Vector2Int GetPosition()
        {
            return position;
        }

        public bool HasBonusItem()
        {
            return bonusItemTemplate != null;
        }

        public void ClearBonus()
        {
            bonusItemTemplate = null;
            bonus.gameObject.SetActive(false);
        }

        public void SetTransparency(float alpha)
        {
            var color = backgroundColor.color;
            color.a = alpha;
            backgroundColor.color = color;

            color = underlayColor.color;
            color.a = alpha;
            underlayColor.color = color;

            color = bottomColor.color;
            color.a = alpha;
            bottomColor.color = color;

            color = topColor.color;
            color.a = alpha;
            topColor.color = color;

            color = leftColor.color;
            color.a = alpha;
            leftColor.color = color;

            color = rightColor.color;
            color.a = alpha;
            rightColor.color = color;

            color = overlayColor.color;
            color.a = alpha;
            overlayColor.color = color;

            bonus?.SetTransparency(alpha);
        }
    }
}