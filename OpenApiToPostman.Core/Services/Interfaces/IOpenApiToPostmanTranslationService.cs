using Microsoft.OpenApi.Models;
using OpenApiToPostman.Core.Models.Postman;

namespace OpenApiToPostman.Core.Services.Interfaces;
public interface IOpenApiToPostmanTranslationService
{
    PostmanCollection ConvertToPostmanCollection(OpenApiDocument openApiDocument);
}
