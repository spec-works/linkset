# Linkset - Application/Linkset+JSON Parser for C#

A C# library for parsing and serializing `application/linkset+json` documents as defined in [RFC 9264](https://www.rfc-editor.org/rfc/rfc9264.html).

## Overview

This library provides a complete implementation for working with linkset documents, which are JSON-formatted collections of web links. Linksets are useful for representing relationships between web resources, API documentation, specifications, and more.

## Features

- ✅ **RFC 9264 Compliant**: Full support for the linkset media type specification
- ✅ **RFC 8288 Web Linking**: Supports all standard link attributes
- ✅ **Type-Safe Models**: Strongly-typed classes for linkset documents and links
- ✅ **Async Support**: Async methods for parsing and serializing streams
- ✅ **Validation**: Comprehensive validation of linkset structure and URIs
- ✅ **Query Helpers**: Convenient methods to filter and query links
- ✅ **Extension Data**: Preserves custom properties not defined in the standard
- ✅ **Well-Tested**: 55+ tests covering all functionality

## Installation

```bash
# Build the library
dotnet build src/Linkset/Linkset.csproj

# Run tests
dotnet test
```

## Quick Start

### Parsing a Linkset

```csharp
using Linkset;

var json = @"{
    ""linkset"": [
        {
            ""href"": ""https://www.rfc-editor.org/rfc/rfc9264.html"",
            ""rel"": ""describedby"",
            ""type"": ""text/html"",
            ""title"": ""Linkset Specification""
        },
        {
            ""href"": ""https://example.org/api/docs"",
            ""rel"": ""related"",
            ""type"": ""text/html""
        }
    ]
}";

var parser = new LinksetParser();
var document = parser.Parse(json);

// Access links
Console.WriteLine($"Total links: {document.Linkset.Count}");

foreach (var link in document.Linkset)
{
    Console.WriteLine($"{link.Rel}: {link.Href}");
}
```

### Creating and Serializing a Linkset

```csharp
var document = new LinksetDocument
{
    Linkset = new List<Link>
    {
        new Link
        {
            Href = "https://www.rfc-editor.org/rfc/rfc9264.html",
            Rel = "describedby",
            Type = "text/html",
            Title = "Linkset: Media Types and a Link Relation Type for Link Sets"
        },
        new Link
        {
            Href = "https://www.rfc-editor.org/rfc/rfc8288.html",
            Rel = "describedby",
            Type = "text/html",
            Title = "Web Linking"
        }
    ]
};

var parser = new LinksetParser();
var json = parser.Serialize(document);

Console.WriteLine(json);
```

### Query Helpers

```csharp
// Get all links with a specific relation type
var describedByLinks = document.GetLinksByRel("describedby");

// Get the first link with a specific relation type
var firstRelated = document.GetFirstLinkByRel("related");

// Get all links with a specific media type
var htmlLinks = document.GetLinksByType("text/html");

// Get all distinct relation types
var allRelations = document.GetAllRelationTypes();
```

### Async Parsing and Serialization

```csharp
// Parse from stream
using var fileStream = File.OpenRead("specs.json");
var document = await parser.ParseAsync(fileStream);

// Serialize to stream
using var outputStream = File.Create("output.json");
await parser.SerializeAsync(document, outputStream);
```

## Link Properties

The `Link` class supports all standard properties from RFC 8288:

| Property | Type | Description |
|----------|------|-------------|
| `Href` | string | Target URI (required) |
| `Rel` | string? | Link relation type |
| `Anchor` | string? | Context URI |
| `Type` | string? | Media type of the target |
| `Hreflang` | string? | Language of the target |
| `Title` | string? | Human-readable title |
| `Length` | long? | Content length in bytes |
| `ExtensionData` | Dictionary? | Custom properties |

## Validation

The parser validates:
- ✅ Required `linkset` array is present
- ✅ Each link has a required `href` property
- ✅ `href` values are valid URIs (absolute or relative)
- ✅ Well-formed absolute URIs
- ✅ No malformed URI patterns

Invalid linksets throw `LinksetParseException` with detailed error messages.

## Examples

### Specification References (like this project's specs.json)

