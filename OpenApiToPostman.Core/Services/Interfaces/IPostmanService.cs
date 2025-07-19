using OpenApiToPostman.Core.Models.Postman;

namespace OpenApiToPostman.Core.Services.Interfaces;
public interface IPostmanService
{
    Task<string> ConvertToJsonStringAsync(PostmanCollection postmanCollection);
}
