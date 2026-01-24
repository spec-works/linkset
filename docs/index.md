# Linkset Documentation

Parse and serialize linkset documents according to [RFC 9264](https://www.rfc-editor.org/rfc/rfc9264) and [RFC 8288](https://www.rfc-editor.org/rfc/rfc8288).

## What is Linkset?

Linkset is a .NET library that provides support for parsing and serializing linkset documents. Linksets are structured collections of web links that can be represented in JSON format, enabling efficient discovery and processing of link relationships between web resources.

## Installation

Install via NuGet:

```bash
dotnet add package SpecWorks.Linkset
```

## Features

- ✅ **RFC 9264 Compliant** - Full implementation of Linkset specification
- ✅ **RFC 8288 Compliant** - Web Linking support
- ✅ **Parse Linkset Documents** - Parse JSON linkset documents
- ✅ **Serialize Linksets** - Generate valid linkset JSON
- ✅ **Link Validation** - Validate link structure and relationships
- ✅ **Type-Safe API** - Strong typing with nullable reference types
- ✅ **Multi-Target** - Supports .NET 10.0 and .NET 8.0 (LTS)

## Quick Start

### Parsing Linkset Documents

```csharp
using SpecWorks.Linkset;

// Parse from JSON string
string linksetJson = @"{
  ""linkset"": [
    {
      ""anchor"": ""https://example.com/resource"",
      ""item"": [
        {
          ""href"": ""https://example.com/chapter1"",
          ""type"": ""text/html""
        }
      ]
    }
  ]
}";

var linksetDocument = LinksetParser.Parse(linksetJson);

// Access linksets
foreach (var linkset in linksetDocument.Linksets)
{
    Console.WriteLine($"Anchor: {linkset.Anchor}");

    foreach (var link in linkset.Links)
    {
        Console.WriteLine($"  Link: {link.Href}");
        Console.WriteLine($"  Relation: {link.Relation}");
        Console.WriteLine($"  Type: {link.Type}");
    }
}
```

### Creating Linkset Documents

```csharp
using SpecWorks.Linkset;

// Create a linkset document
var linksetDocument = new LinksetDocument();

// Create a linkset with an anchor
var linkset = new Linkset
{
    Anchor = new Uri("https://example.com/resource")
};

// Add links
linkset.AddLink(new Link
{
    Href = new Uri("https://example.com/chapter1"),
    Relation = "item",
    Type = "text/html",
    Title = "Chapter 1"
});

linkset.AddLink(new Link
{
    Href = new Uri("https://example.com/chapter2"),
    Relation = "item",
    Type = "text/html",
    Title = "Chapter 2"
});

linksetDocument.Linksets.Add(linkset);

// Serialize to JSON
string json = linksetDocument.ToJson();
Console.WriteLine(json);
```

### Working with Link Relations

```csharp
using SpecWorks.Linkset;

var linksetDocument = LinksetParser.Parse(linksetJson);

// Find links by relation type
var itemLinks = linksetDocument
    .GetLinksByRelation("item")
    .ToList();

foreach (var link in itemLinks)
{
    Console.WriteLine($"Item: {link.Href}");
}

// Find links by anchor
var anchorLinks = linksetDocument
    .GetLinksByAnchor(new Uri("https://example.com/resource"))
    .ToList();
```

## Use Cases

### Resource Discovery

Enable discovery of related resources:

```csharp
// Parse linkset from HTTP response
var response = await httpClient.GetAsync("/linkset");
var linksetJson = await response.Content.ReadAsStringAsync();
var linkset = LinksetParser.Parse(linksetJson);

// Discover related resources
var relatedResources = linkset
    .GetLinksByRelation("related")
    .Select(link => link.Href)
    .ToList();
```

### Navigation Structures

Represent navigation hierarchies:

```csharp
var linkset = new Linkset
{
    Anchor = new Uri("https://example.com/book")
};

// Table of contents
linkset.AddLink(new Link
{
    Href = new Uri("https://example.com/toc"),
    Relation = "contents",
    Type = "text/html"
});

// Chapters
for (int i = 1; i <= 10; i++)
{
    linkset.AddLink(new Link
    {
        Href = new Uri($"https://example.com/chapter{i}"),
        Relation = "item",
        Type = "text/html",
        Title = $"Chapter {i}"
    });
}
```

### API Link Collections

Provide structured link collections in APIs:

```csharp
// Create linkset for API response
var linkset = new Linkset
{
    Anchor = new Uri("https://api.example.com/users/123")
};

linkset.AddLink(new Link
{
    Href = new Uri("https://api.example.com/users/123"),
    Relation = "self"
});

linkset.AddLink(new Link
{
    Href = new Uri("https://api.example.com/users/123/posts"),
    Relation = "related",
    Title = "User Posts"
});

linkset.AddLink(new Link
{
    Href = new Uri("https://api.example.com/users/123/profile"),
    Relation = "alternate",
    Type = "text/html"
});

// Return as part of API response
return Ok(linkset.ToJson());
```

## API Reference

- [API Documentation](api/SpecWorks.Linkset.html) - Complete API reference

## Specification Compliance

This library implements:

- [RFC 9264 - Linkset: Media Types and a Link Relation Type for Link Sets](https://www.rfc-editor.org/rfc/rfc9264)
- [RFC 8288 - Web Linking](https://www.rfc-editor.org/rfc/rfc8288)

### Supported Features

| Feature | RFC Section | Status |
|---------|-------------|--------|
| Linkset JSON Format | RFC 9264, Section 4.2 | ✅ Supported |
| Link Target Attributes | RFC 8288, Section 3.4 | ✅ Supported |
| Link Context | RFC 8288, Section 3.2 | ✅ Supported |
| Link Relations | RFC 8288, Section 3.3 | ✅ Supported |
| Extension Relations | RFC 8288, Section 2.1.2 | ✅ Supported |

### Link Attributes

The library supports all standard link attributes:

- `href` - Link target URI (required)
- `rel` - Link relation type
- `anchor` - Link context URI
- `type` - Media type hint
- `title` - Human-readable title
- `hreflang` - Language of target resource
- `media` - Media query for the link

## Requirements

- .NET 10.0 or .NET 8.0 (LTS)
- C# 10.0 or later

## Source Code

View the source code on [GitHub](https://github.com/spec-works/linkset).

## Contributing

Contributions welcome! See the [repository](https://github.com/spec-works/linkset) for:
- Issue tracking
- Pull request guidelines
- Architecture Decision Records (ADRs)

## License

MIT License - see [LICENSE](https://github.com/spec-works/linkset/blob/main/LICENSE) for details.

## Links

- **GitHub Repository**: [github.com/spec-works/linkset](https://github.com/spec-works/linkset)
- **RFC 9264**: [rfc-editor.org/rfc/rfc9264](https://www.rfc-editor.org/rfc/rfc9264)
- **RFC 8288**: [rfc-editor.org/rfc/rfc8288](https://www.rfc-editor.org/rfc/rfc8288)
- **SpecWorks Factory**: [spec-works.github.io](https://spec-works.github.io)
