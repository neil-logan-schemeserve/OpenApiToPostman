using OpenApiToPostman.Core.Models.Postman;
using OpenApiToPostman.Core.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenApiToPostman.Core.Services;
public class PostmanService : IPostmanService
{
    public async Task<string> ConvertToJsonStringAsync(PostmanCollection postmanCollection)
    {
        using var memoryStream = new MemoryStream();

        await JsonSerializer.SerializeAsync(memoryStream, postmanCollection, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        return await reader.ReadToEndAsync();
    }
}
