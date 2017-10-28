using System;
using System.Collections.Generic;
using System.Linq;
using Faker.Core.Extensions;

namespace Faker.Core
{
    public class TemplateMatcher
    {
        private readonly ITemplateStore _templateStore;

        public TemplateMatcher(ITemplateStore templateStore)
        {
            _templateStore = templateStore;
        }

        public ITemplate Match(Uri @namespace, IRequest request)
        {
            var templateContainer = _templateStore.GetTemplateContainer(@namespace);

            return templateContainer?.Templates?.Any()??false
                ? GetClosestMatchingTemplate(templateContainer.Templates, request)?.Template
                : null;
        }

        private static TemplatePropertyMatchResult GetClosestMatchingTemplate(IEnumerable<ITemplate> templates, IRequest request)
        {
            IList<TemplatePropertyMatchResult> results = FindTemplatesWithMostMatchingProperties(templates, request);

            return results.Count > 1
                ? FindTemplatesWithLeastAmountOfdifferences(results).First()
                : results.FirstOrDefault();
        }

        private static IEnumerable<TemplatePropertyMatchResult> FindTemplatesWithLeastAmountOfdifferences(IList<TemplatePropertyMatchResult> results)
        {
            var minimumProperties = results.Min(result => result.TemplateProperties.Count());

            return results.Where(result => result.TemplateProperties.Count() == minimumProperties);
        }

        private static IList<TemplatePropertyMatchResult> FindTemplatesWithMostMatchingProperties(IEnumerable<ITemplate> templates, IRequest request)
        {
            IEnumerable<TemplatePropertyMatchResult> matchingTemplates = FindAllTemplatesWithMatchingProperties(templates, request);

            if (!matchingTemplates.Any()) return new List<TemplatePropertyMatchResult>();

            int maxMatches = matchingTemplates.Max(x => x.PropertyMatchCount);

            return matchingTemplates.Where(template => template.PropertyMatchCount == maxMatches).ToList();
        }

        private static IList<TemplatePropertyMatchResult> FindAllTemplatesWithMatchingProperties(IEnumerable<ITemplate> templates, IRequest request)
        {
            return templates.Select(template => CreateTemplateMatchResult(request, template))
                            .Where(result => result.PropertyMatchCount > 0)
                            .Where(result => FilterMetadata(result, request.Metadata))
                            .ToList();
        }

        private static bool FilterMetadata(TemplatePropertyMatchResult result, IReadOnlyDictionary<string, string> metadata)
        {
            return result.Template
                         .Metadata?
                         .All(kvp => !metadata.ContainsKey(kvp.Key) || metadata[kvp.Key] == kvp.Value)??true;
        }

        private static TemplatePropertyMatchResult CreateTemplateMatchResult(IRequest request, ITemplate template)
        {
            IEnumerable<string> requestProperties = request.GetProperties();

            IEnumerable<string> templateProperties = template.Properties.Keys;

            return new TemplatePropertyMatchResult
            {
                Template = template,
                PropertyMatchCount = GetPropertyMatchCount(requestProperties, templateProperties),
                TemplateProperties = templateProperties
            };
        }

        private static int GetPropertyMatchCount(IEnumerable<string> propertiesLeft, IEnumerable<string> propertiesRight)
        {
            return propertiesLeft.Count(propertiesRight.Contains);
        }
    }
}