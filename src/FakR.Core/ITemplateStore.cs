using System;

namespace FakR.Core {
    public interface ITemplateStore
    {
        Template[] GetTemplates(Uri @namespace);
    }
}