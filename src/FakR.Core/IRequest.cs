namespace FakR.Core {

    public interface IRequest
    {
        string GetPropertyBy(string path);

        string GetPropertyBy(int index);
    }
}