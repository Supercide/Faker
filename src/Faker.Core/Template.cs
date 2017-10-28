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

        public IReadOnlyDictionary<string, string> Properties { get; protected set; }

        public IReadOnlyDictionary<string, string> Metadata { get; protected set; }

        public string Response { get; protected set; }

    }
}