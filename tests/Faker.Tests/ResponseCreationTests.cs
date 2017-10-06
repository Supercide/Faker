using Faker.Core;
using FFakerTests;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Faker.Tests {
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

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(template.Response));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromObject_ThenCreatesResponse()
        {
            var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var expected = "{ \"Message\": \"Captured : 1, 2, 3\" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromObjectUsingIndexes_ThenCreatesResponse()
        {
            var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var expected = "{ \"Message\": \"Captured : 1, 2, 3\" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromNestedObject_ThenCreatesResponse()
        {
            var request = JsonConvert.SerializeObject(NestedObject.Create());
            var expected = "{ \"Message\": \"Captured : Elite, 1337, True, 2, Dam!, 1881, False, 5\" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{String}}, {{Number}}, {{Bool}}, {{Array[1]}}, {{Nested.String}}, {{Nested.Number}}, {{Nested.Bool}}, {{Nested.Array[1]}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromNestedObjectUsingIndexes_ThenCreatesResponse()
        {
            var request = JsonConvert.SerializeObject(NestedObject.Create());
            var expected = "{ \"Message\": \"Captured : Elite, 1, 2, 3, 1337, True\" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }


        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromArray_ThenCreatesResponse()
        {
            var request = "[ \"a\",\"b\",\"c\" ]";
            var expected = "{ \"Message\": \"Captured : a, b, c\" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WithRequestMissingToken_WhenCreatingResponseFromArray_ThenCreatesResponse_WithMissingTokenEmpty()
        {
            var request = "[ \"a\",\"b\",\"c\" ]";
            var expected = "{ \"Message\": \"Captured : a, b, c \" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}} {{3}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WithRequestMissingToken_WhenCreatingResponseFromObject_ThenCreatesResponse_WithMissingTokenEmpty()
        {
            var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var expected = "{ \"Message\": \"Captured : 1, 2, 3 \" }";
            Template template = new Template
            {
                Response = "{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}} {{d}}\" }"
            };

            ResponseFactory factory = new ResponseFactory();

            var response = factory.Create(JsonRequest.Create(request), template);

            Assert.That(response, Is.EqualTo(expected));
        }
    }
}