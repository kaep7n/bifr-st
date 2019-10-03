using System;
using System.Collections.Generic;

namespace Bifröst
{
    public class PatternBuilder
    {
        private readonly List<string> fragments = new List<string>();

        public PatternBuilder()
        {
        }

        public PatternBuilder(string root)
        {
            PathValidator.ValidateAndThrow(root);

            this.fragments.Add(root);
        }

        public PatternBuilder FromTopic(Topic topic)
        {
            if (topic is null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            var fragments = topic.Path.Split("/", StringSplitOptions.RemoveEmptyEntries);

            this.fragments.AddRange(fragments);

            return this;
        }

        public PatternBuilder With(string fragment)
        {
            PathValidator.ValidateAndThrow(fragment);

            this.fragments.Add(fragment);

            return this;
        }

        public PatternBuilder WithWildcard()
        {
            this.fragments.Add("*");

            return this;
        }

        public Pattern Build()
        {
            var path = string.Join("/", this.fragments)
                .Insert(0, "/");

            return new Pattern(path);
        }
    }
}
