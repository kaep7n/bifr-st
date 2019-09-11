using System.Collections.Generic;

namespace Bifröst
{
    public class TopicBuilder
    {
        private readonly List<string> path = new List<string>();

        public TopicBuilder(string root)
        {
            this.path.Add(root);
        }

        public TopicBuilder With(string path)
        {
            this.path.Add(path);
            return this;
        }

        public Topic Build()
        {
            var path = string.Join("/", this.path)
                .Insert(0, "/");

            return new Topic(path);
        }
    }
}
