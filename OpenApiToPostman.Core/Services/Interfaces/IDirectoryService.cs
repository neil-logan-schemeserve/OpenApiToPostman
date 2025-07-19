using OpenApiToPostman.Core.Models.Postman;

namespace OpenApiToPostman.Core.Services.Interfaces;
public interface IDirectoryService
{
    Task<List<string>> GetFilePathsAsync(string path);
    Task SaveFileAsync(string directoryPath, string filename, string postmanCollectionJson);
}
