using SOVa;
using UnityEngine;
using UnityEngine.Events;


namespace SOVa.UpgradeSystem
{
    public abstract class SimpleUpgrader : Upgrader
    {
        [SerializeField] private Reference<int> _currency;

        public abstract int Cost { get; }
        public Reference<int> Currency => _currency;
        public override bool CanUpgrade => IsMaxUpgraded == false && _currency.Value >= Cost;
        public override event UnityAction Upgraded;

        [ContextMenu("Upgrade")]
        public override void Upgrade()
        {
            if (CanUpgrade == false)
            {
                return;
            }
            if (Cost != 0)
            {
                _currency.Value -= Cost;
            }

            PerformUpgrade();
            Upgraded?.Invoke(); 
        }
    }
}