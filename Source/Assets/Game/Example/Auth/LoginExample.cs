#if UNITY_2019_1_OR_NEWER
using System.Threading.Tasks;
using Game.Application.Feature.Auth.Login;
using UnityEngine;

namespace Game.Example.Auth
{
	public class LoginExample : MonoBehaviour
	{
		[SerializeField] private string username = "admin";
		[SerializeField] private string password = "admin123";

		private async void Start()
		{
			await DoLogin();
		}

		public async Task DoLogin()
		{
			var model = new LoginRequestModel
			{
				username = username,
				password = password
			};

			var (errorCode, sessionId) = await LoginFeature.Instance.Execute(model);
			if (string.IsNullOrEmpty(errorCode) && !string.IsNullOrEmpty(sessionId))
			{
				Debug.Log($"[LoginExample] Login success. sessionId={sessionId}");
			}
			else
			{
				Debug.LogWarning($"[LoginExample] Login failed. error={errorCode}");
			}
		}
	}
}
#endif

