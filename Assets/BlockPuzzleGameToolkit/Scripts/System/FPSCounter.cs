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

using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.System
{
    public class FPSCounter : MonoBehaviour
    {
        private readonly float updateInterval = 0.5f;
        private float accum;
        private int frames;
        private float timeLeft;
        private float fps;

        private readonly GUIStyle textStyle = new();

        private void Start()
        {
            timeLeft = updateInterval;

            // Set up the GUI style
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.normal.textColor = Color.white;
            textStyle.fontSize = 24; // Adjust this value to change the text size
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            frames++;

            if (timeLeft <= 0f)
            {
                fps = accum / frames;
                timeLeft = updateInterval;
                accum = 0f;
                frames = 0;
            }
        }

        private void OnGUI()
        {
            // Display FPS in the top-left corner
            UnityEngine.GUI.Label(new Rect(10, 10, 100, 30), "FPS: " + fps.ToString("F2"), textStyle);
        }
    }
}