```csharp
var specs = new LinksetDocument
{
    Linkset = new List<Link>
    {
        new Link
        {
            Href = "https://www.rfc-editor.org/rfc/rfc9264.html",
            Type = "text/html",
            Rel = "describedby",
            Title = "Linkset: Media Types and a Link Relation Type for Link Sets"
        },
        new Link
        {
            Href = "https://www.rfc-editor.org/rfc/rfc8288.html",
            Type = "text/html",
            Rel = "describedby",
            Title = "Web Linking"
        },
        new Link
        {
            Href = "https://www.iana.org/assignments/media-types/application/linkset+json",
            Type = "text/html",
            Rel = "related",
            Title = "IANA Media Type Registration"
        }
    }
};

var json = parser.Serialize(specs);
File.WriteAllText("specs.json", json);
```

### Multi-Language Alternatives

```csharp
var document = new LinksetDocument
{
    Linkset = new List<Link>
    {
        new Link
        {
            Href = "https://example.org/doc/en",
            Rel = "alternate",
            Hreflang = "en",
            Title = "English Version"
        },
        new Link
        {
            Href = "https://example.org/doc/fr",
            Rel = "alternate",
            Hreflang = "fr",
            Title = "French Version"
        }
    }
};
```

### Links with Anchors (Context)

```csharp
var document = new LinksetDocument
{
    Linkset = new List<Link>
    {
        new Link
        {
            Anchor = "https://example.org/resource",
            Href = "https://example.org/resource/description",
            Rel = "describedby",
            Type = "text/html"
        },
        new Link
        {
            Anchor = "https://example.org/resource",
            Href = "https://example.org/resource/metadata",
            Rel = "describedby",
            Type = "application/rdf+xml"
        }
    }
};
```

## Error Handling

```csharp
try
{
    var document = parser.Parse(json);
}
catch (ArgumentNullException ex)
{
    // JSON was null or empty
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (LinksetParseException ex)
{
    // Linkset structure is invalid
    Console.WriteLine($"Parse error: {ex.Message}");
}
catch (JsonException ex)
{
    // JSON syntax error
    Console.WriteLine($"JSON error: {ex.Message}");
}
```

## Parser Options

Customize JSON parsing behavior:

```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    AllowTrailingCommas = true,
    ReadCommentHandling = JsonCommentHandling.Skip
};

var parser = new LinksetParser(options);
```

## Project Structure

```
Linkset/
├── src/
│   └── Linkset/
│       ├── Link.cs                    # Link model
│       ├── LinksetDocument.cs         # Document model with query helpers
│       ├── LinksetParser.cs           # Parser and serializer
│       └── LinksetParseException.cs   # Custom exception
└── tests/
    └── Linkset.Tests/
        ├── LinksetParserTests.cs      # Parser tests (25 tests)
        ├── LinksetSerializationTests.cs # Serialization tests (13 tests)
        ├── LinksetDocumentTests.cs    # Query helper tests (17 tests)
        └── RealWorldExamplesTests.cs  # Real-world examples (10 tests)
```

## Specification Compliance

This implementation follows:
- ✅ [RFC 9264](https://www.rfc-editor.org/rfc/rfc9264.html) - Linkset: Media Types and a Link Relation Type for Link Sets
- ✅ [RFC 8288](https://www.rfc-editor.org/rfc/rfc8288.html) - Web Linking

## Testing

Run all tests:
```bash
dotnet test
```

Run specific test classes:
```bash
dotnet test --filter "FullyQualifiedName~LinksetParserTests"
dotnet test --filter "FullyQualifiedName~RealWorldExamplesTests"
```

## Use Cases

- **API Documentation**: Link to specification documents and related resources
- **Specification References**: Document dependencies between standards (like this project)
- **Content Negotiation**: Provide alternative representations of resources
- **Resource Discovery**: Describe relationships between web resources
- **Metadata Exchange**: Share structured link information between systems

## License

This implementation is provided as-is for use in your projects.

## References

- [RFC 9264 - Linkset](https://www.rfc-editor.org/rfc/rfc9264.html)
- [RFC 8288 - Web Linking](https://www.rfc-editor.org/rfc/rfc8288.html)
- [IANA Media Type Registration](https://www.iana.org/assignments/media-types/application/linkset+json)
