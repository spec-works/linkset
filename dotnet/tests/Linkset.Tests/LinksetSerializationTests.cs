using System.Text;
using Xunit;

namespace Linkset.Tests;

public class LinksetSerializationTests
{
    [Fact]
    public void Serialize_ValidDocument_ReturnsJson()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link
                {
                    Href = "https://example.com",
                    Rel = "describedby",
                    Type = "text/html",
                    Title = "Example Link"
                }
            }
        };
        var parser = new LinksetParser();

        // Act
        var json = parser.Serialize(document);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("linkset", json);
        Assert.Contains("https://example.com", json);
        Assert.Contains("describedby", json);
    }

    [Fact]
    public void Serialize_EmptyLinkset_ReturnsJson()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>()
        };
        var parser = new LinksetParser();

        // Act
        var json = parser.Serialize(document);

        // Assert
        Assert.NotNull(json);
        Assert.Contains("linkset", json);
        Assert.Contains("[]", json);
    }

    [Fact]
    public void Serialize_NullDocument_ThrowsArgumentNullException()
    {
        // Arrange
        var parser = new LinksetParser();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => parser.Serialize(null!));
    }

    [Fact]
    public void Serialize_ThenParse_RoundTrip()
    {
        // Arrange
        var originalDocument = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link
                {
                    Href = "https://example.com/1",
                    Rel = "describedby",
                    Type = "text/html",
                    Title = "First Link"
                },
                new Link
                {
                    Href = "https://example.com/2",
                    Rel = "related",
                    Anchor = "https://example.com/context",
                    Length = 12345
                }
            }
        };
        var parser = new LinksetParser();

        // Act
        var json = parser.Serialize(originalDocument);
        var parsedDocument = parser.Parse(json);

        // Assert
        Assert.Equal(originalDocument.Linkset.Count, parsedDocument.Linkset.Count);

        for (int i = 0; i < originalDocument.Linkset.Count; i++)
        {
            Assert.Equal(originalDocument.Linkset[i].Href, parsedDocument.Linkset[i].Href);
            Assert.Equal(originalDocument.Linkset[i].Rel, parsedDocument.Linkset[i].Rel);
            Assert.Equal(originalDocument.Linkset[i].Type, parsedDocument.Linkset[i].Type);
            Assert.Equal(originalDocument.Linkset[i].Title, parsedDocument.Linkset[i].Title);
            Assert.Equal(originalDocument.Linkset[i].Anchor, parsedDocument.Linkset[i].Anchor);
            Assert.Equal(originalDocument.Linkset[i].Length, parsedDocument.Linkset[i].Length);
        }
    }

    [Fact]
    public void Serialize_OmitsNullValues()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link
                {
                    Href = "https://example.com",
                    Rel = null,
                    Type = null
                }
            }
        };
        var parser = new LinksetParser();

        // Act
        var json = parser.Serialize(document);

        // Assert
        Assert.DoesNotContain("\"rel\"", json);
        Assert.DoesNotContain("\"type\"", json);
    }

    [Fact]
    public async Task ParseAsync_ValidStream_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com"",
                    ""rel"": ""describedby""
                }
            ]
        }";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var parser = new LinksetParser();

        // Act
        var document = await parser.ParseAsync(stream);

        // Assert
        Assert.NotNull(document);
        Assert.Single(document.Linkset);
        Assert.Equal("https://example.com", document.Linkset[0].Href);
    }

    [Fact]
    public async Task ParseAsync_NullStream_ThrowsArgumentNullException()
    {
        // Arrange
        var parser = new LinksetParser();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => parser.ParseAsync(null!));
    }

    [Fact]
    public async Task ParseAsync_InvalidJson_ThrowsLinksetParseException()
    {
        // Arrange
        var json = @"{invalid}";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var parser = new LinksetParser();

        // Act & Assert
        await Assert.ThrowsAsync<LinksetParseException>(() => parser.ParseAsync(stream));
    }

    [Fact]
    public async Task SerializeAsync_ValidDocument_WritesToStream()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link
                {
                    Href = "https://example.com",
                    Rel = "describedby"
                }
            }
        };
        var parser = new LinksetParser();
        var stream = new MemoryStream();

        // Act
        await parser.SerializeAsync(document, stream);

        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        Assert.NotNull(json);
        Assert.Contains("linkset", json);
        Assert.Contains("https://example.com", json);
    }

    [Fact]
    public async Task SerializeAsync_NullDocument_ThrowsArgumentNullException()
    {
        // Arrange
        var parser = new LinksetParser();
        var stream = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => parser.SerializeAsync(null!, stream));
    }

    [Fact]
    public async Task SerializeAsync_NullStream_ThrowsArgumentNullException()
    {
        // Arrange
        var document = new LinksetDocument();
        var parser = new LinksetParser();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => parser.SerializeAsync(document, null!));
    }

    [Fact]
    public async Task SerializeAsync_ThenParseAsync_RoundTrip()
    {
        // Arrange
        var originalDocument = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link
                {
                    Href = "https://example.com",
                    Rel = "describedby",
                    Type = "text/html"
                }
            }
        };
        var parser = new LinksetParser();
        var stream = new MemoryStream();

        // Act
        await parser.SerializeAsync(originalDocument, stream);
        stream.Position = 0;
        var parsedDocument = await parser.ParseAsync(stream);

        // Assert
        Assert.Equal(originalDocument.Linkset.Count, parsedDocument.Linkset.Count);
        Assert.Equal(originalDocument.Linkset[0].Href, parsedDocument.Linkset[0].Href);
        Assert.Equal(originalDocument.Linkset[0].Rel, parsedDocument.Linkset[0].Rel);
    }
}
