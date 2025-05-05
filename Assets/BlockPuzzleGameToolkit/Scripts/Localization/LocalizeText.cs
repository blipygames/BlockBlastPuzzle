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

using System;
using System.Collections.Generic;
using System.Linq;
using BlockPuzzleGameToolkit.Scripts.Settings;
using BlockPuzzleGameToolkit.Scripts.Unity_Reorderable_List_master.List;
using BlockPuzzleGameToolkit.Scripts.Unity_Reorderable_List_master.List.Attributes;
using TMPro;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization
{
    public class LocalizeText : MonoBehaviour
    {
        private TextMeshProUGUI textObject;
        public string instanceID;

        [SerializeField]
        [Reorderable(elementNameProperty = "language")]
        [HideInInspector]
        public MyList objects;

        private string _originalText;
        private string _currentText;

        private void Awake()
        {
            textObject = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _originalText = textObject.text;
            UpdateText();
        }

        public void UpdateText()
        {
            _currentText = LocalizationManager.GetText(instanceID, _originalText);
            textObject.text = _currentText;
        }
    }

    [Serializable]
    public class MyList : ReorderableArray<LanguageObject>
    {
        private SystemLanguage GetSystemLanguage(DebugSettings _debugSettings)
        {
            SystemLanguage lang;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
            {
                lang = _debugSettings.TestLanguage;
            }
            else
            {
                lang = Application.systemLanguage;
            }

            return this.Any(i => i.language == lang) ? _debugSettings.TestLanguage : SystemLanguage.English;
        }

        public IEnumerable<LanguageObject> GetText(DebugSettings _debugSettings)
        {
            var systemLanguage = GetSystemLanguage(_debugSettings);
            return GetText(systemLanguage);
        }

        public IEnumerable<LanguageObject> GetText(SystemLanguage systemLanguage)
        {
            return this.Where(i => i.language == systemLanguage);
        }
    }
}