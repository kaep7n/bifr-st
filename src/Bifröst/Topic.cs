using System;

namespace Bifröst
{
    public class Topic
    {
        internal Topic(string path)
        {
            this.Path = path;
        }

        public string Path { get; }
    }
}
