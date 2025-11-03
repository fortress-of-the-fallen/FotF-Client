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

        private static readonly string _devUrl = "https://preferably-upright-walleye.ngrok-free.app";
        private static readonly string _prodUrl = "https://preferably-upright-walleye.ngrok-free.app";

        static ConfigConstant()
        {
            Url = Env == "Development" ? _devUrl : _prodUrl;
            TodoTestUrl = "https://jsonplaceholder.typicode.com/todos/1";
            PostTestUrl = "https://httpbin.org/post";
            DelayTestUrl = "https://httpbin.org/delay/5";
        }
    }
}
