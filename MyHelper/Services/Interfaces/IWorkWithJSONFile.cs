
namespace MyHelper.Services.Interfaces
{
    internal interface IWorkWithJSONFile
    {
        T? ReadFile<T>(string filePath);
        Task<T?> ReadFileAsync<T>(string filePath);
        bool WriteFile(string filePath, object? data);
        Task<bool> WriteFileAsync(string filePath, object? data);
    }
}
