using Xunit;
using static Xunit.Assert;

namespace VotingSystem.Tests
{
    public class CounterTests
    {
        public const string CounterName = "CounterName";
        public Counter _counter = new Counter { Name = CounterName};

        [Fact]
        public void HasName()
        {
            Equal(CounterName, _counter.Name);
        }

        [Fact]
        public void GetStatistics_IncludesCounterName()
        {
            var name = "Counter Name";
            var counter = new Counter();
            counter.Name = name;

            var statistics = counter.GetStatistics();
            Equal(CounterName, statistics.Name);
        }

        [Fact]
        public void GetStatistics_IncludesCounterCount()
        {
            var statitics = _counter.GetStatistics();
            Equal(CounterName, statitics.Name);
        }
    }

    public class Counter
    {
        public string Name { get; set; }
        public Counter()
        {

        }

        internal Counter GetStatistics()
        {
            return this;
        }
    }
}
