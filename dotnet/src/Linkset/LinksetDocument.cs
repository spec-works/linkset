using System.Text.Json.Serialization;

namespace Linkset;

/// <summary>
/// Represents a linkset document as defined in RFC 9264.
/// The document contains an array of Link objects.
/// </summary>
public class LinksetDocument
{
    /// <summary>
    /// The array of links in the linkset.
    /// </summary>
    [JsonPropertyName("linkset")]
    public List<Link>? Linkset { get; set; }

    /// <summary>
    /// Extension data for additional properties not defined in the standard.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? ExtensionData { get; set; }

    /// <summary>
    /// Gets all links with the specified relation type.
    /// </summary>
    /// <param name="rel">The relation type to filter by.</param>
    /// <returns>A list of links with the specified relation type.</returns>
    public List<Link> GetLinksByRel(string rel)
    {
        if (string.IsNullOrWhiteSpace(rel))
        {
            throw new ArgumentNullException(nameof(rel));
        }

        return (Linkset ?? new List<Link>()).Where(link => link.Rel?.Equals(rel, StringComparison.OrdinalIgnoreCase) == true).ToList();
    }

    /// <summary>
    /// Gets the first link with the specified relation type, or null if not found.
    /// </summary>
    /// <param name="rel">The relation type to find.</param>
    /// <returns>The first link with the specified relation type, or null if not found.</returns>
    public Link? GetFirstLinkByRel(string rel)
    {
        if (string.IsNullOrWhiteSpace(rel))
        {
            throw new ArgumentNullException(nameof(rel));
        }

        return (Linkset ?? new List<Link>()).FirstOrDefault(link => link.Rel?.Equals(rel, StringComparison.OrdinalIgnoreCase) == true);
    }

    /// <summary>
    /// Gets all links with the specified media type.
    /// </summary>
    /// <param name="type">The media type to filter by.</param>
    /// <returns>A list of links with the specified media type.</returns>
    public List<Link> GetLinksByType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentNullException(nameof(type));
        }

        return (Linkset ?? new List<Link>()).Where(link => link.Type?.Equals(type, StringComparison.OrdinalIgnoreCase) == true).ToList();
    }

    /// <summary>
    /// Gets all distinct relation types present in the linkset.
    /// </summary>
    /// <returns>A list of distinct relation types.</returns>
    public List<string> GetAllRelationTypes()
    {
        return (Linkset ?? new List<Link>())
            .Where(link => !string.IsNullOrWhiteSpace(link.Rel))
            .Select(link => link.Rel!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
