using System;
using System.Security.Cryptography;
using System.Text;
using FakR.Core;
using Moq;
using NUnit.Framework;

namespace FakR.Tests
{
    [TestFixture]
    public class TemplateTests
    {
        [Test]
        public void GivenKnownTemplate_WhenRetrievingTemplates_ThenCallsTemplateStoreWithNameSpace()
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
            string templateOne = "{ \"a\":\"1\" }";
            string templateTwo = "{ \"a\": \"1\",\"b\": \"2\" }";
            string expectedTemplate = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            string templateThree = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\", \"d\": \"4\" }";

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();
            mockTemplateStore.Setup(x => x.GetTemplates(It.IsAny<Uri>())).Returns(new string[] {templateOne, templateTwo, templateThree, expectedTemplate });

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            string actualTemplate = templateMatcher.Match(content, new Uri("http://anything"));

            Assert.That(actualTemplate, Is.EqualTo(expectedTemplate));
        }
    }
}
