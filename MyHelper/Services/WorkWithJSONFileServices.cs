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
                readData = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                if (readData is null) return false;
                return true;
            }
            catch (Exception)
            {
                readData = default;
                return false;
            }
        }

        public bool WriteFile(string filePath, object? data)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            return true;
        }
    }
}
