using System;
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

            templateMatcher.Match(string.Empty, expectedNamespace);

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

            var actualTemplate = templateMatcher.Match(content, new Uri("http://anything"));

            Assert.That(actualTemplate.Request, Is.EqualTo(expectedTemplate.Request));
        }

        [Test]
        public void GivenUnkownTemplate_WhenRetrievingTemplates_ThenReturnsNoTemplate()
        {
            string unkownContent = "{ \"fgh\": \"1\",\"fghf\": \"2\",\"rtr\": \"3\" }";
            var template = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }", properties: new[] { "a", "b", "c" });

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            mockTemplateStore.Setup(x => x.GetTemplates(It.IsAny<Uri>())).Returns(new[] { template });

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            var actualTemplate = templateMatcher.Match(unkownContent, new Uri("http://anything"));

            Assert.That(actualTemplate, Is.Null);
        }

        private static ITemplate CreateTemplate(string[] properties = null, string request = null, string response = null)
        {
            return new FakeTemplate
            {
                Response = response,
                Properties = properties,
                Request = request
            };
        }
    }
}
