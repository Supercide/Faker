using System.Collections.Generic;

namespace FakR.Core {
    public class TemplatePropertyMatchResult
    {
        public IEnumerable<string> TemplateProperties { get; set; }
        public int PropertyMatchCount { get; set; }
        public Template Template { get; set; }
    }
}