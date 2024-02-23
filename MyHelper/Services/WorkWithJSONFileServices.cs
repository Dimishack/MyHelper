using MyHelper.Services.Interfaces;
using Newtonsoft.Json;
using System.IO;

namespace MyHelper.Services
{
    internal class WorkWithJSONFileServices : IWorkWithJSONFile
    {
        public bool ReadFile<T>(string filePath, out T? readData)
        {
            if(!File.Exists(filePath))
            {
                readData = default;
                return false;
            }
            readData = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            if (readData is null) return false;
            return true;
        }

        public bool WriteFile(string filePath, object? data)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            return true;
        }
    }
}
