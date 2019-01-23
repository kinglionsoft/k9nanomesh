using K9NanoMesh.Core;
using System.Collections.Generic;
using System.Reflection;

namespace K9NanoMesh.Remote.Test
{
    class TestApiInfo : IApiInfo
    {
        public string AuthenticationAuthority => "http://127.0.0.1:5000";

        public string Title => "Sample Api";

        public string Version => "v1";

        public Assembly ApplicationAssembly => GetType().Assembly;

        public IDictionary<string, string> Scopes => new Dictionary<string, string>
        {
            {"api1", Title}
        };

        public SwaggerAuthInfo SwaggerAuthInfo => new SwaggerAuthInfo(
            "echoapiswaggerui", "", ""
        );


        public string ApiName => "api1";

        public string ApiSecret => "secret";

        public string BindAddress { get; set; } = "localhost";

        public int BindPort { get; set; } = new System.Random().Next(10000, 20000);
    }
}
