using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class ResponseFactory
    {
        public string Create(IRequest request, Template template)
        {
            return MergeWithTemplate(request, template.Response);
        }

        private static string MergeWithTemplate(IRequest request, string responseTemplate)
        {
            var mergeTokens = GetMergeTokens(responseTemplate);

            foreach (MergeToken mergeToken in mergeTokens)
            {
                var propertyValue = request.GetPropertyBy(mergeToken.Property);

                responseTemplate = responseTemplate.Replace(mergeToken.Token, propertyValue);
            }

            return responseTemplate;
        }

        private static List<MergeToken> GetMergeTokens(string mergeTemplate)
        {
            List<MergeToken> mergeFields = new List<MergeToken>();

            foreach (Match match in SearchForTokens(mergeTemplate))
            {
                mergeFields.Add(new MergeToken
                {
                    Token = match.Value,
                    Property = match.Groups[1].Value
                });
            }

            return mergeFields;
        }

        private static MatchCollection SearchForTokens(string mergeTemplate)
        {
            Regex regex = new Regex(@"\{\{(\S+)\}\}");

           return  regex.Matches(mergeTemplate);
        }
    }

    public class MergeToken
    {
        public string Token { get; set; }

        public string Property { get; set; }
    }
}