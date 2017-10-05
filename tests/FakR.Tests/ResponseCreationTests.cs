using FakR.Core;
using NUnit.Framework;

namespace FakR.Tests {
    public class ResponseCreationTests
    {
        [Test]
        public void GivenTemplate_WhenCreatingResponse_ThenCreatesResponse()
        {
            Template template = new Template
            {
                Request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }",
                Response = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(template);

            Assert.That(response, Is.EqualTo(template.Response));

        }
    }
}