using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using OpenApiToPostman.Core.Services.Interfaces;

namespace OpenApiToPostman.Core.Services;
public class OpenApiService : IOpenApiService
{
    public async Task<OpenApiDocument> LoadDocumentAsync(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            var reader = new OpenApiStreamReader();
            var readResult = await reader.ReadAsync(stream);
            return readResult.OpenApiDocument;
        }
    }
}
