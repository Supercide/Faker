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

        public string Match(string content)
        {
            return _templateStore.GetTemplate(ProduceContentHash(content));
        }

        private string ProduceContentHash(string content)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(content)));
        }
    }

    public interface ITemplateStore
    {
        string GetTemplate(string contentHash);
    }
}
