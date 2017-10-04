using System;
using System.Security.Cryptography;
using System.Text;

namespace FakR.Core
{
    public class TemplateMatcher
    {
        private readonly ITemplateStore _templateStore;

        public TemplateMatcher(ITemplateStore templateStore)
        {
            _templateStore = templateStore;
        }

        public string Match(string content, Uri @namespace)
        {
            return _templateStore.GetTemplate(content, @namespace);
        }
    }

    public interface ITemplateStore
    {
        string GetTemplate(string contentHash, Uri @namespace);
    }
}
