using Xunit;

namespace Ysm.Core.Tests
{
    public class UrlHelperTests
    {
        [Fact]
        public void GetFeedUrl_Should_Return_Valid_Result()
        {
            // Arrange
            string expected = "https://www.youtube.com/feeds/videos.xml?channel_id=UCNd0qqcBpuXCWPM76lDUxqg";

            string channelId = "UCNd0qqcBpuXCWPM76lDUxqg";

            // Act
            string result = UrlHelper.GetFeedUrl(channelId);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetFeedUrl_Should_Return_Null_For_Empty_String()
        {
            // Arrange
            string channelId = "";

            // Act
            string result = UrlHelper.GetFeedUrl(channelId);

            // Assert
            Assert.Null(result);
        }
    }
}
