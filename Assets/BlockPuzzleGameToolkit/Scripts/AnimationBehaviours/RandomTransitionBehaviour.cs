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

using System.Collections.Generic;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.AnimationBehaviours
{
    public class RandomTransitionBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Get all parameters of the animator
            var parameters = animator.parameters;

            // Filter and store trigger parameters
            var triggerParams = new List<AnimatorControllerParameter>();
            foreach (var param in parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger)
                {
                    triggerParams.Add(param);
                }
            }

            // Check if there are any trigger parameters
            if (triggerParams.Count > 0)
            {
                // Select a random trigger
                var randomIndex = Random.Range(0, triggerParams.Count);
                var randomTriggerName = triggerParams[randomIndex].name;

                // Set the random trigger
                animator.SetTrigger(randomTriggerName);
            }
        }
    }
}