using dmb_backend.Models;

namespace dmb_backend.Tests;

public class NewsItemModelTests
{
    [Fact]
    public void NewsItem_Title_CanBeSet()
    {
        // Arrange & Act
        var newsItem = new NewsItem { Title = "Test Title", Text = "Test Content" };

        // Assert
        Assert.Equal("Test Title", newsItem.Title);
    }

    [Fact]
    public void NewsItem_Text_CanBeSet()
    {
        // Arrange & Act
        var newsItem = new NewsItem { Title = "Test Title", Text = "Test Content" };

        // Assert
        Assert.Equal("Test Content", newsItem.Text);
    }

    [Fact]
    public void NewsItem_CreatedAtUtc_HasDefaultValue()
    {
        // Arrange & Act
        var newsItem = new NewsItem { Title = "Test", Text = "Content" };

        // Assert
        Assert.NotEqual(default(DateTime), newsItem.CreatedAtUtc);
    }

    [Fact]
    public void NewsItem_CreatedByUserId_CanBeSet()
    {
        // Arrange & Act
        var newsItem = new NewsItem 
        { 
            Title = "Test", 
            Text = "Content",
            CreatedByUserId = "user-123" 
        };

        // Assert
        Assert.Equal("user-123", newsItem.CreatedByUserId);
    }

    [Fact]
    public void NewsItem_AllPropertiesCanBeManipulated()
    {
        // Arrange
        var now = DateTime.UtcNow;
        
        // Act
        var newsItem = new NewsItem 
        { 
            Id = 42,
            Title = "Breaking News", 
            Text = "Important content here",
            CreatedAtUtc = now,
            CreatedByUserId = "admin-user"
        };

        // Assert
        Assert.Equal(42, newsItem.Id);
        Assert.Equal("Breaking News", newsItem.Title);
        Assert.Equal("Important content here", newsItem.Text);
        Assert.Equal(now, newsItem.CreatedAtUtc);
        Assert.Equal("admin-user", newsItem.CreatedByUserId);
    }

    [Fact]
    public void NewsItem_SupportsLongTitles()
    {
        // Arrange
        var longTitle = new string('a', 140);
        
        // Act
        var newsItem = new NewsItem { Title = longTitle, Text = "Content" };

        // Assert
        Assert.Equal(140, newsItem.Title.Length);
    }

    [Fact]
    public void NewsItem_SupportsLongText()
    {
        // Arrange
        var longText = new string('b', 4000);
        
        // Act
        var newsItem = new NewsItem { Title = "Title", Text = longText };

        // Assert
        Assert.Equal(4000, newsItem.Text.Length);
    }
}
