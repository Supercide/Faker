using System;

namespace FakR.Core {
    public interface ITemplateStore
    {
        string[] GetTemplates(Uri @namespace);
    }
}