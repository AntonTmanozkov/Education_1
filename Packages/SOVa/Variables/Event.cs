using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace SOVa
{
    [CreateAssetMenu(menuName = "SOVa/Events/Simple")]
    public class Event : ScriptableObject
    {
        [SerializeField] UnityEvent _unityEvent;
        [SerializeField] private bool _debug;
        
        public delegate void Listener();
        internal List<IEventListener> Listeners = new();
        protected internal bool IsDifficultEvent = false;

        protected internal UnityEvent UnityEvent => _unityEvent;

        public void AddListener(Listener listener, int order = 0) 
        {
            Listeners.Add(new EventListener<Listener>() 
            {
                Listener = listener,
                Pryority = order
            });
            if (order != 0) 
            {
                IsDifficultEvent = true;
            }
            if (IsDifficultEvent) 
            {
                Listeners = Listeners.OrderBy(x => x.Pryority).ToList();
            }
        }

        public void RemoveListener(Listener listener) 
        {
            IEventListener toRemove = Listeners.Find(x => x.Listener == (MulticastDelegate)listener);
            Listeners.Remove(toRemove);
        }

        public virtual void Invoke() 
        {
            #if UNITY_EDITOR
            if (_debug)
            {
                Debug.Log($"Event {name} invoked!", this);
            }
            #endif
            _unityEvent.Invoke();
            foreach (IEventListener element in Listeners.ToArray()) 
            {
                try 
                {
#if UNITY_EDITOR
                    if (_debug)
                    {
                        Debug.Log($"Invoke event {name} for {element.Listener.Method.Name}!", this);
                    }
#endif
                    element.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(Event), true)]
        private class CEditor : Editor 
        {
            private Event targetEvent => target as Event;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (GUILayout.Button("Invoke")) 
                {
                    targetEvent.Invoke();
                }
            }
        }
#endif
    }

    public abstract class Event<T> : Event 
    {
        [SerializeField] UnityEvent<T> _unityEventWithArgs;

        public new delegate void Listener(T argument);

        public void AddListener(Listener listener, int order = 0)
        {
            Listeners.Add(new EventListener<Listener>()
            {
                Listener = listener,
                Pryority = order,
                InvokeWithArgs = true
            });
            if (order != 0)
            {
                IsDifficultEvent = true;
            }
            if (IsDifficultEvent)
            {
                Listeners = Listeners.OrderBy(x => x.Pryority).ToList();
            }
        }

        public void RemoveListener(Listener listener)
        {
            IEventListener toRemove = Listeners.Find(x => x.Listener == (MulticastDelegate)listener);
            Listeners.Remove(toRemove);
        }

        public virtual void Invoke(T arg)
        {
            UnityEvent.Invoke();
            _unityEventWithArgs.Invoke(arg);
            foreach (IEventListener listener in Listeners.ToArray())
            {
                try
                {
                    listener.Invoke(arg);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        [Obsolete("Use invoke with arguments for this event")]
        public new void Invoke()
        {
            Debug.LogError("Use invoke with arguments for this event");
        }
    }
}
