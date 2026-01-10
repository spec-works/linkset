using Xunit;

namespace Linkset.Tests;

public class LinksetDocumentTests
{
    [Fact]
    public void GetLinksByRel_ReturnsMatchingLinks()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Rel = "describedby" },
                new Link { Href = "https://example.com/2", Rel = "related" },
                new Link { Href = "https://example.com/3", Rel = "describedby" }
            }
        };

        // Act
        var links = document.GetLinksByRel("describedby");

        // Assert
        Assert.Equal(2, links.Count);
        Assert.All(links, link => Assert.Equal("describedby", link.Rel));
    }

    [Fact]
    public void GetLinksByRel_CaseInsensitive()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Rel = "describedby" },
                new Link { Href = "https://example.com/2", Rel = "DESCRIBEDBY" }
            }
        };

        // Act
        var links = document.GetLinksByRel("DescribedBy");

        // Assert
        Assert.Equal(2, links.Count);
    }

    [Fact]
    public void GetLinksByRel_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com", Rel = "describedby" }
            }
        };

        // Act
        var links = document.GetLinksByRel("nonexistent");

        // Assert
        Assert.Empty(links);
    }

    [Fact]
    public void GetLinksByRel_NullRel_ThrowsArgumentNullException()
    {
        // Arrange
        var document = new LinksetDocument();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => document.GetLinksByRel(null!));
    }

    [Fact]
    public void GetLinksByRel_EmptyRel_ThrowsArgumentNullException()
    {
        // Arrange
        var document = new LinksetDocument();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => document.GetLinksByRel(""));
    }

    [Fact]
    public void GetFirstLinkByRel_ReturnsFirstMatch()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Rel = "describedby" },
                new Link { Href = "https://example.com/2", Rel = "describedby" }
            }
        };

        // Act
        var link = document.GetFirstLinkByRel("describedby");

        // Assert
        Assert.NotNull(link);
        Assert.Equal("https://example.com/1", link.Href);
    }

    [Fact]
    public void GetFirstLinkByRel_NoMatch_ReturnsNull()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com", Rel = "describedby" }
            }
        };

        // Act
        var link = document.GetFirstLinkByRel("nonexistent");

        // Assert
        Assert.Null(link);
    }

    [Fact]
    public void GetFirstLinkByRel_CaseInsensitive()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com", Rel = "DESCRIBEDBY" }
            }
        };

        // Act
        var link = document.GetFirstLinkByRel("describedby");

        // Assert
        Assert.NotNull(link);
    }

    [Fact]
    public void GetLinksByType_ReturnsMatchingLinks()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Type = "text/html" },
                new Link { Href = "https://example.com/2", Type = "application/json" },
                new Link { Href = "https://example.com/3", Type = "text/html" }
            }
        };

        // Act
        var links = document.GetLinksByType("text/html");

        // Assert
        Assert.Equal(2, links.Count);
        Assert.All(links, link => Assert.Equal("text/html", link.Type));
    }

    [Fact]
    public void GetLinksByType_CaseInsensitive()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com", Type = "TEXT/HTML" }
            }
        };

        // Act
        var links = document.GetLinksByType("text/html");

        // Assert
        Assert.Single(links);
    }

    [Fact]
    public void GetLinksByType_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com", Type = "text/html" }
            }
        };

        // Act
        var links = document.GetLinksByType("application/pdf");

        // Assert
        Assert.Empty(links);
    }

    [Fact]
    public void GetAllRelationTypes_ReturnsDistinctTypes()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Rel = "describedby" },
                new Link { Href = "https://example.com/2", Rel = "related" },
                new Link { Href = "https://example.com/3", Rel = "describedby" },
                new Link { Href = "https://example.com/4", Rel = "alternate" }
            }
        };

        // Act
        var types = document.GetAllRelationTypes();

        // Assert
        Assert.Equal(3, types.Count);
        Assert.Contains("describedby", types);
        Assert.Contains("related", types);
        Assert.Contains("alternate", types);
    }

    [Fact]
    public void GetAllRelationTypes_CaseInsensitiveDistinct()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Rel = "describedby" },
                new Link { Href = "https://example.com/2", Rel = "DESCRIBEDBY" },
                new Link { Href = "https://example.com/3", Rel = "DescribedBy" }
            }
        };

        // Act
        var types = document.GetAllRelationTypes();

        // Assert
        Assert.Single(types);
    }

    [Fact]
    public void GetAllRelationTypes_IgnoresNullAndEmpty()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>
            {
                new Link { Href = "https://example.com/1", Rel = "describedby" },
                new Link { Href = "https://example.com/2", Rel = null },
                new Link { Href = "https://example.com/3", Rel = "" }
            }
        };

        // Act
        var types = document.GetAllRelationTypes();

        // Assert
        Assert.Single(types);
        Assert.Contains("describedby", types);
    }

    [Fact]
    public void GetAllRelationTypes_EmptyLinkset_ReturnsEmptyList()
    {
        // Arrange
        var document = new LinksetDocument();

        // Act
        var types = document.GetAllRelationTypes();

        // Assert
        Assert.Empty(types);
    }

    [Fact]
    public void Constructor_LinksetIsNull()
    {
        // Arrange & Act
        var document = new LinksetDocument();

        // Assert
        Assert.Null(document.Linkset!);
    }

    [Fact]
    public void Linkset_CanAddLinks()
    {
        // Arrange
        var document = new LinksetDocument
        {
            Linkset = new List<Link>()
        };

        // Act
        document.Linkset!.Add(new Link { Href = "https://example.com" });

        // Assert
        Assert.Single(document.Linkset!);
    }
}
