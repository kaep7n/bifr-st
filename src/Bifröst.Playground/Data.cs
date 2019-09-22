using System;

namespace Bifröst.Playground
{
    public class Data
    {
        public Data(string key, int value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public int Value { get; }
    }
}
