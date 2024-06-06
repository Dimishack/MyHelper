using MyHelper.Services.Interfaces;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace MyHelper.Services
{
    internal class WorkWithJSONFileServices : IWorkWithJSONFile
    {
        public bool ReadFile<T>(string filePath, out T? readData)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                    readData = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                return true;
            }
            catch (Exception)
            {
                readData = default;
                return false;
            }
        }

        public async Task<bool> WriteFileAsync(string filePath, object? data)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                    await writer.WriteAsync(JsonConvert.SerializeObject(data, Formatting.Indented)).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
