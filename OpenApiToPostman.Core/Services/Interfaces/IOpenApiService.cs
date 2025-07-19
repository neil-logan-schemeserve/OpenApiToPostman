using Microsoft.OpenApi.Models;

namespace OpenApiToPostman.Core.Services.Interfaces;
public interface IOpenApiService
{
    Task<OpenApiDocument> LoadDocumentAsync(string filePath);
}
