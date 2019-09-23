using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
