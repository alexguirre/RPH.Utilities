namespace RPH.Utilities.AI
{
    // System
    using System;
    using System.Collections.Generic;

    // RPH
    using Rage;
    using Rage.Native;

    public abstract class ComplexObject
    {
        public delegate void ComplexObjectEventHandler(ComplexObject sender);

        protected Dictionary<Type, Component> Components = new Dictionary<Type, Component>();

        public bool CanUpdate { get; set; } = true;
        public bool CanUpdateComponents { get; set; } = true;

        protected uint ComponentsUpdateInterval { get; set; } = 250;
        protected uint LastComponentsUpdateGameTime { get; private set; }

        public bool IsDestroyed { get; private set; }

        public event ComplexObjectEventHandler Destroyed;

        public T AddComponent<T>() where T : Component, new()
        {
            if (HasComponent<T>())
                throw new InvalidOperationException($"This {nameof(ComplexObject)} already contains a component of type {typeof(T).Name}");

            T component = new T();
            component.Parent = this;
            Components.Add(typeof(T), component);
            component.OnStart();
            return component;
        }

        public T GetComponent<T>() where T : Component, new()
        {
            return (Components.ContainsKey(typeof(T)) ? Components[typeof(T)] : null) as T;
        }

        public bool HasComponent<T>() where T : Component, new()
        {
            return Components.ContainsKey(typeof(T)) && Components[typeof(T)] != null;
        }

        internal void Update()
        {
            if (!IsDestroyed)
            {
                OnUpdate();

                UpdateComponents();
            }
        }

        public void Destroy()
        {
            if (!IsDestroyed)
            {
                if (ComplexObjectUpdater.ComplexObjectsByType.ContainsKey(this.GetType()))
                {
                    if (ComplexObjectUpdater.ComplexObjectsByType[this.GetType()].Contains(this))
                    {
                        ComplexObjectUpdater.ComplexObjectsByType[this.GetType()].Remove(this);
                    }
                }

                OnDestroy();
                IsDestroyed = true;
                Destroyed?.Invoke(this);
            }
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnDestroy()
        {
        }


        private void UpdateComponents()
        {
            if (CanUpdateComponents)
            {
                if ((Game.GameTime - LastComponentsUpdateGameTime) > ComponentsUpdateInterval)
                {
                    foreach (Component c in Components.Values)
                    {
                        c.OnTimedUpdate();
                    }

                    LastComponentsUpdateGameTime = Game.GameTime;
                }

                foreach (Component c in Components.Values)
                {
                    c.OnContinuousUpdate();
                }
            }
        }
    }
}
