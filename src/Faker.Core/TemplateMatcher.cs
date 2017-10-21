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

        public ITemplate Match(Uri @namespace, IRequest request, Dictionary<string, string> metadata)
        {
            var templates = _templateStore.GetTemplates(@namespace);

            return templates.Any()
                ? GetClosestMatchingTemplate(templates, request, metadata)?.Template
                : null;
        }

        private static TemplatePropertyMatchResult GetClosestMatchingTemplate(ITemplate[] templates, IRequest request, Dictionary<string, string> metadata)
        {
            IList<TemplatePropertyMatchResult> results = FindTemplatesWithMostMatchingProperties(templates, request, metadata);

            return results.Count > 1
                ? FindTemplatesWithLeastAmountOfdifferences(results).First()
                : results.FirstOrDefault();
        }

        private static IEnumerable<TemplatePropertyMatchResult> FindTemplatesWithLeastAmountOfdifferences(IList<TemplatePropertyMatchResult> results)
        {
            var minimumProperties = results.Min(result => result.TemplateProperties.Count());

            return results.Where(result => result.TemplateProperties.Count() == minimumProperties);
        }

        private static IList<TemplatePropertyMatchResult> FindTemplatesWithMostMatchingProperties(IEnumerable<ITemplate> templates, IRequest request, Dictionary<string, string> metadata)
        {
            IEnumerable<TemplatePropertyMatchResult> matchingTemplates = FindAllTemplatesWithMatchingProperties(templates, request, metadata);

            if (!matchingTemplates.Any()) return new List<TemplatePropertyMatchResult>();

            int maxMatches = matchingTemplates.Max(x => x.PropertyMatchCount);

            return matchingTemplates.Where(template => template.PropertyMatchCount == maxMatches).ToList();
        }

        private static IList<TemplatePropertyMatchResult> FindAllTemplatesWithMatchingProperties(IEnumerable<ITemplate> templates, IRequest request, Dictionary<string, string> metadata)
        {
            return templates.Select(template => CreateTemplateMatchResult(request, template))
                            .Where(result => result.PropertyMatchCount > 0)
                            .Where(result => FilterMetadata(result, metadata))
                            .ToList();
        }

        private static bool FilterMetadata(TemplatePropertyMatchResult result, Dictionary<string, string> metadata)
        {
            return result.Template
                         .Request
                         .Metadata
                         .All(kvp => !metadata.ContainsKey(kvp.Key) || metadata[kvp.Key] == kvp.Value);
        }

        private static TemplatePropertyMatchResult CreateTemplateMatchResult(IRequest request, ITemplate template)
        {
            IEnumerable<string> requestProperties = request.GetProperties();

            IEnumerable<string> templateProperties = template.Request.GetProperties().ToArray();

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