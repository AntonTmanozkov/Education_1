using UnityEngine;
using RealTimeSystem;
using TMPro;

public class TimeEventLabel : MonoBehaviour
{
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private bool _useHours;
        
        [SerializeField] private string _format = "{0:D2}:{1:D2}:{2:D2}";
        
        [Header("Init")]
        [SerializeField] private TimeEvent _runtimeEvent;

        public void Init(TimeEvent timeEvent)
        {
            _runtimeEvent = timeEvent;
            enabled = true;
        }

        public void Update()
        {
            _label.text = _runtimeEvent.TimeLeft.ConvertToTime();
        }
}
