using Newtonsoft.Json.Linq;

namespace Faker.Core.Extensions.Json {
    public class JsonArrayRequest : JsonRequest
    {
        private readonly JArray _jArray;

        public JsonArrayRequest(JArray jArray)
        {
            _jArray = jArray;
        }

        public override string GetPropertyValueBy(string path)
        {
            return GetPropertyValueBy(int.Parse(path));
        }

        public override string GetPropertyValueBy(int index)
        {
            string value = null;

            if(index < _jArray.Count)
            {
                value = _jArray[index].Value<string>();
            }
            
            return value;
        }
    }
}