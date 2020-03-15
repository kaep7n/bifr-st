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

        public object Clone() => new Topic((string)this.Path.Clone());

        public override bool Equals(object obj) => this.Equals(obj as Topic);

        public bool Equals(Topic other) => other != null &&
                   this.Path == other.Path;

        public override int GetHashCode() => HashCode.Combine(this.Path);

        public override string ToString() => this.Path;
    }
}
