using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task GivenKnownContent_WhenRetrievingTemplates_ThenCallsTemplateStoreWithNameSpace()
        {
            Uri expectedNamespace = new Uri("http://localhost/anyvalue");

            Mock<IRequest> mockrequest = new Mock<IRequest>();
            mockrequest.Setup(x => x.GetProperties()).Returns(new string[0]);

            Mock<ITemplateStore<ITemplate>> mockTemplateStore = new Mock<ITemplateStore<ITemplate>>();

            TemplateMatcher<ITemplate> templateMatcher = new TemplateMatcher<ITemplate>(mockTemplateStore.Object);

            await templateMatcher.MatchAsync(expectedNamespace, mockrequest.Object, CancellationToken.None);

            mockTemplateStore.Verify(x => x.GetTemplateContainerAsync(It.Is<Uri>(@namespace => @namespace == expectedNamespace), CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task GivenMultipleTemplates_WhenRetrievingTemplates_ThenReturnsTemplateWithMostMatchedFields()
        {
            Mock<IRequest> mockrequest = new Mock<IRequest>();
            mockrequest.Setup(x => x.GetProperties()).Returns(new[] { "a", "b", "c" });

            var templateOne = CreateTemplate(response:"{ \"a\":\"1\" }", properties:new []{"a"});
            var templateTwo = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\" }", properties: new[] { "a", "b" });
            var templateThree = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\", \"d\": \"4\" }", properties: new[] { "a", "b", "c", "d" });
            var expectedTemplate = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }", properties: new[] { "a", "b", "c" });

            Mock<ITemplateStore<ITemplate>> mockTemplateStore = new Mock<ITemplateStore<ITemplate>>();
            Mock<ITemplateContainer<ITemplate>> mockTemplateContainer = new Mock<ITemplateContainer<ITemplate>>();
            mockTemplateContainer.Setup(x => x.Templates).Returns(new[] { templateOne, templateTwo, templateThree, expectedTemplate }.ToList());

            mockTemplateStore.Setup(x => x.GetTemplateContainerAsync(It.IsAny<Uri>(), CancellationToken.None))
                             .ReturnsAsync(mockTemplateContainer.Object);

            TemplateMatcher<ITemplate> templateMatcher = new TemplateMatcher<ITemplate>(mockTemplateStore.Object);

            var actualTemplate = await templateMatcher.MatchAsync(new Uri("http://anything"), mockrequest.Object, CancellationToken.None);

            Assert.That(actualTemplate, Is.EqualTo(expectedTemplate));
        }

        [Test]
        public async Task GivenMultipleTemplates_WhenRetrievingTemplates_ThenReturnsTemplateWithMatchingMetaData()
        {
            Mock<IRequest> mockrequest = new Mock<IRequest>();
            mockrequest.Setup(x => x.GetProperties()).Returns(new[] { "a", "b", "c" });
            mockrequest.Setup(x => x.Metadata).Returns(new Dictionary<string, string>
                                                       {
                                                           {"Method", "POST"}
                                                       });

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

            Mock<ITemplateStore<ITemplate>> mockTemplateStore = new Mock<ITemplateStore<ITemplate>>();
            Mock<ITemplateContainer<ITemplate>> mockTemplateContainer = new Mock<ITemplateContainer<ITemplate>>();
            mockTemplateContainer.Setup(x => x.Templates).Returns(new[] { expectedTemplate, template }.ToList());
            mockTemplateStore.Setup(x => x.GetTemplateContainerAsync(It.IsAny<Uri>(), CancellationToken.None)).ReturnsAsync(mockTemplateContainer.Object);

            TemplateMatcher<ITemplate> templateMatcher = new TemplateMatcher<ITemplate>(mockTemplateStore.Object);

            var actualTemplate = await templateMatcher.MatchAsync(new Uri("http://anything"), mockrequest.Object, CancellationToken.None);

            Assert.That(actualTemplate, Is.EqualTo(expectedTemplate));
        }

        [Test]
        public async Task GivenUnkownTemplate_WhenRetrievingTemplates_ThenReturnsNoTemplate()
        {
            Mock<IRequest> mockrequest = new Mock<IRequest>();
            mockrequest.Setup(x => x.GetProperties()).Returns(new[] { "fgh", "fghf", "rtr" });

            var template = CreateTemplate(response: "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }", properties: new[] { "a", "b", "c" });

            Mock<ITemplateStore<ITemplate>> mockTemplateStore = new Mock<ITemplateStore<ITemplate>>();
            Mock<ITemplateContainer<ITemplate>> mockTemplateContainer = new Mock<ITemplateContainer<ITemplate>>();
            mockTemplateContainer.Setup(x => x.Templates).Returns(new[] { template }.ToList());
            mockTemplateStore.Setup(x => x.GetTemplateContainerAsync(It.IsAny<Uri>(), CancellationToken.None)).ReturnsAsync(mockTemplateContainer.Object);

            TemplateMatcher<ITemplate> templateMatcher = new TemplateMatcher<ITemplate>(mockTemplateStore.Object);

            var actualTemplate = await templateMatcher.MatchAsync(new Uri("http://anything"), mockrequest.Object, CancellationToken.None);

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
            
            Mock<IResponse> mockResponse = new Mock<IResponse>();
            mockResponse.Setup(x => x.Content)
                        .Returns(response);
            Mock<ITemplate> mockTemplate = new Mock<ITemplate>();

            mockTemplate.Setup(x => x.Metadata)
                        .Returns(metadata);

            mockTemplate.Setup(x => x.Properties)
                        .Returns(properties?.ToDictionary(x => x, x => string.Empty));

            mockTemplate.Setup(x => x.Response)
                        .Returns(response);

            return mockTemplate.Object;
        }
    }
}
