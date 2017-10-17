namespace Faker.Core {
    public interface IMergeField
    {
        string Property { get; set; }
        string Token { get; set; }
        string Value { get; set; }
    }
}