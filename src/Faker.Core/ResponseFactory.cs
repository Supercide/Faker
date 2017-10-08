using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Faker.Core.Extensions;
using Newtonsoft.Json.Linq;

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

            return template.MergeFields(mergeFields);
        }

        private static IEnumerable<MergeField> GetMergeFields(IRequest request, string mergeTemplate)
        {
            var matches = SearchForTokens(mergeTemplate);

            return matches.Select(match => CreateMergeField(request, match));
        }

        private static MergeField CreateMergeField(IRequest request, Match match)
        {
            return new MergeField
            {
                Token = match.Value,
                Property = match.Groups[1].Value,
                Value = request.GetPropertyValueBy(match.Groups[1].Value)
            };
        }

        private static IEnumerable<Match> SearchForTokens(string mergeTemplate)
        {
            Regex regex = new Regex(@"\{\{(\S+)\}\}");

           return  regex.Matches(mergeTemplate).Cast<Match>();
        }
    }
}