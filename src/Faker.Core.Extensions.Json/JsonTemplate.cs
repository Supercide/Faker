using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Faker.Core.Extensions.Json {
    public class JsonTemplate : ITemplate
    {
        public string Request { get; set; }
        public string Response { get; set; }

        public IList<string> GetProperties()
        {
            return JObject.Parse(Request)
                          .Properties()
                          .Select(property => property.Name)
                          .ToList();
        }
    }
}