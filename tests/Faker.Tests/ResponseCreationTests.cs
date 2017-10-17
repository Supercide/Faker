using Faker.Core;
using Faker.Core.Extensions;
using Moq;
using NUnit.Framework;

namespace Faker.Tests
{
    public class ResponseCreationTests
    {
        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromObject_ThenCreatesExpectedResponse()
        {
            var mockRequest = CreateMockRequestObject();

            var mockTemplate = new Mock<ITemplate>();

            mockTemplate.Setup(x => x.Response)
                        .Returns("{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}}\" }");

            var expectedResponse = "{ \"Message\": \"Captured : 1, 2, 3\" }";

            var factory = new ResponseFactory();

            var response = factory.Create(mockRequest.Object, mockTemplate.Object);

            Assert.That(expectedResponse, Is.EqualTo(response));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromObjectUsingIndexes_ThenCreatesResponse()
        {
            var mockRequest = CreateMockRequestArray();

            var expected = "{ \"Message\": \"Captured : b, c, a\" }";

            var mockTemplate = new Mock<ITemplate>();

            mockTemplate.Setup(x => x.Response)
                        .Returns("{ \"Message\": \"Captured : {{1}}, {{2}}, {{0}}\" }");

            var factory = new ResponseFactory();

            var response = factory.Create(mockRequest.Object, mockTemplate.Object);

            Assert.That(response, Is.EqualTo(expected));
        }

        [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromNestedObject_ThenCreatesResponse()
        {
            var mockRequest = CreateNestedMockRequestObject();

            var mockTemplate = new Mock<ITemplate>();

            mockTemplate.Setup(x => x.Response)
                        .Returns("{ \"Message\": \"Captured : {{String}}, {{Number}}, {{Bool}}, {{Array[1]}}, {{Nested.String}}, {{Nested.Number}}, {{Nested.Bool}}, {{Nested.Array[1]}}\" }");

            var factory = new ResponseFactory();

            var response = factory.Create(mockRequest.Object, mockTemplate.Object);

            var expected = "{ \"Message\": \"Captured : String-0, Number-0, Bool-0, Array-0, Nested-String-0, Nested-Number-0, Nested-Bool-0, Nested-Array-0\" }";

            Assert.That(response, Is.EqualTo(expected));
        }

        private static Mock<IRequest> CreateNestedMockRequestObject()
        {
            var mockRequest = new Mock<IRequest>();

            mockRequest.Setup(x => x.GetPropertyValueBy("String"))
                       .Returns("String-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Number"))
                       .Returns("Number-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Bool"))
                       .Returns("Bool-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Array[1]"))
                       .Returns("Array-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Nested.String"))
                       .Returns("Nested-String-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Nested.Number"))
                       .Returns("Nested-Number-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Nested.Bool"))
                       .Returns("Nested-Bool-0");
            mockRequest.Setup(x => x.GetPropertyValueBy("Nested.Array[1]"))
                       .Returns("Nested-Array-0");
            return mockRequest;
        }

        private static Mock<IRequest> CreateMockRequestObject()
        {
            var mockRequest = new Mock<IRequest>();

            mockRequest.Setup(x => x.GetPropertyValueBy("a"))
                       .Returns("1");
            mockRequest.Setup(x => x.GetPropertyValueBy("b"))
                       .Returns("2");
            mockRequest.Setup(x => x.GetPropertyValueBy("c"))
                       .Returns("3");
            return mockRequest;
        }

        private static Mock<IRequest> CreateMockRequestArray()
        {
            var mockRequest = new Mock<IRequest>();

            mockRequest.Setup(x => x.GetPropertyValueBy(0))
                       .Returns("a");
            mockRequest.Setup(x => x.GetPropertyValueBy(1))
                       .Returns("b");
            mockRequest.Setup(x => x.GetPropertyValueBy(2))
                       .Returns("c");
            return mockRequest;
        }
    }
}