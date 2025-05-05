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

namespace BlockPuzzleGameToolkit.Scripts.Data
{
    public class ResourceManager : SingletonBehaviour<ResourceManager>
    {
        private ResourceObject[] resources;

        public ResourceObject[] Resources
        {
            get
            {
                if (resources == null || resources.Length == 0)
                {
                    Init();
                }

                return resources;
            }
            set => resources = value;
        }

        public override void Awake()
        {
            base.Awake();
            Init();
        }

        private void Init()
        {
            Resources = UnityEngine.Resources.LoadAll<ResourceObject>("Variables");
            foreach (var resource in Resources)
            {
                resource.LoadPrefs();
            }
        }

        public bool Consume(string resourceName, int amount)
        {
            var resource = GetResource(resourceName);
            if (resource == null)
            {
                Debug.LogError($"Resource {resourceName} not found");
                return false;
            }

            return resource.Consume(amount);
        }

        public ResourceObject GetResource(string resourceName)
        {
            foreach (var resource in Resources)
            {
                if (resource.name == resourceName)
                {
                    return resource;
                }
            }

            return null;
        }
    }
}