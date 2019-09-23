using Xunit;

namespace Bifröst.Tests
{
    public class PatternTests
    {
        [Fact]
        public void Builder_with_root_path_should_create_topic_with_leading_slash()
        {
            const string expectedPath = "/root";

            var pattern = new PatternBuilder("root")
                .Build();

            Assert.Equal(expectedPath, pattern.Value);
        }
    }
}
