using System;
using System.Collections.Generic;
using System.Linq;
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
            var statistics = _counter.GetStatistics(5);
            Equal(CounterName, statistics.Name);
        }

        [Fact]
        public void GetStatistics_IncludesCounterCount()
        {
            var statitics = _counter.GetStatistics(5);
            Equal(5, statitics.Count);
        }

        [Theory]
        [InlineData(5, 10, 50)]
        [InlineData(1, 3, 33.33)]
        [InlineData(2, 3,66.67)]
        [InlineData(1, 8, 25)]
        public void GetStatistics_ShowsPercentageUpToTwoDecimalBasedOnTotalCount(int count, int total, double expected)
        {
            _counter.Count = count;
            var statistics = _counter.GetStatistics(10);
            Equal(expected, statistics.Percent);
        }

        [Fact]
        public void ResolveExcess_DoesntAddExcessWhenAllCountersAreEqual()
        {
            var counter1 = new Counter { Count = 1, Percent = 33.33};
            var counter2 = new Counter { Count = 1, Percent = 33.33};
            var counter3 = new Counter { Count = 1, Percent = 33.33};

            var counters = new List<Counter> { counter1, counter2, counter3 };
            new CounterManager().ResolveExcess(counters);

            Equal(33.33, counter1.Percent);
            Equal(33.33, counter2.Percent);
            Equal(33.33, counter3 .Percent);
        }

        [Theory]
        [InlineData(66.66, 66,67)]
        [InlineData(66.65, 66,68)]
        public void ResolveExcess_AddsExcessToHighestCounter(double initial, double expected, double lowest)
        {
            var counter1 = new Counter { Count = 2, Percent = initial };
            var counter2 = new Counter { Count = 1, Percent = lowest };
            var counters = new List<Counter> { counter1, counter2 };

            new CounterManager().ResolveExcess(counters);

            Equal(expected, counter1.Percent);
            Equal(lowest, counter2.Percent);
        }

        //[Fact]
        //[InlineData(11.11, 11.12, 44.44)]
        //[InlineData(11.11, 11.12, 44.44)]
        //public void ResolveExcess_AddsExcessToLowestCounterWhenMoreThanOneHighestCounters(double initial, double highest)
        //{
        //    var counter1 = new Counter {  Percent = highest };
        //    var counter2 = new Counter {  Percent = highest };
        //    var counter3 = new Counter {  Percent = initial };

        //    var counters = new List<Counter> { counter1, counter2, counter3 };

        //    new CounterManager().ResolveExcess(counters);

        //    Equal(highest, counter1.Percent);
        //    Equal(highest, counter2.Percent);
        //    Equal(initial, counter3.Percent);
        //}

        [Fact]
        public void ResolveExcess_DoesntAddExcessIfTotalPercentIs100()
        {
            var counter1 = new Counter { Count = 4, Percent = 80 };
            var counter2 = new Counter { Count = 1, Percent = 20 };
            var counters = new List<Counter> { counter1, counter2 };

            new CounterManager().ResolveExcess(counters);

            Equal(80, counter1.Percent);
            Equal(20, counter2.Percent);

        }
    }

    public class Counter
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public double Percent { get; set; }

        internal Counter GetStatistics(int totalCount)
        {
            Percent = CounterManager.RoundUp(Count * 100.0 / totalCount);
            return this;
        }
    }

    public class CounterManager
    {
        public void ResolveExcess(List<Counter> counters)
        {
            if (counters.Sum(x => x.Percent) == 100) return;

            var excess = 100;

            var highestPercent = counters.Max(x => x.Percent);
            var highestCounters = counters.Where(x => x.Percent == highestPercent).ToList();

            if (highestCounters.Count == 1)
            {
                counters[0].Percent += excess;
            }
            else if (highestCounters.Count < counters.Count)
            {
                var lowestPercent = counters.Min(x => x.Percent);
                var lowestcounters = counters.First(x => x.Percent == lowestPercent);
                lowestcounters.Percent = RoundUp(lowestcounters.Percent + excess);
            }
        }

        public static double RoundUp(double num) => Math.Round(num, 2);
    }
}
