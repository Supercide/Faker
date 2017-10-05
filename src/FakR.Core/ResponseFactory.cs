using FakR.Core;

namespace FakR.Tests {
    public class ResponseFactory
    {
        public string Create(Template template)
        {
            return template.Response;
        }
    }
}