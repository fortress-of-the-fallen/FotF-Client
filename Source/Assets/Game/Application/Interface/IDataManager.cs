#if UNITY_2019_1_OR_NEWER
using System;

namespace Game.Application.Interface
{
	public interface IDataManager
	{
		bool CreateOrReplace<T>(string key, T value);
		bool TryRead<T>(string key, out T value);
		bool Update<T>(string key, T value);
		bool Delete(string key);
		bool Exists(string key);
		void Clear();
	}
}
#endif

