using FakR.Core;
using NUnit.Framework;

namespace FakR.Tests {
    public class ResponseCreationTests
    {
        [Test]
        public void GivenRequest_WhenCreatingResponseFromStaticTemplate_ThenCreatesResponse()
        {
            var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";

            Template template = new Template
            {
                Response = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(request, template);

            Assert.That(response, Is.EqualTo(template.Response));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponse_ThenCreatesResponse()
        {
            var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var expected = "{ \"Message\": \"Captured : 1, 2, 3\" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(request, template);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WithRequestMissingToken_WhenCreatingResponse_ThenCreatesResponse_WithMissingTokenEmpty()
        {
            var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var expected = "{ \"Message\": \"Captured : 1, 2, 3 \" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}} {{d}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(request, template);

            Assert.That(response, Is.EqualTo(expected));
        }
    }
}