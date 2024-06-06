namespace MyHelper.Services.Interfaces
{
    internal interface IWorkWithJSONFile
    {
        bool ReadFile<T>(string filePath, out T? readData);
        Task<bool> WriteFileAsync(string filePath, object? data);
    }
}
