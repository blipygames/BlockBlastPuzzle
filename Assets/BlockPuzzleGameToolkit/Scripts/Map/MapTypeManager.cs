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

using System;
using System.Collections.Generic;
using BlockPuzzleGameToolkit.Scripts.Enums;
using BlockPuzzleGameToolkit.Scripts.System;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Map
{
    [Serializable]
    public struct MapTypeBinding
    {
        public GameObject mapContainer;
        public EMapType mapType;
    }
    
    [ExecuteInEditMode]
    public class MapTypeManager : SingletonBehaviour<MapTypeManager>
    {
        [SerializeField]
        private List<MapTypeBinding> mapBindings = new List<MapTypeBinding>();
        
        private void OnEnable()
        {
            ApplyMapType();
        }
        
        public void ApplyMapType()
        {
            EMapType currentMapType = GameManager.instance.GameSettings.mapType;
            
            // Set active state for all map containers
            foreach (var binding in mapBindings)
            {
                if (binding.mapContainer != null)
                {
                    binding.mapContainer.SetActive(binding.mapType == currentMapType);
                }
            }
        }
        
        public void SwitchMapType(EMapType newMapType)
        {
            // Update the GameSettings mapType value
            GameManager.instance.GameSettings.mapType = newMapType;
            
            // Apply the new map type
            ApplyMapType();
        }
    }
} 