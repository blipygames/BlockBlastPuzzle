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

using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    public static class EditorMenu
    {
        public static string BlockPuzzleGameToolkit = "BlockPuzzleGameToolkit";

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Shop settings")]
        public static void IAPProducts()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/CoinsShopSettings.asset");
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Ads settings")]
        public static void AdsSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/AdsSettings.asset");
        }

        //DailyBonusSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Daily bonus settings")]
        public static void DailyBonusSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/DailyBonusSettings.asset");
        }

        //GameSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Game settings")]
        public static void GameSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/GameSettings.asset");
        }

        //SpinSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Spin settings")]
        public static void SpinSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/SpinSettings.asset");
        }

        //DebugSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Debug settings")]
        public static void DebugSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/DebugSettings.asset");
        }

        //TutorialSettings
        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Settings/Tutorial settings")]
        public static void TutorialSettings()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Settings/TutorialSettings.asset");
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Scenes/Main scene &1", priority = 0)]
        public static void MainScene()
        {
            EditorSceneManager.OpenScene("Assets/" + BlockPuzzleGameToolkit + "/Scenes/main.unity");
            StateManager.instance.CurrentState = EScreenStates.MainMenu;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Scenes/Game scene &2")]
        public static void GameScene()
        {
            StateManager.instance.CurrentState = EScreenStates.Game;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Scenes/Map scene &3")]
        public static void MapScene()
        {
            StateManager.instance.CurrentState = EScreenStates.Map;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }


        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Editor/Level Editor _C", priority = 1)]
        public static void LevelEditor()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Levels/Level_1.asset");
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Editor/Color editor", priority = 1)]
        public static void ColorEditor()
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/" + BlockPuzzleGameToolkit + "/Resources/Items/ItemTemplate 0.asset");
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Editor/Shape editor", priority = 1)]
        public static void ShapeEditor()
        {
            var shapeAssets = Resources.LoadAll("Shapes");
            if (shapeAssets.Length > 0)
            {
                Selection.activeObject = shapeAssets[0];
            }
            else
            {
                Debug.LogWarning("No shape assets found in the specified folder.");
            }
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Documentation/Main", priority = 2)]
        public static void MainDoc()
        {
            Application.OpenURL("https://candy-smith.gitbook.io/main");
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Documentation/ADS/Setup ads")]
        public static void UnityadsDoc()
        {
            Application.OpenURL("https://candy-smith.gitbook.io/bubble-shooter-toolkit/tutorials/ads-setup/");
        }

        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Documentation/Unity IAP (in-apps)")]
        public static void Inapp()
        {
            Application.OpenURL("https://candy-smith.gitbook.io/main/block-puzzle-game-toolkit/setting-up-in-app-purchase-products");
        }


        [MenuItem("Tools/" + nameof(BlockPuzzleGameToolkit) + "/Reset PlayerPrefs")]
        private static void ResetPlayerPrefs()
        {
            GameDataManager.ClearALlData();
            PlayerPrefs.DeleteKey("GameState");
            Debug.Log("PlayerPrefs are reset");
        }
    }
}