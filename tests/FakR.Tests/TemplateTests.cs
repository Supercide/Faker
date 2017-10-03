using System;
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
        public void GivenKnownTemplate_WhenMatchingStaticContent_ThenReturnsTemplate()
        {
            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

        }
    }
}
