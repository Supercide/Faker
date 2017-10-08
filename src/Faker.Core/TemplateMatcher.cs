using System;
using System.Collections.Generic;
using System.Linq;
using Faker.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Faker.Core
{
    public class TemplateMatcher
    {
        private readonly ITemplateStore _templateStore;

        public TemplateMatcher(ITemplateStore templateStore)
        {
            _templateStore = templateStore;
        }

        public ITemplate Match(string pattern, Uri @namespace)
        {
            var templates = _templateStore.GetTemplates(@namespace);

            return templates.Any()
                ? GetClosestMatchingTemplate(templates, pattern)?.Template
                : null;
        }

        private static TemplatePropertyMatchResult GetClosestMatchingTemplate(ITemplate[] templates, string request)
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

        private static IEnumerable<string> GetRequestProperties(string request)
        {
            JObject token = JObject.Parse(string.IsNullOrEmpty(request) ? "{}" : request);

            return token.Properties()
                        .Select(property => property.Name);
        }

        private static IList<TemplatePropertyMatchResult> FindTemplatesWithMostMatchingProperties(IEnumerable<ITemplate> templates, string request)
        {
            IEnumerable<TemplatePropertyMatchResult> matchingTemplates = FindAllTemplatesWithMatchingProperties(templates, request);

            int maxMatches = matchingTemplates.Max(x => x.PropertyMatchCount);

            return matchingTemplates.Where(template => template.PropertyMatchCount == maxMatches).ToList();
        }

        private static IList<TemplatePropertyMatchResult> FindAllTemplatesWithMatchingProperties(IEnumerable<ITemplate> templates, string request)
        {
            return templates.Select(template => CalculatePropertiesMatchingInTemplate(request, template))
                            .Where(result => result.PropertyMatchCount > 0)
                            .ToList();
        }

        private static TemplatePropertyMatchResult CalculatePropertiesMatchingInTemplate(string request, ITemplate template)
        {
            IEnumerable<string> requestProperties = GetRequestProperties(request);

            IList<string> templateProperties = template.GetProperties();

            return new TemplatePropertyMatchResult
            {
                Template = template,
                PropertyMatchCount = GetPropertyMatchCount(requestProperties, templateProperties),
                TemplateProperties = templateProperties
            };
        }

        private static int GetPropertyMatchCount(IEnumerable<string> propertiesLeft, IList<string> propertiesRight)
        {
            return propertiesLeft.Count(propertiesRight.Contains);
        }
    }
}