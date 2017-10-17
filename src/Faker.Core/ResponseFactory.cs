using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Faker.Core.Extensions;

namespace Faker.Core {
    public class ResponseFactory
    {
        public string Create(IRequest request, ITemplate template)
        {
            return MergeWithTemplate(request, template);
        }

        private static string MergeWithTemplate(IRequest request, ITemplate template)
        {
            var mergeFields = GetMergeFields(request, template.Response);

            string response = template.Response;

            foreach (IMergeField mergeToken in mergeFields)
            {
                response = response.Replace(mergeToken.Token, mergeToken.Value);
            }

            return response;
        }

        private static IEnumerable<IMergeField> GetMergeFields(IRequest request, string mergeTemplate)
        {
            var matches = SearchForTokens(mergeTemplate);

            return matches.Select(match => CreateMergeField(request, match));
        }

        private static IMergeField CreateMergeField(IRequest request, Match match)
        {
            Property property = match.Groups[1].Value;

            return new MergeField
            {
                Token = match.Value,
                Property = property,
                Value = property.IsNumber
                            ? request.GetPropertyValueBy(property.Index)
                            : request.GetPropertyValueBy(property.Path)
            };
        }

        private static IEnumerable<Match> SearchForTokens(string mergeTemplate)
        {
            Regex regex = new Regex(@"\{\{(\S+)\}\}");

           return regex.Matches(mergeTemplate)
                       .Cast<Match>();
        }
    }
}