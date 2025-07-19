using System.Text.Json.Serialization;

namespace OpenApiToPostman.Core.Models.Postman
{
    public class PostmanCollection
    {
        [JsonPropertyName("info")]
        public Info Info { get; set; } = new Info();

        [JsonPropertyName("item")]
        public List<Item> Items { get; set; } = new List<Item>();

        [JsonPropertyName("variable")]
        public List<Variable>? Variables { get; set; }

        [JsonPropertyName("auth")]
        public Auth? Auth { get; set; }
    }

    public class Info
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("schema")]
        public string Schema { get; set; } = "https://schema.getpostman.com/json/collection/v2.1.0/collection.json";

        [JsonPropertyName("_postman_id")]
        public string? PostmanId { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // Item can be a folder (with nested items)
        [JsonPropertyName("item")]
        public List<Item>? Items { get; set; }

        // Or an actual request
        [JsonPropertyName("request")]
        public Request? Request { get; set; }

        [JsonPropertyName("response")]
        public List<Response>? Response { get; set; }
    }

    public class Request
    {
        [JsonPropertyName("method")]
        public string Method { get; set; } = "GET";

        [JsonPropertyName("header")]
        public List<Header>? Headers { get; set; }

        [JsonPropertyName("body")]
        public Body? Body { get; set; }

        [JsonPropertyName("url")]
        public Url Url { get; set; } = new Url();

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("auth")]
        public Auth? Auth { get; set; }
    }

    public class Url
    {
        // Full raw URL string
        [JsonPropertyName("raw")]
        public string Raw { get; set; } = string.Empty;

        // URL protocol: http, https
        [JsonPropertyName("protocol")]
        public string? Protocol { get; set; }

        // Host as array (["api", "example", "com"])
        [JsonPropertyName("host")]
        public List<string>? Host { get; set; }

        // Path segments (["v1", "users"])
        [JsonPropertyName("path")]
        public List<string>? Path { get; set; }

        // Query parameters
        [JsonPropertyName("query")]
        public List<QueryParam>? Query { get; set; }

        // Variables inside the URL like {{baseUrl}}
        [JsonPropertyName("variable")]
        public List<Variable>? Variables { get; set; }
    }

    public class QueryParam
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class Header
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class Body
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = "raw";

        [JsonPropertyName("raw")]
        public string? Raw { get; set; }

        [JsonPropertyName("urlencoded")]
        public List<BodyKeyValue>? UrlEncoded { get; set; }

        [JsonPropertyName("formdata")]
        public List<BodyKeyValue>? FormData { get; set; }

        [JsonPropertyName("options")]
        public BodyOptions? Options { get; set; }
    }

    public class BodyKeyValue
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }  // e.g., "text", "file"
    }

    public class BodyOptions
    {
        [JsonPropertyName("raw")]
        public RawOptions? Raw { get; set; }
    }

    public class RawOptions
    {
        [JsonPropertyName("language")]
        public string Language { get; set; } = "json";
    }

    public class Response
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("originalRequest")]
        public Request? OriginalRequest { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("_postman_previewlanguage")]
        public string? PostmanPreviewLanguage { get; set; }

        [JsonPropertyName("header")]
        public List<Header>? Headers { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public class Variable
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }  // e.g., "string"

        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }
    }

    public class Auth
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "noauth"; // Use string, not enum

        [JsonPropertyName("basic")]
        public List<AuthKeyValuePair>? Basic { get; set; }

        [JsonPropertyName("bearer")]
        public List<AuthKeyValuePair>? Bearer { get; set; }
    }

    public class AuthKeyValuePair
    {
        [JsonPropertyName("key")]
        public string Key { get; set; } = "";

        [JsonPropertyName("value")]
        public string Value { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "string";
    }
}
