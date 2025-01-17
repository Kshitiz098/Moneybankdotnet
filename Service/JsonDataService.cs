using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoneyBanks.Service
{
    internal class JsonDataService<T>
    {
        private readonly string _filePath;

        /* public JsonFileService()
         {
             // Get the directory where the application is running
             string projectDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
             _filePath = Path.Combine(projectDirectory, "data.json");
         }
     */
        public JsonDataService()
        {
            // Explicitly set the path to the project directory
            string projectDirectory = @"D:\MODULES\3rd year\Application development\MoneyBanks";
            _filePath = Path.Combine(projectDirectory, "data.json");
        }
        public async Task<T> LoadDataAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Activator.CreateInstance<T>(); // Return default instance if file doesn't exist
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var data = JsonSerializer.Deserialize<T>(json);

                return data ?? Activator.CreateInstance<T>(); // Return default instance if deserialization fails
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading data: {ex.Message}");
            }
        }

        public async Task SaveDataAsync(T data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving data: {ex.Message}");
            }
        }
    }
}
