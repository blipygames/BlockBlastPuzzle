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

using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Gameplay
{
    public class StateManager : SingletonBehaviour<StateManager>
    {
        [SerializeField]
        private GameObject[] mainMenus;

        [SerializeField]
        private GameObject[] maps;

        [SerializeField]
        private GameObject[] games;

        private EScreenStates _currentState;

        public EScreenStates CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                SetActiveState(mainMenus, _currentState == EScreenStates.MainMenu);
                SetActiveState(maps, _currentState == EScreenStates.Map);
                SetActiveState(games, _currentState == EScreenStates.Game);
            }
        }

        private void SetActiveState(GameObject[] gameObjects, bool isActive)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.activeSelf != isActive)
                {
                    gameObject.SetActive(isActive);
                }
            }
        }
    }

    public enum EScreenStates
    {
        MainMenu,
        Map,
        Game
    }
}