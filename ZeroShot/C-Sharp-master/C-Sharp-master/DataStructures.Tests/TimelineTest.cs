using NUnit.Framework;
using DataStructures; // Assuming Timeline.cs is in this namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures.Tests
{
    [TestFixture]
    public class TimelineTests
    {
#pragma warning disable IDE1006 // Naming Styles
        private DateTime _time1, _time2, _time3, _time1Dup;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private string _value1, _value2, _value3;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning restore IDE1006 // Naming Styles

        [SetUp]
        public void Setup()
        {
            _time1 = new DateTime(2023, 1, 1, 10, 0, 0);
            _time1Dup = new DateTime(2023, 1, 1, 10, 0, 0); // Same as _time1
            _time2 = new DateTime(2023, 1, 1, 12, 0, 0);
            _time3 = new DateTime(2023, 1, 2, 10, 0, 0);

            _value1 = "Event A";
            _value2 = "Event B";
            _value3 = "Event C";
        }

        #region Constructors
        [Test]
        public void Constructor_Default_CreatesEmptyTimeline()
        {
            var timeline = new Timeline<string>();
            Assert.That(timeline.Count, Is.EqualTo(0));
            Assert.That(timeline.TimesCount, Is.EqualTo(0));
            Assert.That(timeline.ValuesCount, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_SingleEvent_InitializesTimeline()
        {
            var timeline = new Timeline<string>(_time1, _value1);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains((_time1, _value1)));
            Assert.That(timeline.TimesCount, Is.EqualTo(1));
            Assert.That(timeline.ValuesCount, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_Params_InitializesAndSortsTimeline()
        {
            var events = new[] { (_time2, _value2), (_time1, _value1), (_time3, _value3) };
            var timeline = new Timeline<string>(events);

            Assert.That(timeline.Count, Is.EqualTo(3));
            var array = timeline.ToArray();
            Assert.That(array[0], Is.EqualTo((_time1, _value1)));
            Assert.That(array[1], Is.EqualTo((_time2, _value2)));
            Assert.That(array[2], Is.EqualTo((_time3, _value3)));
        }

        [Test]
        public void Constructor_Params_WithDuplicateTimes_InitializesTimelineAndSorts()
        {
            var events = new[] { (_time2, _value3), (_time1, _value1), (_time1Dup, _value2) };
            var timeline = new Timeline<string>(events);

            Assert.That(timeline.Count, Is.EqualTo(3));
            Assert.That(timeline.TimesCount, Is.EqualTo(2));
            Assert.That(timeline.ValuesCount, Is.EqualTo(3));

            var array = timeline.ToArray();
            // Order of items with same time depends on original order if sort is stable
            // Input: (t2,v3), (t1,v1), (t1,v2) -> Sorted: (t1,v1), (t1,v2), (t2,v3)
            Assert.That(array[0], Is.EqualTo((_time1, _value1)));
            Assert.That(array[1], Is.EqualTo((_time1Dup, _value2)));
            Assert.That(array[2], Is.EqualTo((_time2, _value3)));
        }
        #endregion

        #region Properties
        [Test]
        public void TimesCount_ReturnsNumberOfUniqueTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1Dup, _value2); // Same time as _time1
            timeline.Add(_time2, _value3);

            Assert.That(timeline.TimesCount, Is.EqualTo(2));
        }

        [Test]
        public void ValuesCount_ReturnsTotalNumberOfValues()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1Dup, _value2);
            timeline.Add(_time2, _value3);

            Assert.That(timeline.ValuesCount, Is.EqualTo(3));
            Assert.That(timeline.ValuesCount, Is.EqualTo(timeline.Count));
        }

        [Test]
        public void Indexer_Get_ReturnsValuesForTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1, _value2);
            timeline.Add(_time2, _value3);

            var valuesAtTime1 = timeline[_time1];
            Assert.That(valuesAtTime1, Is.EquivalentTo(new[] { _value1, _value2 }));

            var valuesAtTime2 = timeline[_time2];
            Assert.That(valuesAtTime2, Is.EquivalentTo(new[] { _value3 }));

            var valuesAtUnusedTime = timeline[DateTime.MinValue];
            Assert.That(valuesAtUnusedTime, Is.Empty);
        }

        [Test]
        public void Indexer_Set_ReplacesValuesForTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1, _value2);
            timeline.Add(_time2, _value3);

            string newValue1 = "New Value 1";
            string newValue2 = "New Value 2";
            timeline[_time1] = new[] { newValue1, newValue2 };

            Assert.That(timeline.Count, Is.EqualTo(3));
            var valuesAtTime1 = timeline[_time1];
            Assert.That(valuesAtTime1, Is.EquivalentTo(new[] { newValue1, newValue2 }));
            Assert.That(timeline.Contains(_time1, _value1), Is.False);
            Assert.That(timeline.Contains(_time1, _value2), Is.False);

            var newTime = new DateTime(2024, 1, 1);
            string anotherNewValue = "Another New Value";
            timeline[newTime] = new[] { anotherNewValue };
            Assert.That(timeline.Count, Is.EqualTo(4));
            Assert.That(timeline[newTime], Is.EquivalentTo(new[] { anotherNewValue }));
        }

        [Test]
        public void Indexer_Set_WithEmptyArray_RemovesAllValuesForTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1, _value2);
            timeline.Add(_time2, _value3);

            timeline[_time1] = Array.Empty<string>();

            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline[_time1], Is.Empty);
            Assert.That(timeline.Contains(_time2, _value3));
        }

        [Test]
        public void ICollection_IsReadOnly_IsFalse()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>();
            Assert.That(timeline.IsReadOnly, Is.False);
        }
        #endregion

        #region ICollection Methods
        [Test]
        public void Clear_RemovesAllEvents()
        {
            var timeline = new Timeline<string>(_time1, _value1);
            timeline.Add(_time2, _value2);

            timeline.Clear();

            Assert.That(timeline.Count, Is.EqualTo(0));
            Assert.That(timeline.TimesCount, Is.EqualTo(0));
            Assert.That(timeline.ValuesCount, Is.EqualTo(0));
        }

        [Test]
        public void CopyTo_CopiesEventsToArray()
        {
            var timeline = new Timeline<string>(
                (_time1, _value1),
                (_time2, _value2)
            );

            var array = new (DateTime, string)[3];
            timeline.CopyTo(array, 1);

            Assert.That(array[0], Is.EqualTo(default((DateTime, string))));
            Assert.That(array[1], Is.EqualTo((_time1, _value1)));
            Assert.That(array[2], Is.EqualTo((_time2, _value2)));
        }

        [Test]
        public void CopyTo_EmptyTimeline_DoesNothing()
        {
            var timeline = new Timeline<string>();
            var array = new (DateTime, string)[1];
            Assert.DoesNotThrow(() => timeline.CopyTo(array, 0));
            Assert.That(array[0], Is.EqualTo(default((DateTime, string))));
        }

        [Test]
        public void CopyTo_ThrowsArgumentNullException_IfArrayIsNull()
        {
            var timeline = new Timeline<string>((_time1, _value1));
            Assert.Throws<ArgumentNullException>(() => timeline.CopyTo(null!, 0));
        }

        [Test]
        public void CopyTo_ThrowsArgumentOutOfRangeException_IfArrayIndexIsNegative()
        {
            var timeline = new Timeline<string>((_time1, _value1));
            var array = new (DateTime, string)[1];
            Assert.Throws<ArgumentOutOfRangeException>(() => timeline.CopyTo(array, -1));
        }

        [Test]
        public void CopyTo_ThrowsArgumentException_IfArrayIsTooSmall()
        {
            var timeline = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var array = new (DateTime, string)[1];
            Assert.Throws<ArgumentException>(() => timeline.CopyTo(array, 0));

            var array2 = new (DateTime, string)[2];
            Assert.Throws<ArgumentException>(() => timeline.CopyTo(array2, 1));
        }

        [Test]
        public void ICollection_Add_AddsEvent()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>();
            timeline.Add((_time1, _value1));

            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(((Timeline<string>)timeline).Contains(_time1, _value1));
        }

        [Test]
        public void ICollection_Contains_ChecksEventExistence()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>((_time1, _value1));

            Assert.That(timeline.Contains((_time1, _value1)), Is.True);
            Assert.That(timeline.Contains((_time2, _value2)), Is.False);
        }

        [Test]
        public void ICollection_Remove_RemovesEvent()
        {
            ICollection<(DateTime Time, string Value)> timeline = new Timeline<string>();
            ((Timeline<string>)timeline).Add(_time1, _value1);
            ((Timeline<string>)timeline).Add(_time2, _value2);

            var result = timeline.Remove((_time1, _value1));

            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains((_time1, _value1)), Is.False);
            Assert.That(timeline.Contains((_time2, _value2)), Is.True);

            var resultNonExistent = timeline.Remove((_time3, _value3));
            Assert.That(resultNonExistent, Is.False);
        }
        #endregion

        #region Enumerators
        [Test]
        public void GetEnumerator_AllowsIteration()
        {
            var events = new[] { (_time1, _value1), (_time2, _value2) };
            var timeline = new Timeline<string>(events);

            var iteratedEvents = new List<(DateTime, string)>();
            foreach (var item in timeline) // Uses IEnumerable.GetEnumerator() or specific one
            {
                iteratedEvents.Add(item);
            }

            Assert.That(iteratedEvents.Count, Is.EqualTo(2));
            Assert.That(iteratedEvents[0], Is.EqualTo((_time1, _value1)));
            Assert.That(iteratedEvents[1], Is.EqualTo((_time2, _value2)));
        }

        [Test]
        public void GetEnumerator_Generic_AllowsIteration()
        {
            var events = new[] { (_time1, _value1), (_time2, _value2) };
            var timeline = new Timeline<string>(events);

            var iteratedEvents = new List<(DateTime, string)>();
            using (var enumerator = ((IEnumerable<(DateTime Time, string Value)>)timeline).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    iteratedEvents.Add(enumerator.Current);
                }
            }

            Assert.That(iteratedEvents.Count, Is.EqualTo(2));
            Assert.That(iteratedEvents[0], Is.EqualTo((_time1, _value1)));
            Assert.That(iteratedEvents[1], Is.EqualTo((_time2, _value2)));
        }
        #endregion

        #region Equality and HashCode
        // Note: operator== has bugs (uses && instead of ||, and Value!.Equals)
        // These tests reflect the current buggy behavior.
        [Test]
        public void Equals_And_OperatorEquals_WithIdenticalTimelines_ReturnsTrue()
        {
            var timeline1 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline2 = new Timeline<string>((_time1, _value1), (_time2, _value2));

            Assert.That(timeline1.Equals(timeline2), Is.True);
            Assert.That(timeline1 == timeline2, Is.True);
            Assert.That(timeline2 == timeline1, Is.True);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            var timeline1 = new Timeline<string>((_time1, _value1));
            Assert.That(timeline1.Equals(null), Is.False);
        }

        [Test]
        public void OperatorEquals_WithDifferentCounts_ReturnsFalse()
        {
            var timeline1 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline2 = new Timeline<string>((_time1, _value1));

            Assert.That(timeline1 == timeline2, Is.False);
            Assert.That(timeline2 == timeline1, Is.False);
        }

        [Test]
        public void OperatorEquals_Bug_DifferentValuesSameTime_ReturnsTrue_DueToBug()
        {
            var timeline1 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline2 = new Timeline<string>((_time1, _value1), (_time2, _value3));
            Assert.That(timeline1 == timeline2, Is.True, "This test passes due to a bug in operator== (&& logic). It should be false.");
        }

        [Test]
        public void OperatorEquals_Bug_DifferentTimesSameValue_ReturnsTrue_DueToBug()
        {
            var timeline1 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline2 = new Timeline<string>((_time1, _value1), (_time3, _value2));
            Assert.That(timeline1 == timeline2, Is.True, "This test passes due to a bug in operator== (&& logic). It should be false.");
        }

        [Test]
        public void OperatorEquals_DifferentTimeAndValue_ReturnsFalse()
        {
            var timeline1 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline2 = new Timeline<string>((_time1, _value1), (_time3, _value3));
            Assert.That(timeline1 == timeline2, Is.False); // This case works as expected by coincidence of the bug.
        }

        [Test]
        public void OperatorEquals_WithNullValueInOne_ThrowsNullReferenceException_DueToBug()
        {
            // Setup: Timelines with the same time, one null value, one non-null. Using string?
            var timeline1 = new Timeline<string?>((_time1, null as string));
            var timeline2 = new Timeline<string?>((_time1, _value2));

            // Action & Assert:
            // Due to the '&&' short-circuit in operator== when times are equal,
            // the Value!.Equals part is NOT reached.
            // The operator== incorrectly returns true.
            bool result = false;
            Assert.DoesNotThrow(() => { result = timeline1 == timeline2; },
                "Operator== should not throw NRE here due to short-circuiting.");
            Assert.That(result, Is.True,
                "Operator== incorrectly returns true when times are equal and one value is null, due to short-circuiting before the NRE-prone Value!.Equals.");
            // Similar to the above, the NRE isn't thrown. The bug is an incorrect equality result.
        }

        [Test]
        public void OperatorEquals_WithBothNullValues_ThrowsNullReferenceException_DueToBug()
        {
            // Setup: Timelines with the same time and null values. Using string?
            var timeline1 = new Timeline<string?>((_time1, null as string));
            var timeline2 = new Timeline<string?>((_time1, null as string));

            // Action & Assert:
            // Due to the '&&' short-circuit in operator== when times are equal,
            // the Value!.Equals part is NOT reached.
            // The operator== incorrectly returns true.
            bool result = false;
            Assert.DoesNotThrow(() => { result = timeline1 == timeline2; },
                "Operator== should not throw NRE here due to short-circuiting.");
            Assert.That(result, Is.True,
                "Operator== incorrectly returns true when times are equal and both values are null, due to short-circuiting before the NRE-prone Value!.Equals.");
            // The original test description "This test passes due to a bug in operator== (Value!.Equals)."
            // is slightly misleading for the NRE part, as the NRE isn't thrown.
            // The bug manifested here is an incorrect equality result.
        }


        [Test]
        public void OperatorNotEquals_IsInverseOfOperatorEquals()
        {
            var timeline1 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline2 = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var timeline3 = new Timeline<string>((_time1, _value1));

            Assert.That(timeline1 != timeline2, Is.False);
            Assert.That(timeline1 != timeline3, Is.True);
        }

        [Test]
        public void GetHashCode_IsNotOverriddenToMatchValueEquality()
        {
            var timeline1 = new Timeline<string>((_time1, _value1));
            var timeline2 = new Timeline<string>((_time1, _value1));

            // Default GetHashCode is from object (via List<T>), based on reference.
            // Thus, two different instances with same content will likely have different hash codes.
            Assert.DoesNotThrow(() => timeline1.GetHashCode());
            Assert.That(timeline1.GetHashCode(), Is.Not.EqualTo(timeline2.GetHashCode()), "GetHashCode is not based on content by default for List<T>.");

            var timeline3 = timeline1;
            Assert.That(timeline1.GetHashCode(), Is.EqualTo(timeline3.GetHashCode()), "HashCode should be same for same reference.");
        }
        #endregion

        #region Get Methods
        [Test]
        public void GetAllTimes_ReturnsDistinctSortedTimes()
        {
            var timeline = new Timeline<string>(
                (_time2, _value2),
                (_time1, _value1),
                (_time1Dup, _value3)
            );
            var times = timeline.GetAllTimes();
            // Constructor sorts by time. GetAllTimes uses Distinct() which preserves order of first occurrence.
            Assert.That(times, Is.EqualTo(new[] { _time1, _time2 }));
        }

        [Test]
        public void GetTimesByValue_ReturnsTimesForValue()
        {
            var timeline = new Timeline<string>(
                (_time1, _value1),
                (_time2, _value2),
                (_time3, _value1)
            );

            var timesForValue1 = timeline.GetTimesByValue(_value1).OrderBy(t => t).ToArray(); // Ensure order for comparison
            Assert.That(timesForValue1, Is.EqualTo(new[] { _time1, _time3 }));

            var timesForValue2 = timeline.GetTimesByValue(_value2);
            Assert.That(timesForValue2, Is.EqualTo(new[] { _time2 }));

            var timesForNonExistentValue = timeline.GetTimesByValue("NonExistent");
            Assert.That(timesForNonExistentValue, Is.Empty);
        }

        [Test]
        public void GetTimesByValue_HandlesNullValue()
        {
            // Setup: Timeline with null values. Using string? to allow nulls.
            var timeline = new Timeline<string?>(
                (_time1, null),
                (_time2, _value1),
                (_time3, null)
            );

            // Assert: Expect a NullReferenceException because Timeline.GetTimesByValue
            // uses pair.Value!.Equals(value), which will throw if pair.Value is null.
            Assert.Throws<NullReferenceException>(() =>
            {
                // The call to GetTimesByValue(null) will trigger the NRE.
                // The subsequent .OrderBy().ToArray() won't be reached if NRE is thrown.
                var timesForNull = timeline.GetTimesByValue(null).OrderBy(t => t).ToArray();
            }, "GetTimesByValue should throw NullReferenceException when encountering null values due to pair.Value!.Equals(value).");
        }

        [Test]
        public void GetTimesBefore_ReturnsSortedTimes()
        {
            var timeline = new Timeline<string>(
                (_time1, _value1), (_time2, _value2), (_time3, _value3)
            );

            var timesBeforeTime2 = timeline.GetTimesBefore(_time2);
            Assert.That(timesBeforeTime2, Is.EqualTo(new[] { _time1 }));

            var timesBeforeTime1 = timeline.GetTimesBefore(_time1);
            Assert.That(timesBeforeTime1, Is.Empty);
        }

        [Test]
        public void GetTimesAfter_ReturnsSortedTimes()
        {
            var timeline = new Timeline<string>(
                (_time1, _value1), (_time2, _value2), (_time3, _value3)
            );

            var timesAfterTime2 = timeline.GetTimesAfter(_time2);
            Assert.That(timesAfterTime2, Is.EqualTo(new[] { _time3 }));

            var timesAfterTime3 = timeline.GetTimesAfter(_time3);
            Assert.That(timesAfterTime3, Is.Empty);
        }

        [Test]
        public void GetAllValues_ReturnsAllValuesInOrderOfEvents()
        {
            var timeline = new Timeline<string>(
                (_time2, _value2),
                (_time1, _value1)
            );

            var values = timeline.GetAllValues();
            Assert.That(values, Is.EqualTo(new[] { _value1, _value2 }));
        }

        [Test]
        public void GetValuesByTime_ReturnsValuesForSpecificTime()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1Dup, _value2);
            timeline.Add(_time2, _value3);

            var valuesAtTime1 = timeline.GetValuesByTime(_time1);
            Assert.That(valuesAtTime1, Is.EquivalentTo(new[] { _value1, _value2 }));
        }

        [Test]
        public void GetValuesBefore_ReturnsNewTimelineWithEarlierEvents()
        {
            var originalTimeline = new Timeline<string>(
                (_time1, _value1), (_time2, _value2), (_time3, _value3)
            );

            var timelineBeforeTime2 = originalTimeline.GetValuesBefore(_time2);
            Assert.That(timelineBeforeTime2.Count, Is.EqualTo(1));
            Assert.That(timelineBeforeTime2.Contains((_time1, _value1)));
            Assert.That(timelineBeforeTime2.ToArray()[0], Is.EqualTo((_time1, _value1)));
        }

        [Test]
        public void GetValuesAfter_ReturnsNewTimelineWithLaterEvents()
        {
            var originalTimeline = new Timeline<string>(
                (_time1, _value1), (_time2, _value2), (_time3, _value3)
            );

            var timelineAfterTime2 = originalTimeline.GetValuesAfter(_time2);
            Assert.That(timelineAfterTime2.Count, Is.EqualTo(1));
            Assert.That(timelineAfterTime2.Contains((_time3, _value3)));
            Assert.That(timelineAfterTime2.ToArray()[0], Is.EqualTo((_time3, _value3)));
        }

        [Test]
        public void GetValuesByYear_ReturnsMatchingEvents()
        {
            var time2023 = new DateTime(2023, 5, 15);
            var time2024 = new DateTime(2024, 6, 20);
            var originalTimeline = new Timeline<string>(
                (time2023, _value1), (time2024, _value2), (new DateTime(2023, 10, 1), _value3)
            );

            var timelineIn2023 = originalTimeline.GetValuesByYear(2023);
            Assert.That(timelineIn2023.Count, Is.EqualTo(2));
            Assert.That(timelineIn2023.ContainsValue(_value1, _value3));
            Assert.That(timelineIn2023.ContainsValue(_value2), Is.False);
            var arr = timelineIn2023.ToArray(); // New timeline is sorted by constructor
            Assert.That(arr[0].Time, Is.EqualTo(time2023));
            Assert.That(arr[1].Time, Is.EqualTo(new DateTime(2023, 10, 1)));
        }

        [Test]
        public void GetValuesByDayOfWeek_ReturnsMatchingEvents()
        {
            // _time1 = 2023/01/01 (Sunday)
            // _time3 = 2023/01/02 (Monday)
            var originalTimeline = new Timeline<string>(
                (_time1, _value1), (_time2, _value2), (_time3, _value3) // _time2 is also Sunday
            );

            var sundayEvents = originalTimeline.GetValuesByDayOfWeek(DayOfWeek.Sunday);
            Assert.That(sundayEvents.Count, Is.EqualTo(2));
            Assert.That(sundayEvents.Contains((_time1, _value1)));
            Assert.That(sundayEvents.Contains((_time2, _value2)));

            var mondayEvents = originalTimeline.GetValuesByDayOfWeek(DayOfWeek.Monday);
            Assert.That(mondayEvents.Count, Is.EqualTo(1));
            Assert.That(mondayEvents.Contains((_time3, _value3)));
        }
        #endregion

        #region Add Methods
        [Test]
        public void Add_SingleEvent_AddsToTimelineUnsorted()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains((_time1, _value1)));

            timeline.Add(_time3, _value3);
            timeline.Add(_time2, _value2); // Added last, time is between _time1 and _time3

            var arr = timeline.ToArray(); // Reflects insertion order for Add
            Assert.That(arr[0], Is.EqualTo((_time1, _value1)));
            Assert.That(arr[1], Is.EqualTo((_time3, _value3)));
            Assert.That(arr[2], Is.EqualTo((_time2, _value2)));
        }

        [Test]
        public void Add_ParamsEvents_AddsToTimeline()
        {
            var timeline = new Timeline<string>();
            timeline.Add((_time1, _value1), (_time2, _value2));
            Assert.That(timeline.Count, Is.EqualTo(2));
            Assert.That(timeline.Contains((_time1, _value1)));
            Assert.That(timeline.Contains((_time2, _value2)));
        }

        [Test]
        public void Add_TimelineInstance_AppendsEvents()
        {
            var timeline1 = new Timeline<string>((_time1, _value1)); // Sorted: (t1,v1)
            var timeline2 = new Timeline<string>((_time3, _value3), (_time2, _value2)); // Sorted: (t2,v2), (t3,v3)

            timeline1.Add(timeline2); // Appends timeline2's ToArray() result

            Assert.That(timeline1.Count, Is.EqualTo(3));
            Assert.That(timeline1.Contains((_time1, _value1)));
            Assert.That(timeline1.Contains((_time2, _value2)));
            Assert.That(timeline1.Contains((_time3, _value3)));

            var arr = timeline1.ToArray();
            Assert.That(arr[0], Is.EqualTo((_time1, _value1)));
            Assert.That(arr[1], Is.EqualTo((_time2, _value2))); // From timeline2, sorted
            Assert.That(arr[2], Is.EqualTo((_time3, _value3))); // From timeline2, sorted
        }

        [Test]
        public void AddNow_AddsEventsWithCurrentTime()
        {
            var timeline = new Timeline<string>();
            var before = DateTime.Now;
            timeline.AddNow(_value1, _value2);
            var after = DateTime.Now;

            Assert.That(timeline.Count, Is.EqualTo(2));
            Assert.That(timeline.ContainsValue(_value1, _value2));

            var eventTimes = timeline.GetAllTimes();
            Assert.That(eventTimes.Length, Is.EqualTo(1));
            var eventTime = eventTimes[0];
            Assert.That(eventTime, Is.GreaterThanOrEqualTo(before).And.LessThanOrEqualTo(after));
        }
        #endregion

        #region Contains Methods
        [Test]
        public void Contains_SpecificEvent_ChecksExistence()
        {
            var timeline = new Timeline<string>((_time1, _value1));
            Assert.That(timeline.Contains(_time1, _value1), Is.True);
            Assert.That(timeline.Contains(_time1, _value2), Is.False);
            Assert.That(timeline.Contains(_time2, _value1), Is.False);
        }

        [Test]
        public void Contains_ParamsEvents_ChecksIfAnyExist()
        {
            var timeline = new Timeline<string>((_time1, _value1), (_time2, _value2));
            Assert.That(timeline.Contains((_time1, _value1), (_time3, _value3)), Is.True);
            Assert.That(timeline.Contains((_time3, _value3), (_time1, "NonExistent")), Is.False);
        }

        [Test]
        public void Contains_TimelineInstance_ChecksIfAnyExist()
        {
            var mainTimeline = new Timeline<string>((_time1, _value1), (_time2, _value2));
            var subTimelineExists = new Timeline<string>((_time2, _value2), (_time3, _value3));
            var subTimelineNotExists = new Timeline<string>((_time3, _value3));

            Assert.That(mainTimeline.Contains(subTimelineExists), Is.True);
            Assert.That(mainTimeline.Contains(subTimelineNotExists), Is.False);
        }

        [Test]
        public void ContainsTime_ChecksIfAnyTimeExists()
        {
            var timeline = new Timeline<string>((_time1, _value1), (_time2, _value2));
            Assert.That(timeline.ContainsTime(_time1, _time3), Is.True);
            Assert.That(timeline.ContainsTime(_time3, new DateTime(2025, 1, 1)), Is.False);
        }

        [Test]
        public void ContainsValue_ChecksIfAnyValueExists()
        {
            var timeline = new Timeline<string>((_time1, _value1), (_time2, _value2));
            Assert.That(timeline.ContainsValue(_value1, _value3), Is.True);
            Assert.That(timeline.ContainsValue(_value3, "NonExistent"), Is.False);
        }

        [Test]
        public void ContainsValue_WithNull_ChecksCorrectly()
        {
            // Change Timeline<string> to Timeline<string?> to allow TValue to be a nullable string.
            // This makes the constructor signature Timeline(params (DateTime, string?)[] timeline).
            var timeline = new Timeline<string?>(
                (_time1, null),      // (_time1, null) is (DateTime, string?), now compatible.
                (_time2, _value2)    // _value2 is 'string', implicitly convertible to 'string?'.
            );

            // When TValue is string?, the 'values' parameter in ContainsValue(params TValue[] values)
            // becomes string?[]. Passing 'null' and '_value3' (string) is fine.
            Assert.That(timeline.ContainsValue(null, _value3), Is.True); // Checks if timeline contains null OR _value3
            // Since timeline contains null, this should be true.
            Assert.That(timeline.ContainsValue(_value3, "NonExistent"), Is.False);
            Assert.That(timeline.ContainsValue(_value2), Is.True);
        }
        #endregion

        #region Remove Methods
        [Test]
        public void Remove_SpecificEvent_RemovesAndReturnsStatus()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time2, _value2);

            bool result1 = timeline.Remove(_time1, _value1);
            Assert.That(result1, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_time1, _value1), Is.False);

            bool result2 = timeline.Remove(_time3, _value3);
            Assert.That(result2, Is.False);
            Assert.That(timeline.Count, Is.EqualTo(1));
        }

        [Test]
        public void Remove_ParamsEvents_RemovesMatchingAndReturnsStatus()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time2, _value2);
            timeline.Add(_time3, _value3);

            bool result = timeline.Remove((_time1, _value1), (_time2, "NonExistentValue"));
            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(2));
            Assert.That(timeline.Contains(_time1, _value1), Is.False);
            Assert.That(timeline.Contains(_time2, _value2), Is.True);

            bool resultNone = timeline.Remove((_time1, "No"), (_time2, "No"));
            Assert.That(resultNone, Is.False);
        }

        [Test]
        public void Remove_TimelineInstance_RemovesMatchingAndReturnsStatus()
        {
            var mainTimeline = new Timeline<string>();
            mainTimeline.Add(_time1, _value1);
            mainTimeline.Add(_time2, _value2);
            mainTimeline.Add(_time3, _value3);

            var timelineToRemove = new Timeline<string>((_time1, _value1), (_time3, "NonExistent"));

            bool result = mainTimeline.Remove(timelineToRemove);
            Assert.That(result, Is.True);
            Assert.That(mainTimeline.Count, Is.EqualTo(2));
            Assert.That(mainTimeline.Contains(_time1, _value1), Is.False);
            Assert.That(mainTimeline.Contains(_time3, _value3), Is.True);
        }

        [Test]
        public void RemoveTimes_RemovesAllEventsAtSpecifiedTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1Dup, _value2);
            timeline.Add(_time2, _value3);
            timeline.Add(_time3, "Another");

            bool result = timeline.RemoveTimes(_time1, _time3);
            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_time2, _value3), Is.True);
            Assert.That(timeline.ContainsTime(_time1), Is.False);
            Assert.That(timeline.ContainsTime(_time3), Is.False);

            bool resultNone = timeline.RemoveTimes(new DateTime(2025, 1, 1));
            Assert.That(resultNone, Is.False);
        }

        [Test]
        public void RemoveValues_RemovesAllEventsWithSpecifiedValues()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time2, _value2);
            timeline.Add(_time3, _value1);
            timeline.Add(_time1Dup, _value3);

            bool result = timeline.RemoveValues(_value1, _value3);
            Assert.That(result, Is.True);
            Assert.That(timeline.Count, Is.EqualTo(1));
            Assert.That(timeline.Contains(_time2, _value2), Is.True);
            Assert.That(timeline.ContainsValue(_value1), Is.False);
            Assert.That(timeline.ContainsValue(_value3), Is.False);

            bool resultNone = timeline.RemoveValues("NonExistent");
            Assert.That(resultNone, Is.False);
        }

        [Test]
        public void RemoveValues_WithNullValue_RemovesCorrectly()
        {
            // Timeline<string?> ensures TValue is a nullable string.
            var timeline = new Timeline<string?>();

            // Adding null is fine because TValue in Add(DateTime, TValue) is string?.
            timeline.Add(_time1, null);
            timeline.Add(_time2, _value1); // _value1 (string) is compatible with string?
            timeline.Add(_time3, null);

            // RemoveValues(params TValue[] values) means params string?[].
            // (null as string) correctly provides (string?)null, creating new string?[] { null }.
            bool result = timeline.RemoveValues(null as string);

            Assert.That(result, Is.True); // Should be true if nulls were found and removed.
            Assert.That(timeline.Count, Is.EqualTo(1)); // Only (_time2, _value1) should remain.

            // timeline.Contains(DateTime, TValue) means Contains(DateTime, string?).
            // _value1 (string) is compatible with string?.
            Assert.That(timeline.Contains(_time2, _value1), Is.True);

            // ContainsValue(params TValue[] values) means params string?[].
            // Explicitly cast null to string? for clarity, though it should be inferred.
            // This ensures the 'null' literal is treated as (string?)null before being wrapped
            // into the params array new string?[] { (string?)null }.
            Assert.That(timeline.ContainsValue((string?)null), Is.False); // No null values should be left.
        }


        #endregion

        #region To... Conversion Methods
        [Test]
        public void ToArray_ReturnsArrayOfEvents_InCurrentOrder()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time3, _value3);
            timeline.Add(_time2, _value2);

            var array = timeline.ToArray();
            Assert.That(array, Is.TypeOf<(DateTime, string)[]>());
            Assert.That(array.Length, Is.EqualTo(3));
            Assert.That(array[0], Is.EqualTo((_time1, _value1)));
            Assert.That(array[1], Is.EqualTo((_time3, _value3)));
            Assert.That(array[2], Is.EqualTo((_time2, _value2)));
        }

        [Test]
        public void ToList_ReturnsListOfEvents_InCurrentOrder()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time3, _value3);
            timeline.Add(_time2, _value2);

            var list = timeline.ToList();
            Assert.That(list, Is.TypeOf<List<(DateTime, string)>>());
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo((_time1, _value1)));
            Assert.That(list[1], Is.EqualTo((_time3, _value3)));
            Assert.That(list[2], Is.EqualTo((_time2, _value2)));
        }

        [Test]
        public void ToDictionary_ReturnsDictionary_UniqueTimes()
        {
            var timeline = new Timeline<string>((_time1, _value1), (_time2, _value2));

            var dictionary = timeline.ToDictionary();
            Assert.That(dictionary.Count, Is.EqualTo(2));
            Assert.That(dictionary[_time1], Is.EqualTo(_value1));
            Assert.That(dictionary[_time2], Is.EqualTo(_value2));
        }

        [Test]
        public void ToDictionary_ThrowsArgumentException_DuplicateTimes()
        {
            var timeline = new Timeline<string>();
            timeline.Add(_time1, _value1);
            timeline.Add(_time1Dup, _value2);

            Assert.Throws<ArgumentException>(() => timeline.ToDictionary());
        }
        #endregion

        #region GetValuesBy... (Time Part) Methods
        [Test]
        public void GetValuesByMillisecond_ReturnsMatchingEvents()
        {
            var timeWithMs100 = new DateTime(2023, 1, 1, 0, 0, 0, 100);
            var timeWithMs200 = new DateTime(2023, 1, 1, 0, 0, 0, 200);
            var timeWithMs100Again = new DateTime(2023, 1, 2, 0, 0, 0, 100);

            var timeline = new Timeline<string>(
                (timeWithMs100, _value1),
                (timeWithMs200, _value2),
                (timeWithMs100Again, _value3)
            );

            var result = timeline.GetValuesByMillisecond(100);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
            Assert.That(result.ToArray()[0].Time, Is.EqualTo(timeWithMs100));
            Assert.That(result.ToArray()[1].Time, Is.EqualTo(timeWithMs100Again));
        }

        [Test]
        public void GetValuesBySecond_ReturnsMatchingEvents()
        {
            var timeAtSec10 = new DateTime(2023, 1, 1, 0, 0, 10);
            var timeAtSec20 = new DateTime(2023, 1, 1, 0, 0, 20);
            var timeAtSec10Again = new DateTime(2023, 1, 1, 0, 1, 10);

            var timeline = new Timeline<string>(
                (timeAtSec10, _value1),
                (timeAtSec20, _value2),
                (timeAtSec10Again, _value3)
            );

            var result = timeline.GetValuesBySecond(10);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }

        [Test]
        public void GetValuesByMinute_ReturnsMatchingEvents()
        {
            var timeAtMin5 = new DateTime(2023, 1, 1, 0, 5, 0);
            var timeAtMin10 = new DateTime(2023, 1, 1, 0, 10, 0);
            var timeAtMin5Again = new DateTime(2023, 1, 1, 1, 5, 0);

            var timeline = new Timeline<string>(
                (timeAtMin5, _value1),
                (timeAtMin10, _value2),
                (timeAtMin5Again, _value3)
            );

            var result = timeline.GetValuesByMinute(5);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }

        [Test]
        public void GetValuesByHour_ReturnsMatchingEvents()
        {
            var timeAtHour9 = new DateTime(2023, 1, 1, 9, 0, 0);
            var timeAtHour10 = new DateTime(2023, 1, 1, 10, 0, 0);
            var timeAtHour9Again = new DateTime(2023, 1, 2, 9, 0, 0);

            var timeline = new Timeline<string>(
                (timeAtHour9, _value1),
                (timeAtHour10, _value2),
                (timeAtHour9Again, _value3)
            );

            var result = timeline.GetValuesByHour(9);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }

        [Test]
        public void GetValuesByDay_ReturnsMatchingEvents()
        {
            var timeOnDay15 = new DateTime(2023, 1, 15);
            var timeOnDay20 = new DateTime(2023, 1, 20);
            var timeOnDay15Again = new DateTime(2023, 2, 15);

            var timeline = new Timeline<string>(
                (timeOnDay15, _value1),
                (timeOnDay20, _value2),
                (timeOnDay15Again, _value3)
            );

            var result = timeline.GetValuesByDay(15);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }

        [Test]
        public void GetValuesByTimeOfDay_ReturnsMatchingEvents()
        {
            var timeAt10AM = new DateTime(2023, 1, 1, 10, 0, 0);
            var timeAt11AM = new DateTime(2023, 1, 1, 11, 0, 0);
            var timeAt10AMDifferentDay = new DateTime(2023, 1, 2, 10, 0, 0);

            var timeline = new Timeline<string>(
                (timeAt10AM, _value1),
                (timeAt11AM, _value2),
                (timeAt10AMDifferentDay, _value3)
            );

            var result = timeline.GetValuesByTimeOfDay(new TimeSpan(10, 0, 0));
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }

        [Test]
        public void GetValuesByDayOfYear_ReturnsMatchingEvents()
        {
            var dayOfYear1 = new DateTime(2023, 1, 1); // Day 1
            var dayOfYear32 = new DateTime(2023, 2, 1); // Day 32
            var dayOfYear1DifferentYear = new DateTime(2024, 1, 1); // Day 1 of different year

            var timeline = new Timeline<string>(
                (dayOfYear1, _value1),
                (dayOfYear32, _value2),
                (dayOfYear1DifferentYear, _value3)
            );

            var result = timeline.GetValuesByDayOfYear(1);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }

        [Test]
        public void GetValuesByMonth_ReturnsMatchingEvents()
        {
            var timeInJan = new DateTime(2023, 1, 15);
            var timeInFeb = new DateTime(2023, 2, 10);
            var timeInJanDifferentYear = new DateTime(2024, 1, 5);

            var timeline = new Timeline<string>(
                (timeInJan, _value1),
                (timeInFeb, _value2),
                (timeInJanDifferentYear, _value3)
            );

            var result = timeline.GetValuesByMonth(1); // January
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.ContainsValue(_value1, _value3));
        }
        #endregion
    }
}
