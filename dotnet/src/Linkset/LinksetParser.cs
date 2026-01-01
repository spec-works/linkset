using System.Text.Json;

namespace Linkset;

/// <summary>
/// Parser for application/linkset+json documents as defined in RFC 9264.
/// </summary>
public class LinksetParser
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the LinksetParser with default options.
    /// </summary>
    public LinksetParser() : this(new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    })
    {
    }

    /// <summary>
    /// Initializes a new instance of the LinksetParser with custom JSON serializer options.
    /// </summary>
    /// <param name="options">Custom JSON serializer options.</param>
    public LinksetParser(JsonSerializerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Parses a linkset JSON string into a LinksetDocument.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The parsed LinksetDocument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when json is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is malformed.</exception>
    /// <exception cref="LinksetParseException">Thrown when the linkset structure is invalid.</exception>
    public LinksetDocument Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json), "JSON string cannot be null or empty.");
        }

        try
        {
            var document = JsonSerializer.Deserialize<LinksetDocument>(json, _options);

            if (document == null)
            {
                throw new LinksetParseException("Failed to deserialize linkset document: result was null.");
            }

            ValidateLinkset(document);

            return document;
        }
        catch (JsonException ex)
        {
            throw new LinksetParseException($"Invalid JSON format: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Parses a linkset JSON stream into a LinksetDocument.
    /// </summary>
    /// <param name="stream">The stream containing the JSON to parse.</param>
    /// <returns>The parsed LinksetDocument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is malformed.</exception>
    /// <exception cref="LinksetParseException">Thrown when the linkset structure is invalid.</exception>
    public async Task<LinksetDocument> ParseAsync(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        try
        {
            var document = await JsonSerializer.DeserializeAsync<LinksetDocument>(stream, _options);

            if (document == null)
            {
                throw new LinksetParseException("Failed to deserialize linkset document: result was null.");
            }

            ValidateLinkset(document);

            return document;
        }
        catch (JsonException ex)
        {
            throw new LinksetParseException($"Invalid JSON format: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Serializes a LinksetDocument to a JSON string.
    /// </summary>
    /// <param name="document">The LinksetDocument to serialize.</param>
    /// <returns>The JSON string representation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when document is null.</exception>
    public string Serialize(LinksetDocument document)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        ValidateLinkset(document);

        return JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }

    /// <summary>
    /// Serializes a LinksetDocument to a stream.
    /// </summary>
    /// <param name="document">The LinksetDocument to serialize.</param>
    /// <param name="stream">The stream to write to.</param>
    /// <exception cref="ArgumentNullException">Thrown when document or stream is null.</exception>
    public async Task SerializeAsync(LinksetDocument document, Stream stream)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        ValidateLinkset(document);

        await JsonSerializer.SerializeAsync(stream, document, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });
    }

    /// <summary>
    /// Validates the linkset document structure.
    /// </summary>
    /// <param name="document">The document to validate.</param>
    /// <exception cref="LinksetParseException">Thrown when validation fails.</exception>
    private void ValidateLinkset(LinksetDocument document)
    {
        if (document.Linkset == null)
        {
            throw new LinksetParseException("Linkset document must contain a 'linkset' array.");
        }

        for (int i = 0; i < document.Linkset.Count; i++)
        {
            var link = document.Linkset[i];
            if (link == null)
            {
                throw new LinksetParseException($"Link at index {i} is null.");
            }

            if (string.IsNullOrWhiteSpace(link.Href))
            {
                throw new LinksetParseException($"Link at index {i} is missing required 'href' property.");
            }

            // Validate that href is a valid URI
            if (!Uri.TryCreate(link.Href, UriKind.RelativeOrAbsolute, out var uri))
            {
                throw new LinksetParseException($"Link at index {i} has an invalid 'href' value: {link.Href}");
            }

            // Additional validation: URIs should not contain unescaped spaces or be malformed
            // Check if it's a well-formed URI (if absolute) or a valid relative reference
            if (uri != null && uri.IsAbsoluteUri)
            {
                if (!uri.IsWellFormedOriginalString())
                {
                    throw new LinksetParseException($"Link at index {i} has a malformed 'href' value: {link.Href}");
                }
            }
            else if (uri != null)
            {
                // For relative URIs, check for obviously invalid patterns
                if (link.Href.Contains("://") && !uri.IsAbsoluteUri)
                {
                    // If it contains "://" but couldn't be parsed as absolute URI, it's likely malformed
                    throw new LinksetParseException($"Link at index {i} has an invalid 'href' value: {link.Href}");
                }
            }
        }
    }
}
