using System;
using System.Collections.Generic;
using Xunit;

namespace Bifröst.Tests
{
    public class PatternTests
    {
        [Fact]
        public void Builder_with_root_path_should_create_pattern_with_leading_slash()
        {
            const string expectedPath = "/root";

            var pattern = new PatternBuilder("root")
                .Build();

            Assert.Equal(expectedPath, pattern.Value);
        }

        [Fact]
        public void Builder_with_deep_path_should_create_pattern_delimitted_by_slashes()
        {
            const string expectedPath = "/root/worker/config/changed";

            var pattern = new PatternBuilder("root")
                .With("worker")
                .With("config")
                .With("changed")
                .Build();

            Assert.Equal(expectedPath, pattern.Value);
        }

        [Fact]
        public void Builder_with_wildcard_should_create_pattern_delimitted_by_slashes()
        {
            const string expectedPath = "/root/worker/config/*";

            var pattern = new PatternBuilder("root")
                .With("worker")
                .With("config")
                .WithWildcard()
                .Build();

            Assert.Equal(expectedPath, pattern.Value);
        }

        [Theory]
        [InlineData("root/worker")] // slash
        [InlineData("root  ")] // whitespace
        [InlineData("   ")] // tab
        public void Builder_should_throw_ArgumentException_when_root_contains_invalid_characters(string fragment)
        {
            Assert.Throws<ArgumentException>(() => new PatternBuilder(fragment));
        }

        [Theory]
        [InlineData("root/worker")] // slash
        [InlineData("root  ")] // whitespace
        [InlineData("   ")] // tab
        public void Builder_should_throw_ArgumentException_when_fragment_contains_invalid_characters_or_slash(string fragment)
        {
            Assert.Throws<ArgumentException>(() => new PatternBuilder("root").With(fragment));
        }

        [Fact]
        public void Builder_should_throw_ArgumentException_when_root_is_null_or_empty_string()
        {
            Assert.Throws<ArgumentException>(() => new PatternBuilder(null));
            Assert.Throws<ArgumentException>(() => new PatternBuilder(string.Empty));
        }

        [Fact]
        public void Builder_should_throw_ArgumentException_when_fragment_is_null_or_empty_string()
        {
            Assert.Throws<ArgumentException>(() => new PatternBuilder("root").With(null));
            Assert.Throws<ArgumentException>(() => new PatternBuilder("root").With(string.Empty));
        }

        [Fact]
        public void Matches_should_return_true_for_exact_match()
        {
            var pattern = new PatternBuilder("root")
                .With("worker")
                .With("config")
                .Build();

            var topic = new TopicBuilder("root")
                .With("worker")
                .With("config")
                .Build();

            var matches = pattern.Matches(topic);
            Assert.True(matches);
        }
    }
}
