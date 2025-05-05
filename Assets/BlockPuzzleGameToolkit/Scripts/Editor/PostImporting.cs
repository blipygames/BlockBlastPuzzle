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
using System.IO;
using BlockPuzzleGameToolkit.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    public class PostImporting : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            CheckDefines("Assets/GoogleMobileAds", "ADMOB");
            CheckDefines("Assets/FacebookSDK", "FACEBOOK");
            CheckDefines("Assets/PlayFabSDK", "PLAYFAB");
            CheckDefines("Assets/GameSparks", "GAMESPARKS");
            CheckDefines("Assets/Appodeal", "APPODEAL");
        }

        private static void CheckDefines(string path, string symbols)
        {
            if (Directory.Exists(path))
            {
                DefineSymbolsUtils.AddSymbol(symbols);
            }
            else
            {
                DefineSymbolsUtils.DeleteSymbol(symbols);
            }
        }

        public static void CheckIronsourceFolder()
        {
            var str = "Assets/LevelPlay";
            if (Directory.Exists(str))
            {
                var asmdefPath = Path.Combine(str, "IronsourceAssembly.asmdef");
                if (!File.Exists(asmdefPath))
                {
                    CreateAsmdefIronSource(asmdefPath);
                }

                // get GUID of the IronsourceAssembly.asmdef
                var guid = AssetDatabase.AssetPathToGUID(asmdefPath);
                // assign asmdef to the Scripts/Ads/CandySmith.Ads.asmdef
                var adsAsmdefPath = Path.Combine("Assets/BlockPuzzleGameToolkit/Scripts/Ads", "CandySmith.Ads.asmdef");
                if (File.Exists(adsAsmdefPath))
                {
                    var asmdef = JsonUtility.FromJson<AssemblyDefinition>(File.ReadAllText(adsAsmdefPath));
                    // check references and add IronsourceAssembly if not exists
                    if (asmdef.references == null)
                    {
                        asmdef.references = new[] { "IronsourceAssembly" };
                    }
                    else
                    {
                        if (Array.IndexOf(asmdef.references, "IronsourceAssembly") == -1 && Array.IndexOf(asmdef.references, "GUID:" + guid) == -1)
                        {
                            Array.Resize(ref asmdef.references, asmdef.references.Length + 1);
                            asmdef.references[asmdef.references.Length - 1] = "IronsourceAssembly";
                        }
                    }

                    File.WriteAllText(adsAsmdefPath, JsonUtility.ToJson(asmdef, true));
                    AssetDatabase.Refresh();
                }
            }
        }

        private static void CreateAsmdefIronSource(string path)
        {
            var assemblyDefinition = new AssemblyDefinition
            {
                name = "IronsourceAssembly",
                references = new string[0],
                includePlatforms = new string[0],
                excludePlatforms = new string[0],
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new string[0],
                autoReferenced = true,
                defineConstraints = new string[0],
                versionDefines = new VersionDefine[]
                {
                    new()
                    {
                        name = "com.unity.services.levelplay",
                        define = "IRONSOURCE"
                    }
                }
            };

            File.WriteAllText(path, JsonUtility.ToJson(assemblyDefinition, true));
            AssetDatabase.Refresh();
        }
    }

    [Serializable]
    public class AssemblyDefinition
    {
        public string name;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public VersionDefine[] versionDefines;
    }

    [Serializable]
    public class VersionDefine
    {
        public string name;
        public string expression;
        public string define;
    }
}