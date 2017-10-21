using System;
using System.Collections.Generic;
using Faker.Core;
using Faker.Core.Extensions;
using Moq;
using NUnit.Framework;

namespace Faker.Tests
{
    [TestFixture]
    public class TemplateMatcherTests
    {
        [Test]
        public void GivenKnownContent_WhenRetrievingTemplates_ThenCallsTemplateStoreWithNameSpace()
        {
            Uri expectedNamespace = new Uri("http://localhost/anyvalue");

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            templateMatcher.Match(expectedNamespace, string.Empty, new Dictionary<string, string>());

            mockTemplateStore.Verify(x => x.GetTemplates(It.Is<Uri>(@namespace => @namespace == expectedNamespace)), Times.Once);
        }

        [Test]
        public void GivenMultipleTemplates_WhenRetrievingTemplates_ThenReturnsTemplateWithMostMatchedFields()
        {
            string content = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var templateOne = CreateTemplate(response:"{ \"a\":\"1\" }", properties:new []{"a"});
            var templateTwo = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\" }", properties: new[] { "a", "b" });
            var templateThree = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\", \"d\": \"4\" }", properties: new[] { "a", "b", "c", "d" });
            var expectedTemplate = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }", properties: new[] { "a", "b", "c" });

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            mockTemplateStore.Setup(x => x.GetTemplates(It.IsAny<Uri>())).Returns(new [] {templateOne, templateTwo, templateThree, expectedTemplate });

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            var actualTemplate = templateMatcher.Match(new Uri("http://anything"), content, new Dictionary<string, string>());

            Assert.That(actualTemplate, Is.EqualTo(expectedTemplate));
        }

        [Test]
        public void GivenMultipleTemplates_WhenRetrievingTemplates_ThenReturnsTemplateWithMatchingMetaData()
        {
            string content = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";

            var template = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }", 
                                                  properties: new[] { "a", "b", "c" }, 
                                                  metadata: new Dictionary<string, string>()
                                                  {
                                                      {"Method", "GET"},
                                                      {"Content", "xml"},
                                                  });

            var expectedTemplate = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }",
                                                  properties: new[] { "a", "b", "c" },
                                                  metadata: new Dictionary<string, string>()
                                                  {
                                                      {"Method", "POST"},
                                                      {"Content", "json"},
                                                  });

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            mockTemplateStore.Setup(x => x.GetTemplates(It.IsAny<Uri>())).Returns(new[] { expectedTemplate, template });

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            var actualTemplate = templateMatcher.Match(new Uri("http://anything"), 
                                                       content, 
                                                       new Dictionary<string, string>
                                                       {
                                                           {"Method", "POST"}
                                                       });

            Assert.That(actualTemplate, Is.EqualTo(expectedTemplate));
        }

        [Test]
        public void GivenUnkownTemplate_WhenRetrievingTemplates_ThenReturnsNoTemplate()
        {
            string unkownContent = "{ \"fgh\": \"1\",\"fghf\": \"2\",\"rtr\": \"3\" }";
            var template = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }", properties: new[] { "a", "b", "c" });

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            mockTemplateStore.Setup(x => x.GetTemplates(It.IsAny<Uri>())).Returns(new[] { template });

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            var actualTemplate = templateMatcher.Match(new Uri("http://anything"), unkownContent, new Dictionary<string, string>());

            Assert.That(actualTemplate, Is.Null);
        }

        private static ITemplate CreateTemplate(string[] properties = null, string response = null, Dictionary<string, string> metadata = null)
        {
            Mock<IRequest> mockRequest = new Mock<IRequest>();

            if (properties != null)
            {
                mockRequest.Setup(x => x.GetProperties())
                           .Returns(properties);

                mockRequest.Setup(x => x.Metadata)
                           .Returns(metadata??new Dictionary<string, string>());
            }
            

            return new FakeTemplate
            {
                Response = response,
                Properties = properties,
                Request = mockRequest.Object
            };
        }
    }
}
