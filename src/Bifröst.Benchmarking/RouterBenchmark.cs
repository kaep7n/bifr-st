using BenchmarkDotNet.Attributes;
using Bifröst.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bifröst.Benchmarking
{
    public class RouterBenchmark
    {
        [Benchmark]
        public void Bench()
        {
            var buffer = new List<IEvent>();

            for (var i = 0; i < 100000; i++)
            {
                buffer.Add(new TestEvent("/", "Test"));
            }

            var router = new Router();

            var subscription = new TestSubscription("/");
            router.Subscribe(subscription);

            foreach (var evt in buffer)
            {
                router.Enqueue(evt);
            }
        }
    }
}
