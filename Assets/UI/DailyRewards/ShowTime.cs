using UnityEngine;
using TMPro;
using RealTimeSystem;
using System;

public class ShowTime : MonoBehaviour
{
    [field:SerializeField] private TextMeshProUGUI _textTime;
    [field:SerializeField] private bool _useHours;
    [field:SerializeField] private string _format = "{0:D2}:{1:D2}:{2:D2}";
    
    [Header("Init")]
    [field:SerializeField] private TimeEvent _timeEvent;

    public void Init(TimeEvent timeEvent)
    {
        _timeEvent = timeEvent;
        enabled = true;
        
    }

    public void Update()
    {
        _textTime.text = _timeEvent.TimeLeft.ConvertToTime();
    }

}
