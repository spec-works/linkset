using System.Text.Json.Serialization;

namespace Linkset;

/// <summary>
/// Represents a single link in a linkset as defined in RFC 9264 and RFC 8288.
/// </summary>
public class Link
{
    /// <summary>
    /// The target URI of the link (required).
    /// </summary>
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// The relation type(s) of the link (optional).
    /// Can be a registered relation type or an extension relation type (URI).
    /// </summary>
    [JsonPropertyName("rel")]
    public string? Rel { get; set; }

    /// <summary>
    /// The context URI of the link (optional).
    /// If not present, the context is the linkset itself.
    /// </summary>
    [JsonPropertyName("anchor")]
    public string? Anchor { get; set; }

    /// <summary>
    /// The media type of the target resource (optional).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// The language of the target resource (optional).
    /// </summary>
    [JsonPropertyName("hreflang")]
    public string? Hreflang { get; set; }

    /// <summary>
    /// A human-readable title for the link (optional).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// The length/size of the target resource in bytes (optional).
    /// </summary>
    [JsonPropertyName("length")]
    public long? Length { get; set; }

    /// <summary>
    /// Extension attributes not defined in the standard.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? ExtensionData { get; set; }
}
