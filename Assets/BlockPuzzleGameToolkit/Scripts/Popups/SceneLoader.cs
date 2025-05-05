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
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.Gameplay;
using BlockPuzzleGameToolkit.Scripts.LevelsData;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        public static Action<Scene> OnSceneLoadedCallback;
        private Loading loading;
        private Scene previouseScene;

        private void Start()
        {
            CheckEvent(SceneManager.GetActiveScene());
        }

        public void StartGameSceneTimed()
        {
            GameDataManager.SetGameMode(EGameMode.Timed);
            GameDataManager.SetLevel(Resources.Load<Level>("Misc/TimeLevel"));
            StateManager.instance.CurrentState = EScreenStates.Game;
        }

        public void StartGameSceneClassic()
        {
            GameDataManager.SetGameMode(EGameMode.Classic);
            GameDataManager.SetLevel(Resources.Load<Level>("Misc/ClassicLevel"));
            StateManager.instance.CurrentState = EScreenStates.Game;
        }

        public void StartGameScene(int levelNumber = 0)
        {
            GameDataManager.SetGameMode(EGameMode.Adventure);
            GameDataManager.SetLevel(Resources.Load<Level>("Levels/Level_" + (levelNumber > 0 ? levelNumber : GameDataManager.GetLevelNum())));
            StateManager.instance.CurrentState = EScreenStates.Game;
        }

        public void GoMain()
        {
            StateManager.instance.CurrentState = EScreenStates.MainMenu;
        }



        private void CheckEvent(Scene scene)
        {
            if (previouseScene != scene)
            {
                OnSceneLoadedCallback?.Invoke(scene);
                previouseScene = scene;
            }
        }

        public void StartMapScene()
        {
            StateManager.instance.CurrentState = EScreenStates.Map;
        }

        public void StartGameSceneTimeTrial()
        {

        }
    }
}