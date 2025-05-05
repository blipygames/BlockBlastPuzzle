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

namespace BlockPuzzleGameToolkit.Scripts.System
{
    public static class EventManager
    {
        // A dictionary to hold all events
        private static readonly Dictionary<EGameEvent, object> events = new();

        public static Event<T> GetEvent<T>(EGameEvent eventName)
        {
            if (events.TryGetValue(eventName, out var e) && e is Event<T> typedEvent)
            {
                return typedEvent;
            }

            var newEvent = new Event<T>();
            events[eventName] = newEvent;
            return newEvent;
        }

        // no generic event
        public static Event GetEvent(EGameEvent eventName)
        {
            if (events.TryGetValue(eventName, out var e) && e is Event typedEvent)
            {
                return typedEvent;
            }

            var newEvent = new Event();
            events[eventName] = newEvent;
            return newEvent;
        }

        public static Dictionary<EGameEvent, object> GetSubscribedEvents()
        {
            return events;
        }

        public static Action<EGameState> OnGameStateChanged;
        private static EGameState gameStatus;

        public static EGameState GameStatus
        {
            get => gameStatus;
            set
            {
                gameStatus = value;
                OnGameStateChanged?.Invoke(gameStatus);
            }
        }
    }

    public class Event
    {
        private event Action _event;

        public void Subscribe(Action subscriber)
        {
            _event += subscriber;
        }

        public void Unsubscribe(Action subscriber)
        {
            _event -= subscriber;
        }

        public void Invoke()
        {
            _event?.Invoke();
        }
    }
}