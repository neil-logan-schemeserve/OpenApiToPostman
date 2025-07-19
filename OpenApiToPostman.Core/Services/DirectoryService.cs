using OpenApiToPostman.Core.Services.Interfaces;

namespace OpenApiToPostman.Core.Services;

public class DirectoryService : IDirectoryService
{
    public async Task<List<string>> GetFilePathsAsync(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");

        return await Task.Run(() =>
        {
            return Directory
                .EnumerateFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly)
                .ToList();
        });
    }

    public async Task SaveFileAsync(string directoryPath, string filename, string postmanCollectionJson)
    {
        Directory.CreateDirectory(directoryPath); // Ensures the folder exists

        var fileName = $"{filename}.json";
        var filePath = Path.Combine(directoryPath, fileName);

        // Convert the string to bytes using UTF8 encoding
        var bytes = System.Text.Encoding.UTF8.GetBytes(postmanCollectionJson);

        await using var fileStream = File.Create(filePath);
        await fileStream.WriteAsync(bytes);
    }
}
