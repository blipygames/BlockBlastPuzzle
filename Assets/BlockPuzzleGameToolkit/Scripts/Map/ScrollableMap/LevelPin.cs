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
        private TextMeshProUGUI numberLabel;
        [SerializeField] 
        private Color normalTextColor = Color.white;
        [SerializeField]
        private Color currentTextColor = Color.yellow;
        private bool isLocked;

        private void OnValidate()
        {
            number = transform.GetSiblingIndex() + 1;
            name = "Level_" + number;
            numberLabel.text = number.ToString();
        }

        public void SetNumber(int number)
        {
            this.number = number;
            numberLabel.text = number.ToString();
        }

        public void Lock()
        {
            isLocked = true;
            numberLabel.color = normalTextColor;
            lockObj.SetActive(true);
            openedObj.SetActive(false);
            currentObj.SetActive(false);
        }
        
        public void UnLock()
        {
            isLocked = false;
            numberLabel.color = normalTextColor;
            numberLabel.gameObject.SetActive(true);
            lockObj.SetActive(false);
            openedObj.SetActive(true);
            currentObj.SetActive(false);
        }
        
        public void SetCurrent(bool isCurrent)
        {
            if (isLocked)
                return;
                
            currentObj.SetActive(isCurrent);
            numberLabel.color = isCurrent ? currentTextColor : normalTextColor;
        }

        public void MouseDown()
        {
            if (isLocked)
                return;
            ScrollableMapManager.instance.OpenLevel(number);
        }
    }
}