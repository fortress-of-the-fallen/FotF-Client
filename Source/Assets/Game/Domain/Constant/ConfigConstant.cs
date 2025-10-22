using System;

namespace Game.Domain.Constant
{
    public static class ConfigConstant
    {
        public static readonly string Env = "Production";
        public static readonly string Url;
        public static readonly string TodoTestUrl;
        public static readonly string PostTestUrl;
        public static readonly string DelayTestUrl;

        static ConfigConstant()
        {
            // var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Url = Env == "Development" ? "http://localhost:3000" : "https://preferably-upright-walleye.ngrok-free.app";
            TodoTestUrl = "https://jsonplaceholder.typicode.com/todos/1";
            PostTestUrl = "https://httpbin.org/post";
            DelayTestUrl = "https://httpbin.org/delay/5";
        }
    }
}
