using System;

namespace Faker.Core {
    public interface ITemplateStore
    {
        Template[] GetTemplates(Uri @namespace);
    }
}