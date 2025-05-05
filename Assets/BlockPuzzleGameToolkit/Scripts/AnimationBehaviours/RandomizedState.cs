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

namespace BlockPuzzleGameToolkit.Scripts.AnimationBehaviours
{
    public class RandomizedState : StateMachineBehaviour
    {
        [Header("Add RandomFinished trigger to transition to next state")]
        [SerializeField]
        private float minTime = 2f;

        [SerializeField]
        private float maxTime = 4f;

        [SerializeField]
        private string randomfinishedStr = "RandomFinished";

        private float currentTimeBeforeBlink;
        private float timeElapsed;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Randomize();
        }

        private void Randomize()
        {
            currentTimeBeforeBlink = Random.Range(minTime, maxTime);
            timeElapsed = 0f;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= currentTimeBeforeBlink)
            {
                animator.SetTrigger(Animator.StringToHash(randomfinishedStr));
            }
        }
    }
}