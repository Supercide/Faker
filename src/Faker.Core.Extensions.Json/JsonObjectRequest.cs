using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Faker.Core.Extensions.Json {
    public class JsonObjectRequest : JsonRequest
    {
        private readonly Dictionary<string, string> _objectDictionary;

        public JsonObjectRequest(JObject jObject)
        {
            _objectDictionary = jObject.Cast<KeyValuePair<string, JToken>>()
                                       .SelectMany(property => WalkNode(property.Value))
                                       .ToDictionary(property => property.Key, property => property.Value);
        }

        private static IEnumerable<KeyValuePair<string, string>> WalkNode(JToken node)
        {
            var paths = WalkObject(node).Union(WalkArray(node))
                                        .ToArray();

            return paths.Any()
                        ? paths
                        : ReturnCurrentPath(node);
        }

        private static KeyValuePair<string, string>[] ReturnCurrentPath(JToken node)
        {
            return new[] {new KeyValuePair<string, string>(node.Path, node.Value<string>())};
        }

        private static IEnumerable<KeyValuePair<string, string>> WalkArray(JToken node)
        {
            var paths = Enumerable.Empty<KeyValuePair<string, string>>();

            if (node.Type == JTokenType.Array)
            {
                paths = node.Children()
                            .SelectMany(WalkNode);
            }

            return paths;
        }

        private static IEnumerable<KeyValuePair<string, string>> WalkObject(JToken node)
        {
            var paths = Enumerable.Empty<KeyValuePair<string, string>>();

            if (node.Type == JTokenType.Object)
            {
                paths = node.Children<JProperty>()
                           .SelectMany(property => WalkNode(property.Value));
            }

            return paths;
        }

        public override string GetPropertyValueBy(string path)
        {
            string value = null;

            if(int.TryParse(path, out var index))
            {
                value = GetPropertyValueBy(index);

            } else if(_objectDictionary.ContainsKey(path))
            {
                value = _objectDictionary[path];

            }

            return value;
        }

        public override string GetPropertyValueBy(int index)
        {
            if (index < _objectDictionary.Count)
            {
                return _objectDictionary.ToArray()[index].Value;
            }

            return null;
        }
    }
}