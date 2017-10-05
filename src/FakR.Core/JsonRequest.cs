using System;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public abstract class JsonRequest : IRequest
    {
        public static JsonRequest Create(string json)
        {
            var container = JToken.Parse(json);

            switch (container.Type)
            {
                case JTokenType.Array:
                    return new JsonArrayRequest(JArray.Parse(json));
                case JTokenType.Object:
                    return new JsonObjectRequest(JObject.Parse(json));
                default:
                    throw new ArgumentException("invalid json type", nameof(json));
            }
        }

        public abstract string GetPropertyBy(string path);
        public abstract string GetPropertyBy(int index);
    }
}