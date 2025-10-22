using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Application.Http;
using Game.Domain.Constant;
using Newtonsoft.Json;
using UnityEngine;

public class RestfulServiceTester : MonoBehaviour
{
    [Serializable]
    public class Todo { public int userId; public int id; public string title; public bool completed; }

    private RestfulService _service;

    private async void Start()
    {
        _service = new RestfulService();

        // 1) Test GET JSON
        var (todo, status1) =
            await _service.Get<Todo>(ConfigConstant.TodoTestUrl);
        Debug.Log($"GET status: {status1} | data: {JsonConvert.SerializeObject(todo)}");

        // 2) Test POST JSON
        var postBody = new { title = "foo", body = "bar", userId = 1 };
        var (postResult, status2) =
            await _service.Post<string>(ConfigConstant.PostTestUrl, postBody);
        Debug.Log($"POST status: {status2} | body: {Truncate(postResult, 200)}");

        // 3) Test timeout/cancel
        using (var cts = new CancellationTokenSource())
        {
            try
            {
                // httpbin delay 5s, set timeout 1s to surface a RequestTimeout
                var (t, status3) = await _service.Get<string>(
                    ConfigConstant.DelayTestUrl, timeout: 1, token: cts.Token);
                Debug.Log($"Timeout test status: {status3}");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    private string Truncate(string s, int max) =>
        string.IsNullOrEmpty(s) ? s : (s.Length <= max ? s : s.Substring(0, max) + "...");

}
