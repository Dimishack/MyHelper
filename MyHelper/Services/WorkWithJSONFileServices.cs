using MyHelper.Services.Interfaces;
using Newtonsoft.Json;
using System.IO;

namespace MyHelper.Services
{
    internal class WorkWithJSONFileServices : IWorkWithJSONFile
    {
        public bool ReadFile<T>(string filePath, out T? readData)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                readData = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                reader.Dispose();
                return true;
            }
            catch (Exception)
            {
                readData = default;
                return false;
            }
        }

        public async Task<bool> WriteFile(string filePath, object? data)
        {
            using var writer = new StreamWriter(filePath);
            await writer.WriteAsync(JsonConvert.SerializeObject(data, Formatting.Indented));
            writer.Dispose();
            //File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            return true;
        }
    }
}
