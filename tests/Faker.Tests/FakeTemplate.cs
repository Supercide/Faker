using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Tests {
    public class FakeTemplate : ITemplate
    {
        public IList<string> GetProperties()
        {
            return Properties;
        }
        public IRequest Request { get; set; }
        public string[] Properties { get; set; }
        public IResponse Response { get; set; }
    }
}