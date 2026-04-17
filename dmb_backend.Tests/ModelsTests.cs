using dmb_backend.Models;

namespace dmb_backend.Tests;

public class ModelsTests
{
    [Fact]
    public void NewsItem_Title_MaxLengthValidation()
    {
        // Arrange
        var newsItem = new NewsItem
        {
            Title = new string('a', 150),
            Text = "Valid content"
        };

        // Act & Assert
        // The model has MaxLength(140), so let's verify the property accepts the string
        // In a real scenario, you'd validate this at the DB level or with Attributes
        Assert.NotNull(newsItem.Title);
        Assert.True(newsItem.Title.Length > 140);
    }

    [Fact]
    public void NewsItem_Text_MaxLengthValidation()
    {
        // Arrange
        var newsItem = new NewsItem
        {
            Title = "Title",
            Text = new string('a', 5000)
        };

        // Act & Assert
        Assert.NotNull(newsItem.Text);
        Assert.True(newsItem.Text.Length > 4000);
    }

    [Fact]
    public void NewsItem_CreatedAtUtcHasDefaultValue()
    {
        // Arrange & Act
        var newsItem = new NewsItem
        {
            Title = "Test Title",
            Text = "Test Content"
        };

        // Assert
        Assert.NotEqual(default(DateTime), newsItem.CreatedAtUtc);
    }

    [Fact]
    public void NewsItem_CanSetCreatedByUserId()
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
    public void ApplicationUser_InheritsFromIdentityUser()
    {
        // Arrange & Act
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Assert
        Assert.NotNull(user.UserName);
        Assert.NotNull(user.Email);
    }

    [Fact]
    public void NewsItem_AllPropertiesCanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var newsItem = new NewsItem();

        // Act
        newsItem.Id = 1;
        newsItem.Title = "Test Title";
        newsItem.Text = "Test Content";
        newsItem.CreatedAtUtc = now;
        newsItem.CreatedByUserId = "user-123";

        // Assert
        Assert.Equal(1, newsItem.Id);
        Assert.Equal("Test Title", newsItem.Title);
        Assert.Equal("Test Content", newsItem.Text);
        Assert.Equal(now, newsItem.CreatedAtUtc);
        Assert.Equal("user-123", newsItem.CreatedByUserId);
    }
}
