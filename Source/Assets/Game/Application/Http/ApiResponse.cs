#if UNITY_2019_1_OR_NEWER
namespace Game.Application.Http
{
	public class ApiResponse<T>
	{
		public string timestamp { get; set; }

		public string errorCode { get; set; }

		public string error { get; set; }

		public bool success { get; set; }
        
		public T result { get; set; }
	}
}
#endif

