using System;
using System.Collections.Generic;
using System.Linq;
using Faker.Core.Extensions;

namespace Faker.Core
{
    public class TemplateMatcher<T> where T : ITemplate
    {
        private readonly ITemplateStore<T> _templateStore;

        public TemplateMatcher(ITemplateStore<T> templateStore)
        {
            _templateStore = templateStore;
        }

        public T Match(Uri @namespace, IRequest request)
        {
            ITemplateContainer<T> templateContainer = _templateStore.GetTemplateContainer(@namespace);

            if(templateContainer?.Templates?.Any() ?? false)
            {
                return GetClosestMatchingTemplate(templateContainer.Templates, request).Template;
            }

            return default(T);
        }

        private static TemplatePropertyMatchResult<T> GetClosestMatchingTemplate(IEnumerable<T> templates, IRequest request)
        {
            IList<TemplatePropertyMatchResult<T>> results = FindTemplatesWithMostMatchingProperties(templates, request);

            return results.Count > 1
                ? FindTemplatesWithLeastAmountOfdifferences(results).First()
                : results.FirstOrDefault();
        }

        private static IEnumerable<TemplatePropertyMatchResult<T>> FindTemplatesWithLeastAmountOfdifferences(IList<TemplatePropertyMatchResult<T>> results)
        {
            var minimumProperties = results.Min(result => result.TemplateProperties.Count());

            return results.Where(result => result.TemplateProperties.Count() == minimumProperties);
        }

        private static IList<TemplatePropertyMatchResult<T>> FindTemplatesWithMostMatchingProperties(IEnumerable<T> templates, IRequest request)
        {
            IEnumerable<TemplatePropertyMatchResult<T>> matchingTemplates = FindAllTemplatesWithMatchingProperties(templates, request);

            if (!matchingTemplates.Any()) return new List<TemplatePropertyMatchResult<T>>();

            int maxMatches = matchingTemplates.Max(x => x.PropertyMatchCount);

            return matchingTemplates.Where(template => template.PropertyMatchCount == maxMatches).ToList();
        }

        private static IList<TemplatePropertyMatchResult<T>> FindAllTemplatesWithMatchingProperties(IEnumerable<T> templates, IRequest request)
        {
            return templates.Select(template => CreateTemplateMatchResult(request, template))
                            .Where(result => result.PropertyMatchCount > 0)
                            .Where(result => FilterMetadata(result, request.Metadata))
                            .ToList();
        }

        private static bool FilterMetadata(TemplatePropertyMatchResult<T> result, IReadOnlyDictionary<string, string> metadata)
        {
            return result.Template
                         .Metadata?
                         .All(kvp => !metadata.ContainsKey(kvp.Key) || metadata[kvp.Key] == kvp.Value)??true;
        }

        private static TemplatePropertyMatchResult<T> CreateTemplateMatchResult(IRequest request, T template)
        {
            IEnumerable<string> requestProperties = request.GetProperties();

            IEnumerable<string> templateProperties = template.Properties.Keys;

            return new TemplatePropertyMatchResult<T>
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