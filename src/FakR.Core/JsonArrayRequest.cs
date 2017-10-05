using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class JsonArrayRequest : JsonRequest
    {
        private readonly JArray _jArray;

        public JsonArrayRequest(JArray jArray)
        {
            _jArray = jArray;
        }

        public override string GetPropertyBy(string path)
        {
            return GetPropertyBy(int.Parse(path));
        }

        public override string GetPropertyBy(int index)
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