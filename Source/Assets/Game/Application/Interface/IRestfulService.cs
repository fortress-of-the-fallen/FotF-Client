using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Application.Interface
{
    public interface IRestfulService
    {
        Task<(bool, HttpStatusCode)> Delete(
            string url,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false);

        Task<(TResult, HttpStatusCode)> Get<TResult>(
            string url,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class;

        Task<(TResult, HttpStatusCode)> Patch<TResult>(
            string url,
            object rawData,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class;

        Task<(TResult, HttpStatusCode)> Post<TResult>(
            string url,
            object rawData,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class;

        Task<(TResult, HttpStatusCode)> Post<TResult>(
            string url,
            Dictionary<string, object> formFields = null,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class;

        Task<(TResult, HttpStatusCode)> Put<TResult>(
            string url,
            object rawData,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class;

        Task<(TResult, HttpStatusCode)> Put<TResult>(
            string url,
            Dictionary<string, object> formFields = null,
            Dictionary<string, string> headers = null,
            int timeout = 30,
            CancellationToken token = default,
            bool force = false
        ) where TResult : class;
    }
}

