using System;
using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Core {
    public class TemplateContainer : ITemplateContainer<Template>
    {
        public Uri Namespace { get; set; }

        public List<Template> Templates { get; set; }
    }
}