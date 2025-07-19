using OpenApiToPostman.Core.Services.Interfaces;

namespace OpenApiToPostman.Core.Managers;
public class OpenApiToPostmanManager
{
    private readonly IOpenApiService _openApiService;
    private readonly IPostmanService _postmanService;
    private readonly IOpenApiToPostmanTranslationService _openApiToPostmanTranslationService;
    private readonly IDirectoryService _directoryService;

    public OpenApiToPostmanManager(
        IOpenApiService openApiService,
        IPostmanService postmanService,
        IOpenApiToPostmanTranslationService openApiToPostmanTranslationService,
        IDirectoryService directoryService)
    {
        _openApiService = openApiService;
        _postmanService = postmanService;
        _openApiToPostmanTranslationService = openApiToPostmanTranslationService;
        _directoryService = directoryService;
    }

    public async Task ConvertSelectedFileAsync(string inputDirectoryPath, string outputDirectoryPath)
    {
        var filePaths = await _directoryService.GetFilePathsAsync(inputDirectoryPath);
        
        foreach (var file in filePaths)
        {
            var openApiDocument = await _openApiService.LoadDocumentAsync(file);
            var postmanCollection = _openApiToPostmanTranslationService.ConvertToPostmanCollection(openApiDocument);
            var postmanCollectionJson = await _postmanService.ConvertToJsonStringAsync(postmanCollection);
            await _directoryService.SaveFileAsync(outputDirectoryPath, postmanCollection.Info.Name, postmanCollectionJson);
        }
    }
}