using System.Linq;
using SOVa.CommonVariables;
using UnityEngine;

namespace SOVa.UpgradeSystem
{
    [CreateAssetMenu(menuName = "Game/Economic/Level upgrader")]
    public class LevelUpgrader : SimpleUpgrader
    {
        [SerializeField] Reference<int> _level;
        [SerializeField, Createble] Int _costPerLevelParemetr;
        public override int Cost
        {
            get
            {
                return _costPerLevelParemetr;
            }
        }
        public override bool IsMaxUpgraded => _level.Value >= ((Int)_level.Variable).Max;
        public int Level => _level.Value;

        protected override void PerformUpgrade()
        {
            _level.Value++;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (_level.HaveReference == false)
            {
                return;
            }
            if (((Int)_level.Variable).IsClamped == false)
            {
                Debug.LogError($"Level {_level.Variable.name} doesn't have Max value!", this);
            }
        }
    }
}
