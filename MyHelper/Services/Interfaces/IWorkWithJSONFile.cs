namespace MyHelper.Services.Interfaces
{
    internal interface IWorkWithJSONFile
    {
        bool ReadFile<T>(string filePath, out T? readData);
        bool WriteFile(string filePath, object? data);
    }
}
