using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Tests {
    public class FakeTemplate : ITemplate
    {
        public IList<string> GetProperties()
        {
            return Properties;
        }
        public string Request { get; set; }
        public string[] Properties { get; set; }
        public string Response { get; set; }
    }
}