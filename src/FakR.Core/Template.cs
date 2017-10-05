using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class Template
    {
        public string Incoming { get; set; }
        private string Outgoing { get; set; }

        public IList<string> GetProperties()
        {
            return JObject.Parse(Incoming)
                          .Properties()
                          .Select(property => property.Name)
                          .ToList();
        }

    }
}