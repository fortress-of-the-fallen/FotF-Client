#if UNITY_2019_1_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using Game.Application.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityApp = UnityEngine.Application;

namespace Game.Application.DataManager
{
	public class LocalJsonDataManager : IDataManager
	{
		private readonly string _dataFilePath;
		private readonly object _sync = new object();
		private Dictionary<string, JToken> _store = new Dictionary<string, JToken>(StringComparer.Ordinal);

		public LocalJsonDataManager(string fileName = "appdata.json")
		{
			_dataFilePath = Path.Combine(UnityApp.persistentDataPath, fileName);
			LoadFromDisk();
		}

		public bool CreateOrReplace<T>(string key, T value)
		{
			if (string.IsNullOrEmpty(key)) return false;
			lock (_sync)
			{
				_store[key] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
				return SaveToDisk();
			}
		}

		public bool TryRead<T>(string key, out T value)
		{
			value = default;
			if (string.IsNullOrEmpty(key)) return false;
			lock (_sync)
			{
				if (!_store.TryGetValue(key, out var token) || token == null)
					return false;
				try
				{
					if (typeof(T) == typeof(string))
					{
						object strVal = token.Type == JTokenType.Null ? string.Empty : token.ToString();
						value = (T)strVal;
					}
					else
					{
						value = token.Type == JTokenType.Null ? default : token.ToObject<T>();
					}
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		public bool Update<T>(string key, T value)
		{
			if (string.IsNullOrEmpty(key)) return false;
			lock (_sync)
			{
				if (!_store.ContainsKey(key)) return false;
				_store[key] = value != null ? JToken.FromObject(value) : JValue.CreateNull();
				return SaveToDisk();
			}
		}

		public bool Delete(string key)
		{
			if (string.IsNullOrEmpty(key)) return false;
			lock (_sync)
			{
				if (_store.Remove(key))
				{
					return SaveToDisk();
				}
				return false;
			}
		}

		public bool Exists(string key)
		{
			if (string.IsNullOrEmpty(key)) return false;
			lock (_sync)
			{
				return _store.ContainsKey(key);
			}
		}

		public void Clear()
		{
			lock (_sync)
			{
				_store.Clear();
				SaveToDisk();
			}
		}

		private void LoadFromDisk()
		{
			try
			{
				if (!File.Exists(_dataFilePath))
				{
					_store = new Dictionary<string, JToken>(StringComparer.Ordinal);
					return;
				}
				var json = File.ReadAllText(_dataFilePath);
				if (string.IsNullOrWhiteSpace(json))
				{
					_store = new Dictionary<string, JToken>(StringComparer.Ordinal);
					return;
				}
				var obj = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
				_store = obj ?? new Dictionary<string, JToken>(StringComparer.Ordinal);
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[LocalJsonDataManager] Load error: {e.Message}");
				_store = new Dictionary<string, JToken>(StringComparer.Ordinal);
			}
		}

		private bool SaveToDisk()
		{
			try
			{
				var dir = Path.GetDirectoryName(_dataFilePath);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				var json = JsonConvert.SerializeObject(_store, Formatting.None);
				File.WriteAllText(_dataFilePath, json);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[LocalJsonDataManager] Save error: {e.Message}");
				return false;
			}
		}
	}
}
#endif

