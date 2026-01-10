using Xunit;

namespace Linkset.Tests;

public class LinksetParserTests
{
    [Fact]
    public void Parse_ValidLinksetWithSingleLink_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com"",
                    ""rel"": ""describedby"",
                    ""type"": ""text/html"",
                    ""title"": ""Example Link""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.NotNull(document);
        Assert.Single(document.Linkset);
        Assert.Equal("https://example.com", document.Linkset![0].Href);
        Assert.Equal("describedby", document.Linkset![0].Rel);
        Assert.Equal("text/html", document.Linkset![0].Type);
        Assert.Equal("Example Link", document.Linkset![0].Title);
    }

    [Fact]
    public void Parse_ValidLinksetWithMultipleLinks_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com/1"",
                    ""rel"": ""describedby""
                },
                {
                    ""href"": ""https://example.com/2"",
                    ""rel"": ""related""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(2, document.Linkset.Count);
        Assert.Equal("https://example.com/1", document.Linkset![0].Href);
        Assert.Equal("https://example.com/2", document.Linkset![1].Href);
    }

    [Fact]
    public void Parse_EmptyLinkset_Success()
    {
        // Arrange
        var json = @"{""linkset"": []}";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.NotNull(document);
        Assert.Empty(document.Linkset);
    }

    [Fact]
    public void Parse_LinkWithAllProperties_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com/doc.pdf"",
                    ""rel"": ""describedby"",
                    ""anchor"": ""https://example.com/context"",
                    ""type"": ""application/pdf"",
                    ""hreflang"": ""en"",
                    ""title"": ""Example Document"",
                    ""length"": 12345
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.NotNull(document);
        var link = document.Linkset![0];
        Assert.Equal("https://example.com/doc.pdf", link.Href);
        Assert.Equal("describedby", link.Rel);
        Assert.Equal("https://example.com/context", link.Anchor);
        Assert.Equal("application/pdf", link.Type);
        Assert.Equal("en", link.Hreflang);
        Assert.Equal("Example Document", link.Title);
        Assert.Equal(12345, link.Length);
    }

    [Fact]
    public void Parse_LinkWithOnlyHref_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.NotNull(document);
        var link = document.Linkset![0];
        Assert.Equal("https://example.com", link.Href);
        Assert.Null(link.Rel);
        Assert.Null(link.Type);
        Assert.Null(link.Title);
    }

    [Fact]
    public void Parse_NullJson_ThrowsArgumentNullException()
    {
        // Arrange
        var parser = new LinksetParser();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => parser.Parse(null!));
    }

    [Fact]
    public void Parse_EmptyJson_ThrowsArgumentNullException()
    {
        // Arrange
        var parser = new LinksetParser();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => parser.Parse(""));
    }

    [Fact]
    public void Parse_WhitespaceJson_ThrowsArgumentNullException()
    {
        // Arrange
        var parser = new LinksetParser();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => parser.Parse("   "));
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsLinksetParseException()
    {
        // Arrange
        var json = @"{invalid json}";
        var parser = new LinksetParser();

        // Act & Assert
        var ex = Assert.Throws<LinksetParseException>(() => parser.Parse(json));
        Assert.Contains("Invalid JSON format", ex.Message);
    }

    [Fact]
    public void Parse_MissingLinksetProperty_ThrowsLinksetParseException()
    {
        // Arrange
        var json = @"{}";
        var parser = new LinksetParser();

        // Act & Assert
        var ex = Assert.Throws<LinksetParseException>(() => parser.Parse(json));
        Assert.Contains("linkset", ex.Message.ToLower());
    }

    [Fact]
    public void Parse_LinkMissingHref_ThrowsLinksetParseException()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""rel"": ""describedby""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act & Assert
        var ex = Assert.Throws<LinksetParseException>(() => parser.Parse(json));
        Assert.Contains("href", ex.Message.ToLower());
    }

    [Fact]
    public void Parse_LinkWithEmptyHref_ThrowsLinksetParseException()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": """"
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act & Assert
        Assert.Throws<LinksetParseException>(() => parser.Parse(json));
    }

    [Fact]
    public void Parse_LinkWithInvalidHref_ThrowsLinksetParseException()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""not a valid uri ://""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act & Assert
        var ex = Assert.Throws<LinksetParseException>(() => parser.Parse(json));
        Assert.Contains("invalid", ex.Message.ToLower());
    }

    [Fact]
    public void Parse_LinkWithRelativeHref_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""/relative/path""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal("/relative/path", document.Linkset![0].Href);
    }

    [Fact]
    public void Parse_CaseInsensitivePropertyNames_Success()
    {
        // Arrange
        var json = @"{
            ""LINKSET"": [
                {
                    ""HREF"": ""https://example.com"",
                    ""REL"": ""describedby""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Single(document.Linkset);
        Assert.Equal("https://example.com", document.Linkset![0].Href);
    }

    [Fact]
    public void Parse_JsonWithTrailingCommas_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com"",
                    ""rel"": ""describedby"",
                },
            ],
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Single(document.Linkset);
    }

    [Fact]
    public void Parse_JsonWithComments_Success()
    {
        // Arrange
        var json = @"{
            // This is a comment
            ""linkset"": [
                {
                    ""href"": ""https://example.com""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Single(document.Linkset);
    }

    [Fact]
    public void Parse_LinkWithExtensionData_PreservesExtensions()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.com"",
                    ""customProperty"": ""customValue""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.NotNull(document);
        var link = document.Linkset![0];
        Assert.NotNull(link.ExtensionData);
        Assert.True(link.ExtensionData.ContainsKey("customProperty"));
    }
}
