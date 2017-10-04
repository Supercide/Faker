using System;
using System.Security.Cryptography;
using System.Text;
using FakR.Core;
using Moq;
using NUnit.Framework;

namespace FakR.Tests
{
    public class TemplateTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void GivenKnownTemplate_WhenRetrievingResponses_ThenCallsTemplateStoreWithNameSpace()
        {
            Uri expectedNamespace = new Uri("http://localhost/anyvalue");

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            templateMatcher.Match(string.Empty, expectedNamespace);

            mockTemplateStore.Verify(x => x.GetTemplate(It.IsAny<string>(), It.Is<Uri>(@namespace => @namespace == expectedNamespace)), Times.Once);
        }

        // dynamic templates
        [Test]
        public void GivenKnownTemplate_WhenRetrievingResponses_ThenCallsTemplateWithContent()
        {
            string expectedContent = "{ \"Message:\" \"Hello world\" }";

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            templateMatcher.Match(expectedContent, new Uri("http://anything"));

            mockTemplateStore.Verify(x => x.GetTemplate(It.Is<string>(content => content == expectedContent), It.IsAny<Uri>()), Times.Once);
        }
        // matching
    }
}
