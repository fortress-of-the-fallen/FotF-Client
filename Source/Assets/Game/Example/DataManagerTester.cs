#if UNITY_2019_1_OR_NEWER
using Game.Application.DataManager;
using Game.Application.Interface;
using UnityEngine;

namespace Game.Example
{
	public class DataManagerTester : MonoBehaviour
	{
		private IDataManager _dataManager;

		private void Awake()
		{
			_dataManager = new LocalJsonDataManager();
		}

		private void Start()
		{
			// Create/replace sessionId
			var created = _dataManager.CreateOrReplace("sessionId", System.Guid.NewGuid().ToString("N"));
			Debug.Log($"CreateOrReplace sessionId -> {created}");

			// Read sessionId
			if (_dataManager.TryRead<string>("sessionId", out var session))
			{
				Debug.Log($"Read sessionId: {session}");
			}

			// Update sessionId
			var updated = _dataManager.Update("sessionId", "fixed-session-id-123");
			Debug.Log($"Update sessionId -> {updated}");

			// Delete sessionId
			var deleted = _dataManager.Delete("sessionId");
			Debug.Log($"Delete sessionId -> {deleted}");
		}
	}
}
#endif

