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
using System.Threading.Tasks;
using UnityEngine;

namespace BlockPuzzleGameToolkit.Scripts.Data
{
    public abstract class ResourceObject : ScriptableObject
    {
        public ResourceValue ResourceValue;

        //name of the resource
        private string ResourceName => name;

        public abstract int DefaultValue { get; }

        //value of the resource
        private int Resource;

        public AudioClip sound;

        //delegate for resource update
        public delegate void ResourceUpdate(int count);

        //event for resource update
        public event ResourceUpdate OnResourceUpdate;

        //runs when the object is created
        private void OnEnable()
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await LoadPrefs();
            });
        }

        //loads prefs from player prefs and assigns to resource variable
        public Task LoadPrefs()
        {
            Resource = LoadResource();
            return Task.CompletedTask;
        }

        public int LoadResource()
        {
            return PlayerPrefs.GetInt(ResourceName, DefaultValue);
        }

        //adds amount to resource and saves to player prefs
        public void Add(int amount)
        {
            Resource += amount;
            PlayerPrefs.SetInt(ResourceName, Resource);
            OnResourceChanged();
        }

        //sets resource to amount and saves to player prefs
        public void Set(int amount)
        {
            Resource = amount;
            PlayerPrefs.SetInt(ResourceName, Resource);
            PlayerPrefs.Save();
            OnResourceChanged();
        }

        //consumes amount from resource and saves to player prefs if there is enough
        public virtual bool Consume(int amount)
        {
            if (IsEnough(amount))
            {
                Resource -= amount;
                PlayerPrefs.SetInt(ResourceName, Resource);
                PlayerPrefs.Save();
                OnResourceChanged();
                return true;
            }

            return false;
        }

        //callback for ui elements
        private void OnResourceChanged()
        {
            OnResourceUpdate?.Invoke(Resource);
        }

        //get the resource
        public int GetValue()
        {
            return Resource;
        }

        //check if there is enough of the resource
        public bool IsEnough(int targetAmount)
        {
            if (GetValue() < targetAmount)
            {
                Debug.Log("Not enough " + ResourceName);
            }

            return GetValue() >= targetAmount;
        }

        public abstract void ResetResource();
    }

    [Serializable]
    public class ResourceValue
    {
    }
}