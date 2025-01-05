using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonDataService<T>
{
    // Define the relative path to the data.json file in your project folder
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
        string projectDirectory = @"E:\moneybankdotnet\Moneybank\Moneybank";
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