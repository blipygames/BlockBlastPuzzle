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

using BlockPuzzleGameToolkit.Scripts.Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace BlockPuzzleGameToolkit.Scripts.Popups
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [SerializeField]
        private Slider musicButton;

        [SerializeField]
        private Slider soundButton;

        [SerializeField]
        private AudioMixer mixer;

        [SerializeField]
        private string musicParameter = "musicVolume";

        [SerializeField]
        private string soundParameter = "soundVolume";

        private void Start()
        {
            musicButton.onValueChanged.AddListener(ToggleMusic);
            soundButton.onValueChanged.AddListener(ToggleSound);
            OnEnable();
        }

        private void OnEnable()
        {
            UpdateButtonState("Music", musicParameter);
            UpdateButtonState("Sound", soundParameter);
        }

        private void UpdateButtonState(string playerPrefKey, string volumeParameter)
        {
            var enabledState = PlayerPrefs.GetInt(playerPrefKey, 1) != 0f;
            float volumeValue = enabledState ? 0 : -80;

            mixer.SetFloat(volumeParameter, volumeValue);
            if (playerPrefKey == "Sound")
            {
                soundButton.value = enabledState ? 1 : 0;
            }
            else
            {
                musicButton.value = enabledState ? 1 : 0;
            }
        }

        private void ToggleMusic(float arg0)
        {
            SoundBase.instance.PlaySound(SoundBase.instance.click);
            PlayerPrefs.SetInt("Music", (int)arg0);
            OnEnable();
        }

        private void ToggleSound(float arg0)
        {
            SoundBase.instance.PlaySound(SoundBase.instance.click);
            PlayerPrefs.SetInt("Sound", (int)arg0);
            OnEnable();
        }
    }
}