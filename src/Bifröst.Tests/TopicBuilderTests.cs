using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Bifröst.Tests
{
    public class TopicBuilderTests
    {
        [Fact]
        public void Test()
        {
            var expectedPath = "/root";

            var topic = new TopicBuilder("root")
                .Build();

            Assert.Equal(expectedPath, topic.Path);
        }
    }
}
