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

            var topicFragments = topic.Path.Split('/');
            var patternFragments = this.Value.Split('/');

            for (var i = 0; i < topicFragments.Length; i++)
            {
                var topicFragment = topicFragments[i];
                var patternFragment = patternFragments[i];

                if (patternFragment == "*")
                {
                    continue;
                }

                if (patternFragment != topicFragment)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
