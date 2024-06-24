using MyHelper.Services.Interfaces;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MyHelper.Services
{
    internal class WorkWithJSONFileServices(IUserDialog userDialog) : IWorkWithJSONFile
    {
        private readonly IUserDialog _userDialog = userDialog;
        private readonly JsonSerializerOptions _options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.Cyrillic, UnicodeRanges.BasicLatin),
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault
        };

        public T? ReadFile<T>(string filePath)
        {
            try
            {
                T? data;
                using (var reader = new StreamReader(filePath))
                    data = JsonSerializer.Deserialize<T>(reader.ReadToEnd());
                return data;
            }
            catch (Exception ex)
            {
                _userDialog.ErrorMessage($"Ошибка чтения файла!\n{ex.Message}");
                return default;
            }
        }

        public async Task<T?> ReadFileAsync<T>(string filePath)
        {
            try
            {
                T? data;
                using (var fs = new FileStream(filePath, FileMode.Open))
                    data = await JsonSerializer.DeserializeAsync<T>(fs, _options);
                return data;
            }
            catch (Exception ex)
            {
                _userDialog.ErrorMessage($"Ошибка чтения файла!\n{ex.Message}");
                return default;
            }
        }

        public bool WriteFile(string filePath, object? data)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                    writer.Write(JsonSerializer.Serialize(data));
                return true;
            }
            catch (Exception ex)
            {
                _userDialog.ErrorMessage($"Ошибка записи файла!\n{ex.Message}");
                return false;
            }
        }

        public async Task<bool> WriteFileAsync(string filePath, object? data)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                    await JsonSerializer.SerializeAsync(fs, data, _options);
                return true;
            }
            catch (Exception ex)
            {
                _userDialog.ErrorMessage($"Ошибка записи файла!\n{ex.Message}");
                return false;
            }
        }
    }
}
