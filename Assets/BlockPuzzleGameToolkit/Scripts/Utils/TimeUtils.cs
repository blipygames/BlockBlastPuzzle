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

namespace BlockPuzzleGameToolkit.Scripts.Utils
{
    public static class TimeUtils
    {
        public static string GetTimeString(int hours, int minutes, int seconds)
        {
            // if hours more than 24 return days and hours
            if (hours >= 24)
            {
                var days = hours / 24;
                var h = hours % 24;
                return $"{days}d {h:D2}h";
            }

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        public static string GetTimeString(float time)
        {
            var hours = (int)time / 3600;
            var minutes = (int)(time % 3600) / 60;
            var seconds = (int)time % 60;
            return GetTimeString(hours, minutes, seconds);
        }

        public static string GetTimeString(float time, float activeTimeLimit, bool descendant = true)
        {
            return GetTimeString(descendant ? activeTimeLimit - time % activeTimeLimit : time % activeTimeLimit);
        }

        public static float GetTimeInSeconds(string timeString)
        {
            var time = timeString.Split(':');
            if (time.Length == 3)
            {
                return GetTimeInSeconds(int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
            }

            return 0;
        }

        public static float GetTimeInSeconds(int hours, int minutes, int seconds)
        {
            return hours * 3600 + minutes * 60 + seconds;
        }
    }
}