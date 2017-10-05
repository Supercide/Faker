using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class ResponseFactory
    {
        public string Create(string request, Template template)
        {
            var obj = JObject.Parse(request);
            var properties = obj.Properties().Select(property => property.Name).ToList();
            Regex regex = new Regex(@"\{\{(\S+)\}\}");
            var matches = regex.Matches(template.Response);
            string response = template.Response;
            foreach (Match match in matches)
            {
                var propertyName = match.Groups[1].Value;

                var replacement = string.Empty;

                if (properties.Contains(propertyName))
                {
                    var token = obj[propertyName];

                    replacement = token.Value<string>();
                }

                response = response.Replace(match.Value, replacement);


            }
            
            return response;
        }
    }
}