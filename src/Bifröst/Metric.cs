using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bifröst
{
    public struct Metric : IEquatable<Metric>
    {
        public Metric(string name, object value)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            this.Value = value;
            this.CreateAt = DateTimeOffset.UtcNow;
        }

        public string Name { get; }

        public object Value { get; }

        public DateTimeOffset CreateAt {get;}

        public override bool Equals(object obj) 
            => obj is Metric metric && this.Equals(metric);

        public bool Equals([AllowNull] Metric other) 
            => this.Name == other.Name 
            && EqualityComparer<object>.Default.Equals(this.Value, other.Value) 
            && this.CreateAt.Equals(other.CreateAt);
        
        public override int GetHashCode() 
            => HashCode.Combine(this.Name, this.Value, this.CreateAt);

        public static bool operator ==(Metric left, Metric right) 
            => left.Equals(right);

        public static bool operator !=(Metric left, Metric right) 
            => !(left == right);
    }
}
