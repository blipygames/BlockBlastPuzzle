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

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPuzzleGameToolkit.Scripts.Editor
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            EditorApplication.update += InitProject;
        }

        private static void InitProject()
        {
            EditorApplication.update -= InitProject;
            if (EditorApplication.timeSinceStartup < 10 || !EditorPrefs.GetBool(Application.dataPath + "AlreadyOpened"))
            {
                if (SceneManager.GetActiveScene().name != "game" && Directory.Exists("Assets/BlockPuzzleGameToolkit/Scenes"))
                {
                    EditorSceneManager.OpenScene("Assets/BlockPuzzleGameToolkit/Scenes/main.unity");
                }

                EditorPrefs.SetBool(Application.dataPath + "AlreadyOpened", true);
            }
        }
    }
}