namespace Faker.Core {

    public interface IRequest
    {
        string GetPropertyValueBy(string path);
        string GetPropertyValueBy(int index);
    }
}