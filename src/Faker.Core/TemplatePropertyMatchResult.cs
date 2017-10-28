using System.Collections.Generic;
using Faker.Core.Extensions;

namespace Faker.Core {
    public class TemplatePropertyMatchResult<T> where T : ITemplate
    {
        public IEnumerable<string> TemplateProperties { get; set; }
        public int PropertyMatchCount { get; set; }
        public T Template { get; set; }
    }
}