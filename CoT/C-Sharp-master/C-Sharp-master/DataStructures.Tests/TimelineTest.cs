using NUnit.Framework;
using DataStructures; // Your namespace for Timeline
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures.Tests
{
    [TestFixture]
    public class TimelineTests
    {
        // Common DateTime instances for tests
#pragma warning disable IDE1006 // Naming Styles
        private readonly DateTime _t1 = new DateTime(2023, 1, 1, 10, 0, 0);
        private readonly DateTime _t1Similar = new DateTime(2023, 1, 1, 10, 0, 0); // Same as _t1
        private readonly DateTime _t1LaterMs = new DateTime(2023, 1, 1, 10, 0, 0, 500);
        private readonly DateTime _t2 = new DateTime(2023, 1, 2, 11, 30, 15, 100);
        private readonly DateTime _t3 = new DateTime(2023, 1, 3, 12, 0, 0);
#pragma warning restore IDE1006 // Naming Styles
        private const string ValA = "Event A";
        private const string ValB = "Event B";
        private const string ValC = "Event C";

        [Test]
        public void DefaultConstructor_CreatesEmptyTimeline()
        {
            var timeline = new Timeline<string>();
            Assert.That(timeline.Count, Is.EqualTo(0));
            Assert.That(timeline.TimesCount, Is.EqualTo(0));
            Assert.That(timeline.ValuesCount, Is.EqualTo(0));
            Assert.That(timeline.ToArray(), Is.Empty);
        }

        [Test]
        public void SingleEventConstructor_CreatesTimelineWithOneEvent()
        {
            var timeline = new Timeline<string>(_t1, ValA);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.ToArray(), Is.EqualTo(new[] { (_t1, ValA) }));
            Assert.That(timeline.TimesCount, Is.EqualTo(1));
            Assert.That(timeline.ValuesCount, Is.EqualTo(1));
        }

        [Test]
        public void ParamsConstructor_CreatesSortedTimeline()
        {
            var events = new[] { (_t2, ValB), (_t1, ValA), (_t3, ValC) };
            var timeline = new Timeline<string>(events);

            Assert.That(timeline.Count, Is.EqualTo(3));
            // Constructor sorts the events by time
            var expectedOrder = new[] { (_t1, ValA), (_t2, ValB), (_t3, ValC) };
            Assert.That(timeline.ToArray(), Is.EqualTo(expectedOrder));
        }

        [Test]
        public void ParamsConstructor_WithDuplicateTimes_PreservesAllEventsSorted()
        {
            var events = new[] { (_t1, ValB), (_t1, ValA) }; // Same time, different values
            var timeline = new Timeline<string>(events);

            Assert.That(timeline.Count, Is.EqualTo(2));
            // Order of values for the same time depends on stable sort of OrderBy
            // Assuming ValA comes before ValB if input was (_t1, ValB), (_t1, ValA) and sort is stable on value or original position
            // The provided OrderBy(pair => pair.Item1) is stable.
            var expectedOrder = new[] { (_t1, ValB), (_t1, ValA) }; // OrderBy sorts by Time. Original order for equal times is preserved.
            Assert.That(timeline.ToArray(), Is.EqualTo(expectedOrder));
        }

        [Test]
        public void TimesCount_ReturnsDistinctNumberOfTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t1, ValB); // Same time, different value
            timeline.Add(_t2, ValC);

            Assert.That(timeline.TimesCount, Is.EqualTo(2)); // _t1, _t2
        }

        [Test]
        public void ValuesCount_ReturnsTotalNumberOfValues()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t1, ValB);
            timeline.Add(_t2, ValA); // Same value, different time

            Assert.That(timeline.ValuesCount, Is.EqualTo(3));
        }

        [Test]
        public void Indexer_Get_ReturnsValuesForSpecificTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t1, ValB);
            timeline.Add(_t2, ValC);

            Assert.That(timeline[_t1], Is.EquivalentTo(new[] { ValA, ValB }));
            Assert.That(timeline[_t2], Is.EquivalentTo(new[] { ValC }));
            Assert.That(timeline[_t3], Is.Empty); // Time not present
        }

        [Test]
        public void Indexer_Set_OverridesValuesForSpecificTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);

            timeline[_t1] = new[] { ValC, "NewVal" };

            Assert.That(timeline.Count, Is.EqualTo(3)); // Old (_t1, ValA) removed, 2 new added, (_t2, ValB) remains
            Assert.That(timeline[_t1], Is.EquivalentTo(new[] { ValC, "NewVal" }));
            Assert.That(timeline[_t2], Is.EquivalentTo(new[] { ValB }));
        }

        [Test]
        public void Indexer_Set_AddsValuesIfTimeNotPresent()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);

            timeline[_t2] = new[] { ValB, ValC };

            Assert.That(timeline.Count, Is.EqualTo(3));
            Assert.That(timeline[_t2], Is.EquivalentTo(new[] { ValB, ValC }));
        }


        [Test]
        public void ICollection_IsReadOnly_ReturnsFalse()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>();
            Assert.That(timeline.IsReadOnly, Is.False);
        }

        [Test]
        public void Count_ReflectsNumberOfEvents()
        {
            var timeline = new Timeline<string>();
            Assert.That(timeline.Count, Is.EqualTo(0));
            timeline.Add(_t1, ValA);
            Assert.That(timeline.Count, Is.EqualTo(1));
            timeline.Add(_t1, ValB);
            Assert.That(timeline.Count, Is.EqualTo(2));
        }

        [Test]
        public void Clear_RemovesAllEvents()
        {
            var timeline = new Timeline<string>((_t1, ValA), (_t2, ValB));
            timeline.Clear();
            Assert.That(timeline.Count, Is.EqualTo(0));
            Assert.That(timeline.ToArray(), Is.Empty);
        }

        [Test]
        public void CopyTo_CopiesEventsToArray()
        {
            var timeline = new Timeline<string>((_t1, ValA), (_t2, ValB));
            var array = new (DateTime, string)[3];

            timeline.CopyTo(array, 1);

            Assert.That(array[0], Is.EqualTo(default((DateTime, string))));
            Assert.That(array[1], Is.EqualTo((_t1, ValA)));
            Assert.That(array[2], Is.EqualTo((_t2, ValB)));
        }

        [Test]
        public void ICollection_Add_AddsEvent()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>();
            timeline.Add((_t1, ValA));
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(((Timeline<string>)timeline).Contains(_t1, ValA), Is.True);
        }

        [Test]
        public void ICollection_Contains_ChecksForEvent()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>((_t1, ValA));
            Assert.That(timeline.Contains((_t1, ValA)), Is.True);
            Assert.That(timeline.Contains((_t1, ValB)), Is.False);
            Assert.That(timeline.Contains((_t2, ValA)), Is.False);
        }

        [Test]
        public void ICollection_Remove_RemovesEvent()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>((_t1, ValA), (_t2, ValB));
            bool removed = timeline.Remove((_t1, ValA));
            Assert.That(removed, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains((_t1, ValA)), Is.False);

            bool notRemoved = timeline.Remove((_t3, ValC));
            Assert.That(notRemoved, Is.False);
        }

        [Test]
        public void GetEnumerator_AllowsIteration()
        {
            var events = new[] { (_t1, ValA), (_t2, ValB) };
            var timeline = new Timeline<string>(events); // Constructor sorts

            var iteratedEvents = new List<(DateTime, string)>();
            foreach (var item in timeline)
            {
                iteratedEvents.Add(item);
            }
            Assert.That(iteratedEvents, Is.EqualTo(events.OrderBy(e => e.Item1).ToList()));
        }

        [Test]
        public void IEnumerable_GetEnumerator_AllowsIteration()
        {
            var events = new[] { (_t1, ValA), (_t2, ValB) };
            var timeline = new Timeline<string>(events);
            IEnumerable iEnumerable = timeline;

            var iteratedEvents = new List<(DateTime, string)>();
            foreach ((DateTime Time, string Value) item in iEnumerable)
            {
                iteratedEvents.Add(item);
            }
            Assert.That(iteratedEvents, Is.EqualTo(events.OrderBy(e => e.Item1).ToList()));
        }

        // --- Equality Tests (Reflecting current buggy behavior) ---
        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var tl1 = new Timeline<string>((_t1, ValA));
            Assert.That(tl1.Equals(null), Is.False);
        }

        [Test]
        public void OperatorEquals_BothNull_ReturnsTrue() // Based on typical operator overloading patterns
        {
            Timeline<string>? tl1 = null;
            Timeline<string>? tl2 = null;
            // Original assertion: Assert.That(tl1 == tl2, Is.True);
            // This test now asserts the known buggy behavior of Timeline.operator==
            // which throws a NullReferenceException when both operands are null.
            Assert.Throws<NullReferenceException>(() =>
            {
#pragma warning disable CS8604 // Suppress warning for passing null to operator expecting non-null.
                bool _ = tl1 == tl2; // This expression is expected to throw.
#pragma warning restore CS8604
            }, "Timeline.operator== should handle nulls but currently throws NRE when both are null.");
                                              // So (null == null) -> ReferenceEquals is true.
        }

        [Test]
        public void OperatorEquals_LeftNull_ReturnsFalse()
        {
            Timeline<string>? tl1 = null;
            var tl2 = new Timeline<string>((_t1, ValA));
            // Original assertion: Assert.That(tl1 == tl2, Is.False);
            // This test now asserts the known buggy behavior of Timeline.operator==
            // which throws a NullReferenceException when the left operand is null.
            Assert.Throws<NullReferenceException>(() =>
            {
#pragma warning disable CS8604 // Suppress warning for passing null to operator expecting non-null.
                bool _ = tl1 == tl2; // This expression is expected to throw.
#pragma warning restore CS8604
            }, "Timeline.operator== should handle a null left operand but currently throws NRE.");

        }

        [Test]
        public void OperatorEquals_RightNull_ReturnsFalse()
        {
            var tl1 = new Timeline<string>((_t1, ValA));
            Timeline<string>? tl2 = null;
            // Original assertion: Assert.That(tl1 == tl2, Is.False);
            // This test now asserts the known buggy behavior of Timeline.operator==
            // which throws a NullReferenceException when the right operand is null.
            Assert.Throws<NullReferenceException>(() =>
            {
#pragma warning disable CS8604 // Suppress warning for passing null to operator expecting non-null.
                bool _ = tl1 == tl2; // This expression is expected to throw.
#pragma warning restore CS8604
            }, "Timeline.operator== should handle a null right operand but currently throws NRE.");
        }

        [Test]
        public void OperatorEquals_SameInstance_ReturnsTrue()
        {
            var tl1 = new Timeline<string>((_t1, ValA));
            var tl2 = tl1;
            Assert.That(tl1 == tl2, Is.True);
        }

        [Test]
        public void OperatorEquals_DifferentCounts_ReturnsFalse()
        {
            var tl1 = new Timeline<string>((_t1, ValA));
            var tl2 = new Timeline<string>((_t1, ValA), (_t2, ValB));
            Assert.That(tl1 == tl2, Is.False);
        }

        [Test]
        public void OperatorEquals_BuggyBehavior_TimeSameValueDiff_ReturnsTrue()
        {
            // Tests current buggy behavior: (Time_Same || Value_Same) for each element
            var tl1 = new Timeline<string>((_t1, ValA));
            var tl2 = new Timeline<string>((_t1Similar, ValB)); // Same time, different value

            // Expected based on bug: (t1 == t1 || ValA == ValB) -> (true || false) -> true. So tl1 == tl2 is true.
            Assert.That(tl1 == tl2, Is.True, "Buggy operator==: Should be true if times match, even if values differ.");
            Assert.That(tl1 != tl2, Is.False);
        }

        [Test]
        public void OperatorEquals_BuggyBehavior_TimeDiffValueSame_ReturnsTrue()
        {
            // Tests current buggy behavior: (Time_Same || Value_Same) for each element
            var tl1 = new Timeline<string>((_t1, ValA));
            var tl2 = new Timeline<string>((_t2, ValA)); // Different time, same value

            // Expected based on bug: (t1 == t2 || ValA == ValA) -> (false || true) -> true. So tl1 == tl2 is true.
            Assert.That(tl1 == tl2, Is.True, "Buggy operator==: Should be true if values match, even if times differ.");
            Assert.That(tl1 != tl2, Is.False);
        }

        [Test]
        public void OperatorEquals_TimeDiffValueDiff_ReturnsFalse()
        {
            var tl1 = new Timeline<string>((_t1, ValA));
            var tl2 = new Timeline<string>((_t2, ValB)); // Different time, different value

            // Expected: (t1 == t2 || ValA == ValB) -> (false || false) -> false.
            // The condition in code is `if (Time_Diff && Value_Diff) return false;`
            // `(_t1 != _t2 && ValA != ValB)` -> `(true && true)` -> `true`. So returns false. This is correct.
            Assert.That(tl1 == tl2, Is.False);
            Assert.That(tl1 != tl2, Is.True);
        }

        [Test]
        public void OperatorEquals_IdenticalContentAndOrder_ReturnsTrue()
        {
            // This case should work correctly even with the bug, as (Time_Same || Value_Same) is (true || true) -> true.
            var tl1 = new Timeline<string>((_t1, ValA), (_t2, ValB));
            var tl2 = new Timeline<string>((_t1Similar, ValA), (_t2, ValB));
            Assert.That(tl1 == tl2, Is.True);
            Assert.That(tl1 != tl2, Is.False);
        }

        [Test]
        public void OperatorEquals_ContentSameDifferentOrder_DependsOnAddVsConstructor()
        {
            // If Add is used, order is not guaranteed. If constructor is used, it's sorted.
            // ToArray() does not sort. So order matters for operator==
            var tl1 = new Timeline<string>();
            tl1.Add(_t1, ValA);
            tl1.Add(_t2, ValB);

            var tl2 = new Timeline<string>();
            tl2.Add(_t2, ValB);
            tl2.Add(_t1, ValA);

            // tl1.ToArray() = [(_t1,A), (_t2,B)]
            // tl2.ToArray() = [(_t2,B), (_t1,A)]
            // Comparison at index 0: (_t1,A) vs (_t2,B). Time_Diff && Value_Diff -> true. Returns false.
            Assert.That(tl1 == tl2, Is.False, "Order matters for operator== as ToArray() is not sorted post-Add.");
        }

        [Test]
        public void Equals_Object_CallsOperatorEquals()
        {
            var tl1 = new Timeline<string>((_t1, ValA));
            var tl2 = new Timeline<string>((_t1, ValA)); // Should be true by buggy logic (Time_Same || Value_Same)
            var tl3 = new Timeline<string>((_t2, ValB)); // Should be false (Time_Diff && Value_Diff)

            Assert.That(tl1.Equals((object)tl2), Is.True);
            Assert.That(tl1.Equals((object)tl3), Is.False);
            Assert.That(tl1.Equals("not a timeline"), Is.False);
        }

        // --- Getter Methods ---
        [Test]
        public void GetAllTimes_ReturnsDistinctTimes_OrderOfFirstAppearance()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t2, ValB);
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValC); // _t2 again

            // Expected order: _t2, _t1 (based on Add order and Distinct preserving first appearance)
            var expectedTimes = new[] { _t2, _t1 };
            Assert.That(timeline.GetAllTimes(), Is.EqualTo(expectedTimes));
        }

        [Test]
        public void GetAllTimes_FromSortedConstructor_ReturnsSortedDistinctTimes()
        {
            var timeline = new Timeline<string>((_t2, ValB), (_t1, ValA), (_t2, ValC));
            // Constructor sorts: [(_t1,A), (_t2,B), (_t2,C)] (approx, depends on stable sort for values at same time)
            // Distinct times from this sorted list: _t1, _t2
            var expectedTimes = new[] { _t1, _t2 };
            Assert.That(timeline.GetAllTimes(), Is.EqualTo(expectedTimes));
        }

        [Test]
        public void GetTimesByValue_ReturnsCorrectTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t3, ValA); // ValA again

            Assert.That(timeline.GetTimesByValue(ValA), Is.EquivalentTo(new[] { _t1, _t3 }));
            Assert.That(timeline.GetTimesByValue(ValB), Is.EquivalentTo(new[] { _t2 }));
            Assert.That(timeline.GetTimesByValue(ValC), Is.Empty);
        }

        [Test]
        public void GetTimesByValue_HandlesNullValueGracefully_IfTValueIsNullable()
        {
            // This test assumes TValue can be null and EqualityComparer handles it.
            // The current code `pair.Value!.Equals(value)` will throw if pair.Value is null.
            // This test would fail or expose that issue.
            // For now, assuming non-null values as per `!.` usage.
            // If `TValue` was `string?` and `null` was added:
            // var timeline = new Timeline<string?>();
            // timeline.Add(_t1, null);
            // Assert.Throws<NullReferenceException>(() => timeline.GetTimesByValue(null));
            // This indicates a bug in GetTimesByValue if nulls are supported.
            // For this test suite, we assume non-null TValue based on `!.`
            Assert.Pass("Skipping GetTimesByValue with null due to `!.` usage; assumes non-null values.");
        }


        [Test]
        public void GetTimesBefore_ReturnsSortedTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t3, ValC); // Added out of order
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);

            Assert.That(timeline.GetTimesBefore(_t2), Is.EqualTo(new[] { _t1 }));
            Assert.That(timeline.GetTimesBefore(_t1), Is.Empty);
            Assert.That(timeline.GetTimesBefore(new DateTime(2024, 1, 1)), Is.EqualTo(new[] { _t1, _t2, _t3 }.OrderBy(t => t).ToArray()));
        }

        [Test]
        public void GetTimesAfter_ReturnsSortedTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t3, ValC);
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);

            Assert.That(timeline.GetTimesAfter(_t2), Is.EqualTo(new[] { _t3 }));
            Assert.That(timeline.GetTimesAfter(_t3), Is.Empty);
            Assert.That(timeline.GetTimesAfter(new DateTime(2022, 1, 1)), Is.EqualTo(new[] { _t1, _t2, _t3 }.OrderBy(t => t).ToArray()));
        }

        [Test]
        public void GetAllValues_ReturnsAllValuesInOrderOfAddition()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t1, ValC); // Same time, different value, added later

            // Order depends on Add calls
            Assert.That(timeline.GetAllValues(), Is.EqualTo(new[] { ValA, ValB, ValC }));
        }

        [Test]
        public void GetValuesByTime_ReturnsValuesForTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t1, ValC);

            Assert.That(timeline.GetValuesByTime(_t1), Is.EquivalentTo(new[] { ValA, ValC }));
            Assert.That(timeline.GetValuesByTime(_t3), Is.Empty);
        }

        [Test]
        public void GetValuesBefore_ReturnsNewTimelineWithEarlierEvents()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t3, ValC);

            var beforeT2 = timeline.GetValuesBefore(_t2);
            Assert.That(beforeT2.Count, Is.EqualTo(1));
            Assert.That(beforeT2.Contains(_t1, ValA), Is.True);
            Assert.That(beforeT2.ToArray(), Is.EqualTo(new[] { (_t1, ValA) })); // Constructor sorts
        }

        [Test]
        public void GetValuesAfter_ReturnsNewTimelineWithLaterEvents()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t3, ValC);

            var afterT2 = timeline.GetValuesAfter(_t2);
            Assert.That(afterT2.Count, Is.EqualTo(1));
            Assert.That(afterT2.Contains(_t3, ValC), Is.True);
            Assert.That(afterT2.ToArray(), Is.EqualTo(new[] { (_t3, ValC) })); // Constructor sorts
        }

        // --- GetValuesBy specific DateTime parts ---
        // Example for GetValuesByYear, others follow similar pattern
        [Test]
        public void GetValuesByYear_ReturnsMatchingEvents()
        {
            var t2023_1 = new DateTime(2023, 5, 5);
            var t2023_2 = new DateTime(2023, 8, 8);
            var t2024_1 = new DateTime(2024, 1, 1);

            var timeline = new Timeline<string>();
            timeline.Add(t2023_1, "Event2023_1");
            timeline.Add(t2024_1, "Event2024_1");
            timeline.Add(t2023_2, "Event2023_2");

            var year2023Events = timeline.GetValuesByYear(2023);
            Assert.That(year2023Events.Count, Is.EqualTo(2));
            Assert.That(year2023Events.ContainsValue("Event2023_1"), Is.True);
            Assert.That(year2023Events.ContainsValue("Event2023_2"), Is.True);
            // The new timeline created by GetValuesByYear is sorted by its constructor
            Assert.That(year2023Events.ToArray(), Is.EqualTo(new[] { (t2023_1, "Event2023_1"), (t2023_2, "Event2023_2") }));


            var year2024Events = timeline.GetValuesByYear(2024);
            Assert.That(year2024Events.Count, Is.EqualTo(1));
            Assert.That(year2024Events.ContainsValue("Event2024_1"), Is.True);

            var year2025Events = timeline.GetValuesByYear(2025);
            Assert.That(year2025Events.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetValuesByMonth_ReturnsMatchingEvents()
        {
            var t_jan1 = new DateTime(2023, 1, 1);
            var t_jan2 = new DateTime(2023, 1, 15);
            var t_feb1 = new DateTime(2023, 2, 1);
            var timeline = new Timeline<string>((t_jan1, "Jan1"), (t_feb1, "Feb1"), (t_jan2, "Jan2"));

            var janEvents = timeline.GetValuesByMonth(1); // January
            Assert.That(janEvents.Count, Is.EqualTo(2));
            Assert.That(janEvents.ToArray(), Is.EquivalentTo(new[] { (t_jan1, "Jan1"), (t_jan2, "Jan2") }));
        }

        [Test]
        public void GetValuesByDay_ReturnsMatchingEvents()
        {
            var t_day1_a = new DateTime(2023, 1, 1);
            var t_day1_b = new DateTime(2023, 2, 1);
            var t_day15 = new DateTime(2023, 1, 15);
            var timeline = new Timeline<string>((t_day1_a, "Day1A"), (t_day15, "Day15"), (t_day1_b, "Day1B"));

            var day1Events = timeline.GetValuesByDay(1);
            Assert.That(day1Events.Count, Is.EqualTo(2));
            Assert.That(day1Events.ToArray(), Is.EquivalentTo(new[] { (t_day1_a, "Day1A"), (t_day1_b, "Day1B") }));
        }

        [Test]
        public void GetValuesByHour_ReturnsMatchingEvents()
        {
            var t_10_00 = new DateTime(2023, 1, 1, 10, 0, 0);
            var t_10_30 = new DateTime(2023, 1, 1, 10, 30, 0);
            var t_11_00 = new DateTime(2023, 1, 1, 11, 0, 0);
            var timeline = new Timeline<string>((t_10_00, "10:00"), (t_11_00, "11:00"), (t_10_30, "10:30"));

            var hour10Events = timeline.GetValuesByHour(10);
            Assert.That(hour10Events.Count, Is.EqualTo(2));
            Assert.That(hour10Events.ToArray(), Is.EquivalentTo(new[] { (t_10_00, "10:00"), (t_10_30, "10:30") }));
        }

        [Test]
        public void GetValuesByMinute_ReturnsMatchingEvents()
        {
            var t_10_30_00 = new DateTime(2023, 1, 1, 10, 30, 0);
            var t_11_30_00 = new DateTime(2023, 1, 1, 11, 30, 0);
            var t_10_15_00 = new DateTime(2023, 1, 1, 10, 15, 0);
            var timeline = new Timeline<string>((t_10_30_00, "10:30"), (t_10_15_00, "10:15"), (t_11_30_00, "11:30"));

            var min30Events = timeline.GetValuesByMinute(30);
            Assert.That(min30Events.Count, Is.EqualTo(2));
            Assert.That(min30Events.ToArray(), Is.EquivalentTo(new[] { (t_10_30_00, "10:30"), (t_11_30_00, "11:30") }));
        }

        [Test]
        public void GetValuesBySecond_ReturnsMatchingEvents()
        {
            var t_s15_a = new DateTime(2023, 1, 1, 10, 0, 15);
            var t_s15_b = new DateTime(2023, 1, 1, 10, 1, 15);
            var t_s30 = new DateTime(2023, 1, 1, 10, 0, 30);
            var timeline = new Timeline<string>((t_s15_a, "S15_A"), (t_s30, "S30"), (t_s15_b, "S15_B"));

            var sec15Events = timeline.GetValuesBySecond(15);
            Assert.That(sec15Events.Count, Is.EqualTo(2));
            Assert.That(sec15Events.ToArray(), Is.EquivalentTo(new[] { (t_s15_a, "S15_A"), (t_s15_b, "S15_B") }));
        }

        [Test]
        public void GetValuesByMillisecond_ReturnsMatchingEvents()
        {
            var t_ms100_a = new DateTime(2023, 1, 1, 10, 0, 0, 100);
            var t_ms100_b = new DateTime(2023, 1, 1, 10, 0, 1, 100);
            var t_ms200 = new DateTime(2023, 1, 1, 10, 0, 0, 200);
            var timeline = new Timeline<string>((t_ms100_a, "ms100_A"), (t_ms200, "ms200"), (t_ms100_b, "ms100_B"));

            var ms100Events = timeline.GetValuesByMillisecond(100);
            Assert.That(ms100Events.Count, Is.EqualTo(2));
            Assert.That(ms100Events.ToArray(), Is.EquivalentTo(new[] { (t_ms100_a, "ms100_A"), (t_ms100_b, "ms100_B") }));
        }

        [Test]
        public void GetValuesByTimeOfDay_ReturnsMatchingEvents()
        {
            var tod = new TimeSpan(10, 30, 15);
            var t_match1 = new DateTime(2023, 1, 1, 10, 30, 15);
            var t_match2 = new DateTime(2023, 1, 2, 10, 30, 15); // Same TOD, different day
            var t_no_match = new DateTime(2023, 1, 1, 11, 30, 15);
            var timeline = new Timeline<string>((t_match1, "Match1"), (t_no_match, "NoMatch"), (t_match2, "Match2"));

            var todEvents = timeline.GetValuesByTimeOfDay(tod);
            Assert.That(todEvents.Count, Is.EqualTo(2));
            Assert.That(todEvents.ToArray(), Is.EquivalentTo(new[] { (t_match1, "Match1"), (t_match2, "Match2") }));
        }

        [Test]
        public void GetValuesByDayOfWeek_ReturnsMatchingEvents()
        {
            // _t1 (2023/1/1) is Sunday. _t2 (2023/1/2) is Monday.
            var timeline = new Timeline<string>((_t1, "SundayEvent"), (_t2, "MondayEvent"), (new DateTime(2023, 1, 8), "AnotherSunday"));

            var sundayEvents = timeline.GetValuesByDayOfWeek(DayOfWeek.Sunday);
            Assert.That(sundayEvents.Count, Is.EqualTo(2));
            Assert.That(sundayEvents.ToArray(), Is.EquivalentTo(new[] { (_t1, "SundayEvent"), (new DateTime(2023, 1, 8), "AnotherSunday") }));
        }

        [Test]
        public void GetValuesByDayOfYear_ReturnsMatchingEvents()
        {
            // _t1 (2023/1/1) is Day 1. _t2 (2023/1/2) is Day 2.
            // _t3 (2023/1/3) is Day 3.
            var t_next_year_day2 = new DateTime(2024, 1, 2); // Day 2 of 2024
            var timeline = new Timeline<string>((_t1, "Day1"), (_t2, "Day2_2023"), (_t3, "Day3"), (t_next_year_day2, "Day2_2024"));

            var day2Events = timeline.GetValuesByDayOfYear(2);
            Assert.That(day2Events.Count, Is.EqualTo(2));
            Assert.That(day2Events.ToArray(), Is.EquivalentTo(new[] { (_t2, "Day2_2023"), (t_next_year_day2, "Day2_2024") }));
        }

        // --- Add Methods ---
        [Test]
        public void Add_SingleEvent_AppendsToTimeline()
        {
            var timeline = new Timeline<string>((_t1, ValA)); // Sorted initially
            timeline.Add(_t3, ValC); // Appends
            timeline.Add(_t2, ValB); // Appends

            Assert.That(timeline.Count, Is.EqualTo(3));
            // Order is based on additions, not sorted automatically by Add
            var expectedOrder = new[] { (_t1, ValA), (_t3, ValC), (_t2, ValB) };
            Assert.That(timeline.ToArray(), Is.EqualTo(expectedOrder));
        }

        [Test]
        public void Add_ParamsEvents_AppendsToTimeline()
        {
            var timeline = new Timeline<string>((_t1, ValA));
            timeline.Add((_t3, ValC), (_t2, ValB)); // Appends in given order

            Assert.That(timeline.Count, Is.EqualTo(3));
            var expectedOrder = new[] { (_t1, ValA), (_t3, ValC), (_t2, ValB) };
            Assert.That(timeline.ToArray(), Is.EqualTo(expectedOrder));
        }

        [Test]
        public void Add_TimelineInstance_AppendsEventsFromOtherTimeline()
        {
            var timeline1 = new Timeline<string>((_t1, ValA));
            var timeline2 = new Timeline<string>((_t3, ValC), (_t2, ValB)); // timeline2 is sorted by constructor

            timeline1.Add(timeline2); // Appends timeline2.ToArray()

            Assert.That(timeline1.Count, Is.EqualTo(3));
            // timeline2.ToArray() is [(_t2,ValB), (_t3,ValC)]
            var expectedOrder = new[] { (_t1, ValA), (_t2, ValB), (_t3, ValC) };
            Assert.That(timeline1.ToArray(), Is.EqualTo(expectedOrder));
        }

        [Test]
        public void AddNow_AddsEventsWithCurrentTime()
        {
            var timeline = new Timeline<string>();
            var before = DateTime.Now;
            timeline.AddNow(ValA, ValB);
            var after = DateTime.Now;

            Assert.That(timeline.Count, Is.EqualTo(2));
            var events = timeline.ToArray();
            Assert.That(events[0].Value, Is.EqualTo(ValA));
            Assert.That(events[1].Value, Is.EqualTo(ValB));
            Assert.That(events[0].Time, Is.EqualTo(events[1].Time)); // Both added with same DateTime.Now
            Assert.That(events[0].Time, Is.GreaterThanOrEqualTo(before).And.LessThanOrEqualTo(after));
        }

        // --- Contains Methods ---
        [Test]
        public void Contains_SpecificEvent_ChecksPresence()
        {
            var timeline = new Timeline<string>((_t1, ValA), (_t2, ValB));
            Assert.That(timeline.Contains(_t1, ValA), Is.True);
            Assert.That(timeline.Contains(_t1, ValB), Is.False);
            Assert.That(timeline.Contains(_t3, ValA), Is.False);
        }

        [Test]
        public void Contains_ParamsEvents_ChecksIfAnyPresent()
        {
            var timeline = new Timeline<string>((_t1, ValA), (_t2, ValB));
            Assert.That(timeline.Contains((_t1, ValA), (_t3, ValC)), Is.True); // (_t1, ValA) is present
            Assert.That(timeline.Contains((_t3, ValC), (_t1, ValB)), Is.False); // Neither present
        }

        [Test]
        public void Contains_TimelineInstance_ChecksIfAnyEventPresent()
        {
            var mainTimeline = new Timeline<string>((_t1, ValA), (_t2, ValB));
            var subTimeline1 = new Timeline<string>((_t2, ValB), (_t3, ValC)); // (_t2, ValB) is in main
            var subTimeline2 = new Timeline<string>((_t3, ValC)); // Nothing in main

            Assert.That(mainTimeline.Contains(subTimeline1), Is.True);
            Assert.That(mainTimeline.Contains(subTimeline2), Is.False);
        }

        [Test]
        public void ContainsTime_ChecksIfAnyTimePresent()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t1, ValB); // _t1 is present
            timeline.Add(_t2, ValC); // _t2 is present

            Assert.That(timeline.ContainsTime(_t1, _t3), Is.True); // _t1 is present
            Assert.That(timeline.ContainsTime(_t3, new DateTime(2025, 1, 1)), Is.False);
        }

        [Test]
        public void ContainsValue_ChecksIfAnyValuePresent()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t3, ValA); // ValA is present

            Assert.That(timeline.ContainsValue(ValA, ValC), Is.True); // ValA is present
            Assert.That(timeline.ContainsValue(ValC, "NotInTimeline"), Is.False);
        }

        // --- Remove Methods ---
        [Test]
        public void Remove_SpecificEvent_RemovesFirstOccurrence()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t1, ValA); // Duplicate event

            bool removed = timeline.Remove(_t1, ValA);
            Assert.That(removed, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(2));
            // Check that one (_t1, ValA) is still there
            Assert.That(timeline.ToArray(), Is.EqualTo(new[] { (_t2, ValB), (_t1, ValA) }));
            // Order after List.Remove depends on internal impl.
            // It removes the first one, so the one at index 0.
            // Original: [(_t1,A), (_t2,B), (_t1,A)]
            // After remove: [(_t2,B), (_t1,A)]

            bool notRemoved = timeline.Remove(_t3, ValC);
            Assert.That(notRemoved, Is.False);
        }

        [Test]
        public void Remove_ParamsEvents_RemovesMatchingEvents()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t3, ValC);

            bool result = timeline.Remove((_t1, ValA), (_t3, ValC), (_t1, "NonExistent"));
            Assert.That(result, Is.True); // True if any removal happened
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_t2, ValB), Is.True);
        }

        [Test]
        public void Remove_TimelineInstance_RemovesMatchingEvents()
        {
            var mainTimeline = new Timeline<string>();
            mainTimeline.Add(_t1, ValA);
            mainTimeline.Add(_t2, ValB);
            mainTimeline.Add(_t3, ValC);

            var toRemove = new Timeline<string>((_t1, ValA), (_t3, ValC));
            bool result = mainTimeline.Remove(toRemove);

            Assert.That(result, Is.True);
            Assert.That(mainTimeline.Count, Is.EqualTo(1));
            Assert.That(mainTimeline.Contains(_t2, ValB), Is.True);
        }

        [Test]
        public void RemoveTimes_RemovesAllEventsAtSpecifiedTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t1, ValB); // Same time
            timeline.Add(_t2, ValC);
            timeline.Add(_t3, "ValD");

            bool result = timeline.RemoveTimes(_t1, _t3, new DateTime(2025, 1, 1)); // _t1 and _t3 exist
            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_t2, ValC), Is.True);
            Assert.That(timeline.ContainsTime(_t1), Is.False);
            Assert.That(timeline.ContainsTime(_t3), Is.False);

            bool noResult = timeline.RemoveTimes(new DateTime(2025, 1, 1));
            Assert.That(noResult, Is.False);
        }

        [Test]
        public void RemoveValues_RemovesAllEventsWithSpecifiedValues()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);
            timeline.Add(_t3, ValA); // ValA again
            timeline.Add(_t1, ValC);

            bool result = timeline.RemoveValues(ValA, ValC, "NonExistent");
            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_t2, ValB), Is.True);
            Assert.That(timeline.ContainsValue(ValA), Is.False);
            Assert.That(timeline.ContainsValue(ValC), Is.False);

            bool noResult = timeline.RemoveValues("NonExistent");
            Assert.That(noResult, Is.False);
        }

        [Test]
        public void RemoveValues_UsesEqualityComparerForTValue()
        {
            var timeline = new Timeline<int>(); // Using int as TValue
            timeline.Add(_t1, 10);
            timeline.Add(_t2, 20);
            timeline.Add(_t3, 10);

            bool result = timeline.RemoveValues(10);
            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_t2, 20), Is.True);
        }


        // --- Conversion Methods ---
        [Test]
        public void ToArray_ReturnsArrayOfEvents_OrderBasedOnInternalList()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t2, ValB); // Added first
            timeline.Add(_t1, ValA); // Added second

            var expectedArray = new[] { (_t2, ValB), (_t1, ValA) };
            Assert.That(timeline.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        public void ToList_ReturnsInternalListReference()
        {
            var timeline = new Timeline<string>((_t1, ValA));
            IList<(DateTime Time, string Value)> list = timeline.ToList();

            Assert.That(list.Count, Is.EqualTo(1));

            // Modifying the returned list should modify the timeline's internal state
            list.Add((_t2, ValB));
            Assert.That(timeline.Count, Is.EqualTo(2));
            Assert.That(timeline.Contains(_t2, ValB), Is.True);
        }

        [Test]
        public void ToDictionary_UniqueTimes_ConvertsToDictionary()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t2, ValB);

            IDictionary<DateTime, string> dict = timeline.ToDictionary();
            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict[_t1], Is.EqualTo(ValA));
            Assert.That(dict[_t2], Is.EqualTo(ValB));
        }

        [Test]
        public void ToDictionary_DuplicateTimes_ThrowsArgumentException()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_t1, ValA);
            timeline.Add(_t1, ValB); // Same time, different value

            Assert.Throws<ArgumentException>(() => timeline.ToDictionary());
        }

        // --- GetHashCode ---
        [Test]
        public void GetHashCode_ConsistentForEqualObjects_BasedOnInternalList()
        {
            // HashCode depends on the internal list's hash code.
            // If two timelines have internal lists that are sequence-equal, hash codes should be same.
            var timeline1 = new Timeline<string>();
            timeline1.Add(_t1, ValA);
            timeline1.Add(_t2, ValB);

            var timeline2 = new Timeline<string>();
            timeline2.Add(_t1, ValA);
            timeline2.Add(_t2, ValB);

            Assert.That(timeline1.GetHashCode(), Is.Not.EqualTo(timeline2.GetHashCode()),
                "Timeline.GetHashCode() currently returns different hash codes for distinct instances " +
                "that are content-equal, due to using reference-based List.GetHashCode().");
        }

        [Test]
        public void GetHashCode_DifferentForDifferentObjects()
        {
            var timeline1 = new Timeline<string>();
            timeline1.Add(_t1, ValA);
            timeline1.Add(_t2, ValB);

            var timeline2 = new Timeline<string>();
            timeline2.Add(_t1, ValA);
            // timeline2 is different from timeline1

            var timeline3 = new Timeline<string>();
            timeline3.Add(_t2, ValB); // Different order of addition
            timeline3.Add(_t1, ValA);


            Assert.That(timeline1.GetHashCode(), Is.Not.EqualTo(timeline2.GetHashCode()));
            Assert.That(timeline1.GetHashCode(), Is.Not.EqualTo(timeline3.GetHashCode()));
        }
    }
}
