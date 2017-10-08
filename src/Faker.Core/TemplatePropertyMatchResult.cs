using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Core {
    public class TemplatePropertyMatchResult
    {
        public IEnumerable<string> TemplateProperties { get; set; }
        public int PropertyMatchCount { get; set; }
        public ITemplate Template { get; set; }
    }
}