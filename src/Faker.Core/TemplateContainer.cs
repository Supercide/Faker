using System;
using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Core {
    public class TemplateContainer : ITemplateContainer
    {
        public Uri Namespace { get; protected set; }

        public IEnumerable<ITemplate> Templates { get; protected set; }
    }
}