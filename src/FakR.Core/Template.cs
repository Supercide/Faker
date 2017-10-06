using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class Template
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

        public string MergeFields(IEnumerable<MergeField> mergeFields)
        {
            string template = Response;

            foreach (MergeField mergeToken in mergeFields)
            {
                template = template.Replace(mergeToken.Token, mergeToken.Value);
            }

            return template;
        }
    }
}