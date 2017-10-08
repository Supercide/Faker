using Faker.Core.Extensions;

namespace Faker.Core {
    public class MergeField : IMergeField
    {
        public string Token { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
    }
}