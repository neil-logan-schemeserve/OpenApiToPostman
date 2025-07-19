using Microsoft.OpenApi.Models;
using OpenApiToPostman.Core.Models.Postman;
using OpenApiToPostman.Core.Services.Interfaces;
using System.Text.Json;

namespace OpenApiToPostman.Core.Services;

public class OpenApiToPostmanTranslationService : IOpenApiToPostmanTranslationService
{
    public PostmanCollection ConvertToPostmanCollection(OpenApiDocument openApiDocument)
    {
        var collection = new PostmanCollection
        {
            Info = new Info
            {
                Name = openApiDocument.Info?.Title ?? "Generated Collection",
                Description = openApiDocument.Info?.Description,
                PostmanId = Guid.NewGuid().ToString()
            },
            Items = new List<Item>(),
            Variables = new List<Variable>(),

            // Set collection-level Bearer auth with {{token}}
            Auth = new Auth
            {
                Type = "bearer",
                Bearer = new List<AuthKeyValuePair>
                {
                    new AuthKeyValuePair
                    {
                        Key = "token",
                        Value = "{{token}}",
                        Type = "string"
                    }
                }
            }
        };

        var baseUrl = openApiDocument.Servers?.FirstOrDefault()?.Url?.TrimEnd('/');
        var baseUri = baseUrl != null ? new Uri(baseUrl) : null;

        // Collect all unique path parameters from paths and operations
        var pathParameterNames = new HashSet<string>();

        foreach (var path in openApiDocument.Paths)
        {
            // Path-level parameters
            if (path.Value.Parameters != null)
            {
                foreach (var param in path.Value.Parameters)
                {
                    if (param.In == ParameterLocation.Path)
                        pathParameterNames.Add(param.Name);
                }
            }

            // Operation-level parameters
            foreach (var operation in path.Value.Operations.Values)
            {
                if (operation.Parameters != null)
                {
                    foreach (var param in operation.Parameters)
                    {
                        if (param.In == ParameterLocation.Path)
                            pathParameterNames.Add(param.Name);
                    }
                }
            }
        }

        // Add baseUrl variable
        collection.Variables.Add(new Variable
        {
            Key = "baseUrl",
            Value = baseUrl ?? "http://localhost",
            Description = "Base URL for API requests"
        });

        // Add variables for each path parameter
        foreach (var paramName in pathParameterNames)
        {
            collection.Variables.Add(new Variable
            {
                Key = paramName,
                Value = $"{{{paramName}}}",
                Description = $"Path parameter '{paramName}'"
            });
        }

        // Add token variable for Bearer auth
        collection.Variables.Add(new Variable
        {
            Key = "token",
            Value = "",
            Description = "Bearer token for authorization"
        });

        // Process each path and operation into requests
        foreach (var path in openApiDocument.Paths)
        {
            var pathTemplate = path.Key;
            foreach (var paramName in pathParameterNames)
            {
                pathTemplate = pathTemplate.Replace($"{{{paramName}}}", $"{{{{{paramName}}}}}");
            }

            foreach (var (method, operation) in path.Value.Operations)
            {
                var request = new Request
                {
                    Method = method.ToString().ToUpperInvariant(),
                    Description = operation.Description ?? operation.Summary,
                    Url = new Url
                    {
                        Raw = $"{{{{baseUrl}}}}{pathTemplate}",
                        Host = new List<string> { "{{baseUrl}}" },
                        Path = pathTemplate
                            .Trim('/')
                            .Split('/', StringSplitOptions.RemoveEmptyEntries)
                            .ToList()
                    },
                    Headers = new List<Header>
                {
                    // You can optionally keep the Authorization header here if you want it per-request
                    new Header
                    {
                        Key = "Authorization",
                        Value = "Bearer {{token}}",
                        Description = "Bearer token authorization"
                    }
                }
                };

                if (baseUri != null)
                {
                    request.Url.Protocol = baseUri.Scheme;
                    request.Url.Host = baseUri.Host.Split('.').ToList();
                }

                // Query parameters
                var queryParams = operation.Parameters?
                    .Where(p => p.In == ParameterLocation.Query)
                    .Select(p => new QueryParam
                    {
                        Key = p.Name,
                        Value = p.Example?.ToString(),
                        Description = p.Description
                    })
                    .ToList();

                if (queryParams?.Any() == true)
                    request.Url.Query = queryParams;

                // Add request body if present
                if (operation.RequestBody != null)
                {
                    var resolvedRequestBody = ResolveRequestBody(operation.RequestBody, openApiDocument);
                    var content = resolvedRequestBody?.Content;

                    if (content != null && content.TryGetValue("application/json", out var mediaType))
                    {
                        string? example = null;

                        if (mediaType.Example != null)
                        {
                            example = mediaType.Example.ToString();
                        }
                        else if (mediaType.Examples?.Any() == true)
                        {
                            example = mediaType.Examples.First().Value?.Value?.ToString();
                        }
                        else if (mediaType.Schema != null)
                        {
                            var resolvedSchema = ResolveSchemaRecursive(mediaType.Schema, openApiDocument);
                            var sample = BuildSampleJson(resolvedSchema, openApiDocument, new HashSet<string>());
                            example = JsonSerializer.Serialize(sample, new JsonSerializerOptions { WriteIndented = true });
                        }

                        if (example != null)
                        {
                            request.Body = new Body
                            {
                                Mode = "raw",
                                Raw = example,
                                Options = new BodyOptions
                                {
                                    Raw = new RawOptions
                                    {
                                        Language = "json"
                                    }
                                }
                            };
                        }
                    }
                }

                var item = new Item
                {
                    Name = operation.Summary ?? $"{method} {pathTemplate}",
                    Request = request
                };

                collection.Items.Add(item);
            }
        }

        return collection;
    }

