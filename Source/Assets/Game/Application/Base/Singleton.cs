#if UNITY_2019_1_OR_NEWER
using System;

namespace Game.Application.Base
{
	public abstract class Singleton<T> where T : class, new()
	{
		private static readonly object _lock = new object();
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance != null) return _instance;
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new T();
					}
				}
				return _instance;
			}
		}

		protected Singleton() { }
	}
}
#endif

