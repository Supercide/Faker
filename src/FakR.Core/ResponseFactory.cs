using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace FakR.Core {
    public class ResponseFactory
    {
        public string Create(IRequest request, Template template)
        {
            return MergeWithTemplate(request, template);
        }

        private static string MergeWithTemplate(IRequest request, Template template)
        {
            var mergeFields = GetMergeFields(request, template.Response);

            return template.MergeFields(mergeFields);
        }

        private static List<MergeField> GetMergeFields(IRequest request, string mergeTemplate)
        {
            List<MergeField> mergeFields = new List<MergeField>();

            foreach (Match match in SearchForTokens(mergeTemplate))
            {
                mergeFields.Add(new MergeField
                {
                    Token = match.Value,
                    Property = match.Groups[1].Value,
                    Value = request.GetPropertyValueBy(match.Groups[1].Value)
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
}