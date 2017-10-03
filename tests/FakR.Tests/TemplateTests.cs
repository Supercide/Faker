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
        public void GivenKnownTemplate_WhenMatchingStaticContent_ThenCallsTemplateStoreWithBase64Hash()
        {
            string content = "{ \"Message:\" \"Hello world\" }";

            SHA1 sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            string expectedHash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(content)));

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            templateMatcher.Match(content);

            mockTemplateStore.Verify(x => x.GetTemplate(It.Is<string>(hash => hash == expectedHash)), Times.Once);
        }
    }
}
