using System;
using UnityEngine;
using UnityEngine.Events;


namespace SOVa.UpgradeSystem
{
    public abstract class Upgrader : ScriptableObject
    {
        public abstract event UnityAction Upgraded;

        public abstract bool IsMaxUpgraded { get; }
        public abstract bool CanUpgrade { get; }

        protected abstract void PerformUpgrade();

        public abstract void Upgrade();

        protected virtual void OnValidate()
        {
        }
    }
}