using System;
using Xunit;

namespace Bifröst.Tests
{
    public class TopicTests
    {
        [Fact]
        public void Builder_with_root_path_should_create_topic_with_leading_slash()
        {
            var expectedPath = "/root";

            var topic = new TopicBuilder("root")
                .Build();

            Assert.Equal(expectedPath, topic.Path);
        }

        [Fact]
        public void Builder_with_deep_path_should_create_topic_with_delemitted_slashes()
        {
            var expectedPath = "/root/worker/config/changed";

            var topic = new TopicBuilder("root")
                .With("worker")
                .With("config")
                .With("changed")
                .Build();

            Assert.Equal(expectedPath, topic.Path);
        }

        [Fact]
        public void Builder_should_throw_ArgumentException_when_root_is_null_or_empty_string()
        {
            Assert.Throws<ArgumentException>(() => new TopicBuilder(null));
            Assert.Throws<ArgumentException>(() => new TopicBuilder(string.Empty));
        }

        [Fact]
        public void Builder_should_throw_ArgumentException_when_fragment_is_null_or_empty_string()
        {
            Assert.Throws<ArgumentException>(() => new TopicBuilder("root").With(null));
            Assert.Throws<ArgumentException>(() => new TopicBuilder("root").With(string.Empty));
        }

        [Theory]
        [InlineData("root/worker")] // slash
        [InlineData("root  ")] // whitespace
        [InlineData("   ")] // tab
        public void Builder_should_throw_ArgumentException_when_root_contains_invalid_characters_or_slash(string fragment) => Assert.Throws<ArgumentException>(() => new TopicBuilder(fragment));

        [Theory]
        [InlineData("root/worker")] // slash
        [InlineData("root  ")] // whitespace
        [InlineData("   ")] // tab
        public void Builder_should_throw_ArgumentException_when_fragment_contains_invalid_characters_or_slash(string fragment) => Assert.Throws<ArgumentException>(() => new TopicBuilder("root").With(fragment));
    }
}