    private OpenApiRequestBody? ResolveRequestBody(OpenApiRequestBody requestBody, OpenApiDocument document)
    {
        if (requestBody.Reference != null &&
            document.Components.RequestBodies.TryGetValue(requestBody.Reference.Id, out var resolved))
        {
            return resolved;
        }

        return requestBody;
    }

    private OpenApiSchema ResolveSchemaRecursive(OpenApiSchema schema, OpenApiDocument document)
    {
        var seenRefs = new HashSet<string>();
        return ResolveRecursive(schema);

        OpenApiSchema ResolveRecursive(OpenApiSchema s)
        {
            while (s.Reference != null && document.Components.Schemas.TryGetValue(s.Reference.Id, out var resolved))
            {
                if (!seenRefs.Add(s.Reference.Id))
                    break;

                s = resolved;
            }

            return s;
        }
    }

    private object? BuildSampleJson(OpenApiSchema schema, OpenApiDocument document, HashSet<string> seenSchemas)
    {
        schema = ResolveSchemaRecursive(schema, document);

        // Prevent circular references
        if (schema.Reference?.Id != null && !seenSchemas.Add(schema.Reference.Id))
            return null;

        if (schema.Example != null)
            return schema.Example;

        if (schema.Default != null)
            return schema.Default;

        if (schema.AllOf?.Count > 0)
        {
            var combined = new Dictionary<string, object?>();

            foreach (var sub in schema.AllOf)
            {
                var part = BuildSampleJson(sub, document, seenSchemas);
                if (part is Dictionary<string, object?> d)
                {
                    foreach (var kvp in d)
                        combined[kvp.Key] = kvp.Value;
                }
            }

            return combined;
        }

        if (schema.OneOf?.Count > 0)
            return BuildSampleJson(schema.OneOf.First(), document, seenSchemas);

        if (schema.AnyOf?.Count > 0)
            return BuildSampleJson(schema.AnyOf.First(), document, seenSchemas);

        if (schema.Type == "object")
        {
            var obj = new Dictionary<string, object?>();

            if (schema.Properties != null)
            {
                foreach (var prop in schema.Properties)
                {
                    obj[prop.Key] = BuildSampleJson(prop.Value, document, seenSchemas);
                }
            }

            return obj;
        }

        if (schema.Type == "array" && schema.Items != null)
        {
            return new[] { BuildSampleJson(schema.Items, document, seenSchemas) };
        }

        return GetExampleValue(schema);
    }

    private object? GetExampleValue(OpenApiSchema schema)
    {
        return schema.Type switch
        {
            "string" => "example",
            "integer" => 0,
            "number" => 0.0,
            "boolean" => true,
            _ => "example"
        };
    }
}