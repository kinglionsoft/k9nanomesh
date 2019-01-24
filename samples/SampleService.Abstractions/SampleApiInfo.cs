using System.Collections.Generic;
using System.Reflection;
using K9NanoMesh.Core;

namespace SampleService.Abstractions
{
    public class SampleApiInfo: IApiInfo
    {
        public string AuthenticationAuthority => "http://127.0.0.1:5000";

        public string Title => "Sample Api";

        public string Version => "v1";
        
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

        public int BindPort { get; set; } = 9901;
    }
}