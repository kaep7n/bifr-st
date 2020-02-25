using System;
using System.Collections.Generic;
using System.Linq;

namespace Bifröst
{
    public class TopicBuilder
    {
        private readonly List<string> fragments = new List<string>();

        public TopicBuilder(string root)
        {
            PathValidator.ValidateAndThrow(root);

            this.fragments.Add(root);
        }

        public TopicBuilder With(string fragment)
        {
            PathValidator.ValidateAndThrow(fragment);

            this.fragments.Add(fragment);

            return this;
        }

        public Topic Build()
        {
            var path = string.Join("/", this.fragments)
                .Insert(0, "/");

            return new Topic(path);
        }
    }
}
