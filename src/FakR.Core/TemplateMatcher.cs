using System;

namespace FakR.Core
{
    public class TemplateMatcher
    {
        private readonly ITemplateStore _templateStore;

        public TemplateMatcher(ITemplateStore templateStore)
        {
            _templateStore = templateStore;
        }
    }

    public interface ITemplateStore
    {
        
    }
}
