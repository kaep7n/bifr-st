using System;
using System.Collections.Generic;
using System.Text;

namespace Bifröst
{
    public class PatternBuilder
    {
        private readonly List<string> path = new List<string>();

        public PatternBuilder(string root)
        {
            this.path.Add(root);
        }

        public PatternBuilder With(string path)
        {
            this.path.Add(path);

            return this;
        }

        public PatternBuilder WithWildcard()
        {
            this.path.Add("*");

            return this;
        }

        public Pattern Build()
        {
            var path = string.Join("/", this.path)
                .Insert(0, "/");

            return new Pattern(path);
        }
    }
}
