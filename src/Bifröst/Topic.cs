using System;

namespace Bifröst
{
    public class Topic : IEquatable<Topic>, ICloneable
    {
        internal Topic(string path)
        {
            this.Path = path;
        }

        public string Path { get; }

        public object Clone()
        {
            return new Topic((string)this.Path.Clone());
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Topic);
        }

        public bool Equals(Topic other)
        {
            return other != null &&
                   this.Path == other.Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Path);
        }
    }
}
