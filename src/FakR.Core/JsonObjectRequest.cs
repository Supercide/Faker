using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class JsonObjectRequest : JsonRequest
    {
        private readonly JObject _jObject;
        private readonly IList<string> _properties;

        public JsonObjectRequest(JObject jObject)
        {
            _jObject = jObject;

            _properties = jObject.Properties()
                                 .Select(property => property.Name)
                                 .ToList();
        }

        public override string GetPropertyBy(string path)
        {
            if(!int.TryParse(path, out var index))
            {
                return _jObject[path].Value<string>();
            }

            return GetPropertyBy(index);
        }

        public override string GetPropertyBy(int index)
        {
            if (index < _jObject.Count)
            {
                return _jObject[_properties[index]].Value<string>();
            }

            return null;
        }
    }
}