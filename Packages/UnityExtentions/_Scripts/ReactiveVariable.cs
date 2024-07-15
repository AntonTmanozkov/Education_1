using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtentions
{
	[Serializable]
	public class ReactiveVariable<T>
	{
		[SerializeField] private T _value;

		private UnityEvent<T> _changed = new();

		public UnityEvent<T> Changed => _changed;
		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_changed.Invoke(_value);
			}
		}

		public ReactiveVariable(T value)
		{
			_value = value;
		}
	}
}