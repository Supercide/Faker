using System.Collections.Generic;

namespace Faker.Core {
    public class Response
    {
        public string Content { get; set; }

        public IReadOnlyDictionary<string, string> Metadata { get; set; }
    }
}