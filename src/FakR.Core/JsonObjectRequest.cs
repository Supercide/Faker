using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class JsonObjectRequest : JsonRequest
    {
        private readonly Dictionary<string, string> _objectDictionary;

        public JsonObjectRequest(JObject jObject)
        {
            _objectDictionary = jObject.Cast<KeyValuePair<string, JToken>>()
                                       .SelectMany(property => WalkNode(property.Value))
                                       .ToDictionary(property => property.Key, property => property.Value);
        }

        static IEnumerable<KeyValuePair<string, string>> WalkNode(JToken node)
        {
            List<KeyValuePair<string, string>> paths = new List<KeyValuePair<string, string>>();

            switch (node.Type)
            {
                case JTokenType.Object:
                    paths.AddRange(WalkObject(node));
                    break;

                case JTokenType.Array:
                    paths.AddRange(WalkArray(node));
                    break;

                default:
                    paths.Add(new KeyValuePair<string, string>(node.Path, node.Value<string>()));
                    break;
            }

            return paths;
        }

        private static IEnumerable<KeyValuePair<string, string>> WalkArray(JToken node) => node.Children()
                                                                                               .SelectMany(WalkNode);

        private static IEnumerable<KeyValuePair<string, string>> WalkObject(JToken node) => node.Children<JProperty>()
                                                                                                .SelectMany(property => WalkNode(property.Value));

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