#if UNITY_2019_1_OR_NEWER
using System.Net;
using System.Threading.Tasks;
using Game.Application.Interface;
using Game.Application.Http;
using Game.Application.Base;
using Game.Application.DataManager;
using Game.Domain.Constant;

namespace Game.Application.Feature.Auth.Login
{
	public class LoginFeature : Singleton<LoginFeature>, IFeature<LoginRequestModel, (string, string)>
	{
		private readonly IRestfulService _restfulService = RestfulService.Instance;
		private const string AuthLoginPath = "/v1/auth/login";
		private const string SessionIdKey = "sessionId";


		public LoginFeature() { }

		public async Task<(string, string)> Execute(LoginRequestModel input)
		{
			var url = ConfigConstant.Url + AuthLoginPath;
			var body = new
			{
				username = input.username,
				password = input.password,
				rememberMe = true,
                connectionId = "abcd"
			};

			var (resp, status) = await _restfulService.Post<ApiResponse<string>>(url, body);
			var isHttpSuccess = (int)status >= 200 && (int)status < 300;
			if (isHttpSuccess && resp != null && resp.success)
			{
				if (!string.IsNullOrEmpty(resp.result))
				{
					LocalJsonDataManager.Instance.CreateOrReplace(SessionIdKey, resp.result);
				}
				return (string.Empty, resp.result ?? string.Empty);
			}

			var errorCode = resp != null && !string.IsNullOrEmpty(resp.errorCode)
				? resp.errorCode
				: status.ToString();
			return (errorCode, string.Empty);
		}
	}
}
#endif

