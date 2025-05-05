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

using System.Collections;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Map.ScrollableMap
{
    public class PatrollingParticle : MonoBehaviour
    {
        public ParticleSystem fireflySystem;
        public float boundary = 5f; // adjust boundary to your desired area
        public float checkInterval = 0.2f; // interval to check particle positions
        public float strengthOut = 0.2f; // strength when outside boundary
        public float strengthIn = 1f; // strength when inside boundary

        private ParticleSystem.NoiseModule noiseModule;

        private void Start()
        {
            noiseModule = fireflySystem.noise;
            StartCoroutine(CheckParticlePositions());
        }

        private IEnumerator CheckParticlePositions()
        {
            while (true)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[fireflySystem.particleCount]; 
                fireflySystem.GetParticles(particles);

                for (int i = 0; i < particles.Length; i++)
                {
                    Vector3 directionToCenter = (Vector3.zero - particles[i].position).normalized;
                    float distanceToCenter = Vector3.Distance(Vector3.zero, particles[i].position);

                    if (distanceToCenter > boundary)
                    {
                        particles[i].velocity += directionToCenter * strengthOut;
                    }
                    else
                    {
                        particles[i].velocity += directionToCenter * strengthIn;
                    }
                }

                fireflySystem.SetParticles(particles, particles.Length);

                yield return new WaitForSeconds(checkInterval);
            }
        }
    }
}