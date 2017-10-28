using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Core {
    public class Template : ITemplate
    {
        public Template(Dictionary<string, string> metadata, Dictionary<string, string> properties, string response)
        {
            Metadata = metadata;
            Properties = properties;
            Response = response;
        }

        public Dictionary<string, string> Properties { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public string Response { get; set; }

    }
}