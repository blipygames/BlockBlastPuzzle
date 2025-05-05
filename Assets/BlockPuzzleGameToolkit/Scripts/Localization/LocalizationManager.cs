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
using BlockPuzzleGameToolkit.Scripts.Settings;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Localization
{
    public class LocalizationManager : SingletonBehaviour<LocalizationManager>
    {
        private static DebugSettings _debugSettings;
        public static Dictionary<string, string> _dic;
        private static SystemLanguage _currentLanguage;

        public override void Awake()
        {
            InitializeLocalization();
        }

        public static void InitializeLocalization()
        {
            _debugSettings = Resources.Load("Settings/DebugSettings") as DebugSettings;
            LoadLanguage(GetSystemLanguage());
        }

        public static void LoadLanguage(SystemLanguage language)
        {
            _currentLanguage = language;
            var txt = Resources.Load<TextAsset>($"Localization/{language}");
            if (txt == null)
            {
                Debug.LogWarning($"Localization file for {language} not found. Falling back to English.");
                txt = Resources.Load<TextAsset>("Localization/English");
                _currentLanguage = SystemLanguage.English;
            }

            _dic = new Dictionary<string, string>();
            var lines = txt.text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var inp_ln in lines)
            {
                var l = inp_ln.Split(new[] { ':' }, 2);
                if (l.Length == 2)
                {
                    var key = l[0].Trim();
                    var text = l[1].Trim();
                    _dic[key] = text;
                }
            }
        }

        public static SystemLanguage GetSystemLanguage()
        {
            if (_debugSettings == null)
            {
                _debugSettings = Resources.Load("Settings/DebugSettings") as DebugSettings;
            }

            if (Application.isEditor)
            {
                return _debugSettings.TestLanguage;
            }

            return Application.systemLanguage;
        }

        public static string GetText(string key, string defaultText)
        {
            var currentLanguage = GetSystemLanguage();
            if (_currentLanguage != currentLanguage || _dic == null || _dic.Count == 0)
            {
                LoadLanguage(currentLanguage);
            }

            if (!_dic.ContainsKey(key))
            {
                LoadLanguage(currentLanguage);
            }

            if (_dic.TryGetValue(key, out var localizedText) && !string.IsNullOrEmpty(localizedText))
            {
                return PlaceholderManager.ReplacePlaceholders(localizedText);
            }

            return PlaceholderManager.ReplacePlaceholders(defaultText);
        }

        public static SystemLanguage GetCurrentLanguage()
        {
            return _currentLanguage;
        }
    }
}