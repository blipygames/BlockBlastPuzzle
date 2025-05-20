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
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap
{
    public class LevelPin : MonoBehaviour
    {
        [SerializeField]
        public int number = 1;
        [SerializeField]
        private GameObject lockObj;
        [SerializeField]
        private GameObject openedObj;
        [SerializeField]
        private GameObject currentObj;

        [SerializeField]
        private TextMeshProUGUI[] numberLabels;
        [SerializeField] 
        private Color normalTextColor;
        [SerializeField]
        private Color currentTextColor;
        [SerializeField]
        private Color lockTextColor;

        private bool isLocked;

        public bool IsLocked => isLocked;

        private void OnValidate()
        {
            number = transform.GetSiblingIndex() + 1;
            name = "Level_" + number;

            foreach (var numberLabel in numberLabels)
            {
                numberLabel.text = number.ToString();
            }
        }

        public void SetNumber(int number)
        {
            this.number = number;
            foreach (var numberLabel in numberLabels)
            {
                numberLabel.text = number.ToString();
            }
        }

        public void Lock()
        {
            isLocked = true;
            foreach (var numberLabel in numberLabels)
            {
                numberLabel.outlineWidth = 0.0f;
                numberLabel.outlineColor = lockTextColor;
            }
            lockObj.SetActive(true);
            openedObj.SetActive(false);
            currentObj.SetActive(false);
        }
        
        public void UnLock()
        {
            isLocked = false;
            
            foreach (var numberLabel in numberLabels)
            {
                numberLabel.outlineWidth = 0.2f;
                numberLabel.outlineColor = normalTextColor;
                numberLabel.gameObject.SetActive(true);
            }

            lockObj.SetActive(false);
            openedObj.SetActive(true);
            currentObj.SetActive(false);
        }

        public void SetCurrent(bool isCurrent)
        {
            if (isLocked)
                return;

            currentObj.SetActive(isCurrent);

            foreach (var numberLabel in numberLabels)
            {
                numberLabel.outlineWidth = 0.2f;
                numberLabel.outlineColor = isCurrent ? currentTextColor : normalTextColor;
            }
        }

        public void MouseDownScrollableMap()
        {
            if (isLocked)
                return;
            ScrollableMapManager.instance.OpenLevel(number);
        }

        public void MouseDownGridMap()
        {
            if (isLocked)
                return;
            GridMapManager.instance.OpenLevel(number);
        }
    }
}