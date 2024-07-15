using System.Collections;
using UnityEngine;

namespace RealTimeSystem
{
    public class TimeEventObject : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private TimeEvent _timeEvent;
        [SerializeField] private Mode _mode; 

        private Coroutine _enableCoroutune;

        private void Awake()
        {
            _timeEvent.TimerBegined += UpdateView;
            UpdateView();
        }

        private void Start()
        {
            if (_timeEvent.IsTicking)
            {
                _enableCoroutune = StartCoroutine(EnableCoroutine());
            }
        }

        private void OnDestroy()
        {
            _timeEvent.TimerBegined -= UpdateView;
            StopCoroutine(_enableCoroutune);
        }

        private IEnumerator EnableCoroutine()
        {
            yield return new WaitForSeconds((float)_timeEvent.TimeLeft.TotalSeconds);
            UpdateView();
        }

        private void UpdateView()
        {
            switch (_mode)
            {
                case Mode.EnabledWhileTicking:
                {
                    _target.SetActive(_timeEvent.IsTicking);
                }
                break;
                case Mode.DisabledWhileTicking:
                {
                    _target.SetActive(_timeEvent.IsTicking == false);
                }
                break;
            }
        }

        private enum Mode
        {
            EnabledWhileTicking,
            DisabledWhileTicking
        }
    }
}
