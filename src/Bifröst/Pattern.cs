using System;
using System.Collections.Generic;
using System.Text;

namespace Bifröst
{
    public class Pattern
    {
        public string Value { get; }

        internal Pattern(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
        }

        public bool Matches(Topic topic)
        {
            if (topic is null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            return topic.Path.Equals(this.Value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
