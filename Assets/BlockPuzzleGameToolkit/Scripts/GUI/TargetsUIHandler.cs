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

using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.GUI
{
    public class TargetsUIHandler : MonoBehaviour
    {
        public GameObject ScoreLabel;
        public GameObject TargetsLabel;
        public GameObject ClassicModeLabel;
        public GameObject TimedModeLabel;

        public void OnLevelLoaded(ELevelType levelTypeElevelType)
        {
            ScoreLabel.SetActive(levelTypeElevelType == ELevelType.Score);
            TargetsLabel.SetActive(levelTypeElevelType == ELevelType.CollectItems);
            ClassicModeLabel.SetActive(levelTypeElevelType == ELevelType.Classic && GameDataManager.GetGameMode() == EGameMode.Classic);
            TimedModeLabel.SetActive(levelTypeElevelType == ELevelType.Classic && GameDataManager.GetGameMode() == EGameMode.Timed);
        }
    }
}