using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class Template
    {
        public string Request { get; set; }
        private string Outgoing { get; set; }
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