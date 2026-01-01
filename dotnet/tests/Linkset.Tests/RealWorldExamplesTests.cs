using Xunit;

namespace Linkset.Tests;

public class RealWorldExamplesTests
{
    [Fact]
    public void Parse_RFC9264Example_Success()
    {
        // Arrange - Example from RFC 9264 Section 4.2
        var json = @"{
            ""linkset"": [
                {
                    ""anchor"": ""https://example.org/resource"",
                    ""href"": ""https://example.org/resource/description"",
                    ""rel"": ""describedby"",
                    ""type"": ""text/html""
                },
                {
                    ""anchor"": ""https://example.org/resource"",
                    ""href"": ""https://example.org/resource/metadata"",
                    ""rel"": ""describedby"",
                    ""type"": ""application/rdf+xml""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal(2, document.Linkset.Count);
        Assert.All(document.Linkset, link =>
        {
            Assert.Equal("https://example.org/resource", link.Anchor);
            Assert.Equal("describedby", link.Rel);
        });
    }

    [Fact]
    public void Parse_iCalendarSpecsExample_Success()
    {
        // Arrange - Similar to the iCalendar specs.json from the project
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://www.rfc-editor.org/rfc/rfc5545.html"",
                    ""type"": ""text/html"",
                    ""rel"": ""describedby"",
                    ""title"": ""Internet Calendaring and Scheduling Core Object Specification (iCalendar)""
                },
                {
                    ""href"": ""https://www.rfc-editor.org/rfc/rfc5546.html"",
                    ""type"": ""text/html"",
                    ""rel"": ""describedby"",
                    ""title"": ""iCalendar Transport-Independent Interoperability Protocol (iTIP)""
                },
                {
                    ""href"": ""https://www.iana.org/assignments/media-types/text/calendar"",
                    ""type"": ""text/html"",
                    ""rel"": ""related"",
                    ""title"": ""IANA Media Type Registration for text/calendar""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal(3, document.Linkset.Count);

        var describedByLinks = document.GetLinksByRel("describedby");
        Assert.Equal(2, describedByLinks.Count);

        var relatedLinks = document.GetLinksByRel("related");
        Assert.Single(relatedLinks);
        Assert.Contains("IANA", relatedLinks[0].Title);
    }

    [Fact]
    public void Parse_RateLimiterSpecsExample_Success()
    {
        // Arrange - The RateLimiter specs.json from the project
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://datatracker.ietf.org/doc/draft-ietf-httpapi-ratelimit-headers/"",
                    ""type"": ""text/html"",
                    ""rel"": ""describedby"",
                    ""title"": ""RateLimit Header Fields for HTTP""
                },
                {
                    ""href"": ""https://www.rfc-editor.org/rfc/rfc9651.html"",
                    ""type"": ""text/html"",
                    ""rel"": ""describedby"",
                    ""title"": ""Structured Field Values for HTTP""
                },
                {
                    ""href"": ""https://www.rfc-editor.org/rfc/rfc9110.html#name-retry-after"",
                    ""type"": ""text/html"",
                    ""rel"": ""related"",
                    ""title"": ""HTTP Semantics - Retry-After Header""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal(3, document.Linkset.Count);
        Assert.Equal(2, document.GetLinksByRel("describedby").Count);
        Assert.Single(document.GetLinksByRel("related"));
    }

    [Fact]
    public void Parse_LinksetWithMultipleMediaTypes_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.org/doc"",
                    ""rel"": ""alternate"",
                    ""type"": ""application/pdf"",
                    ""title"": ""PDF Version""
                },
                {
                    ""href"": ""https://example.org/doc"",
                    ""rel"": ""alternate"",
                    ""type"": ""text/html"",
                    ""title"": ""HTML Version""
                },
                {
                    ""href"": ""https://example.org/doc"",
                    ""rel"": ""alternate"",
                    ""type"": ""application/epub+zip"",
                    ""title"": ""EPUB Version""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal(3, document.Linkset.Count);
        Assert.All(document.Linkset, link => Assert.Equal("alternate", link.Rel));

        var pdfLinks = document.GetLinksByType("application/pdf");
        Assert.Single(pdfLinks);
        Assert.Equal("PDF Version", pdfLinks[0].Title);
    }

    [Fact]
    public void Parse_LinksetWithLanguageVariants_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.org/doc/en"",
                    ""rel"": ""alternate"",
                    ""hreflang"": ""en"",
                    ""title"": ""English Version""
                },
                {
                    ""href"": ""https://example.org/doc/fr"",
                    ""rel"": ""alternate"",
                    ""hreflang"": ""fr"",
                    ""title"": ""French Version""
                },
                {
                    ""href"": ""https://example.org/doc/de"",
                    ""rel"": ""alternate"",
                    ""hreflang"": ""de"",
                    ""title"": ""German Version""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal(3, document.Linkset.Count);
        var languages = document.Linkset.Select(l => l.Hreflang).ToList();
        Assert.Contains("en", languages);
        Assert.Contains("fr", languages);
        Assert.Contains("de", languages);
    }

    [Fact]
    public void Parse_LinksetWithContentLength_Success()
    {
        // Arrange
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.org/video.mp4"",
                    ""rel"": ""alternate"",
                    ""type"": ""video/mp4"",
                    ""length"": 52428800,
                    ""title"": ""Video File (50MB)""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        var link = document.Linkset[0];
        Assert.Equal(52428800, link.Length);
        Assert.Equal("video/mp4", link.Type);
    }

    [Fact]
    public void Serialize_ThenParse_iCalendarExample_RoundTrip()
    {
        // Arrange
        var originalDocument = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link
                {
                    Href = "https://www.rfc-editor.org/rfc/rfc5545.html",
                    Type = "text/html",
                    Rel = "describedby",
                    Title = "Internet Calendaring and Scheduling Core Object Specification (iCalendar)"
                },
                new Link
                {
                    Href = "https://www.iana.org/assignments/media-types/text/calendar",
                    Type = "text/html",
                    Rel = "related",
                    Title = "IANA Media Type Registration for text/calendar"
                }
            }
        };
        var parser = new LinksetParser();

        // Act
        var json = parser.Serialize(originalDocument);
        var parsedDocument = parser.Parse(json);

        // Assert
        Assert.Equal(originalDocument.Linkset.Count, parsedDocument.Linkset.Count);
        Assert.Equal(2, parsedDocument.GetLinksByRel("describedby").Count + parsedDocument.GetLinksByRel("related").Count);
    }

    [Fact]
    public void Parse_LinksetWithMixedRelationTypes_Success()
    {
        // Arrange - Mix of standard and extension relation types
        var json = @"{
            ""linkset"": [
                {
                    ""href"": ""https://example.org/license"",
                    ""rel"": ""license""
                },
                {
                    ""href"": ""https://example.org/author"",
                    ""rel"": ""author""
                },
                {
                    ""href"": ""https://example.org/custom"",
                    ""rel"": ""https://example.org/rels/custom""
                }
            ]
        }";
        var parser = new LinksetParser();

        // Act
        var document = parser.Parse(json);

        // Assert
        Assert.Equal(3, document.Linkset.Count);
        var relationTypes = document.GetAllRelationTypes();
        Assert.Contains("license", relationTypes);
        Assert.Contains("author", relationTypes);
        Assert.Contains("https://example.org/rels/custom", relationTypes);
    }
}
