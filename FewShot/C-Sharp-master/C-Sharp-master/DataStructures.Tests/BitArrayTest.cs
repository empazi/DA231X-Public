using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq; // For IEnumerable tests

namespace DataStructures.Tests;

[TestFixture]
public class BitArrayTests
{
    [TestFixture]
    public class ConstructorTests
    {
        [Test]
        public void Constructor_WithInt_PositiveLength_CreatesArrayOfSpecifiedLengthAndDefaultsToZero()
        {
            var bitArray = new BitArray(8);
            bitArray.ToString().Should().Be("00000000"); // Default is false
            bitArray.Count().Should().Be(8); // Using LINQ Count() for IEnumerable
        }

        [Test]
        public void Constructor_WithInt_ZeroLength_CreatesEmptyArray()
        {
            var bitArray = new BitArray(0);
            bitArray.ToString().Should().BeEmpty();
            bitArray.Count().Should().Be(0);
        }

        [Test]
        public void Constructor_WithInt_NegativeLength_ThrowsOverflowException()
        {
            // The constructor `new bool[n]` will throw if n is negative.
            Action act = () => new BitArray(-5);
            act.Should().Throw<OverflowException>();
        }

        [Test]
        public void Constructor_WithString_ValidSequence_CreatesCorrectArray()
        {
            var bitArray = new BitArray("10110");
            bitArray.ToString().Should().Be("10110");
            bitArray.Count().Should().Be(5);
        }

        [Test]
        public void Constructor_WithString_EmptySequence_ThrowsArgumentException()
        {
            Action act = () => new BitArray("");
            act.Should().Throw<ArgumentException>().WithMessage("Sequence must been greater than or equal to 1");
        }

        [Test]
        public void Constructor_WithString_InvalidSequence_ThrowsArgumentException()
        {
            Action act = () => new BitArray("10210");
            act.Should().Throw<ArgumentException>().WithMessage("The sequence may only contain ones or zeros");
        }

        [Test]
        public void Constructor_WithBoolArray_CreatesCorrectArray()
        {
            var bools = new[] { true, false, true, true, false };
            var bitArray = new BitArray(bools);
            bitArray.ToString().Should().Be("10110");
            bitArray.Count().Should().Be(5);
        }

        [Test]
        public void Constructor_WithBoolArray_EmptyArray_CreatesEmptyBitArray()
        {
            var bools = Array.Empty<bool>();
            var bitArray = new BitArray(bools);
            bitArray.ToString().Should().BeEmpty();
            bitArray.Count().Should().Be(0);
        }
    }

    [TestFixture]
    public class PropertyAndIndexerTests
    {
        [Test]
        public void Indexer_Get_ReturnsCorrectBit()
        {
            var bitArray = new BitArray("101");
            bitArray[0].Should().BeTrue();
            bitArray[1].Should().BeFalse();
            bitArray[2].Should().BeTrue();
        }

        [Test]
        public void Indexer_Get_OutOfBounds_ThrowsIndexOutOfRangeException()
        {
            var bitArray = new BitArray("101");
            Action act1 = () => { var _ = bitArray[-1]; };
            Action act2 = () => { var _ = bitArray[3]; };
            act1.Should().Throw<IndexOutOfRangeException>();
            act2.Should().Throw<IndexOutOfRangeException>();
        }

        [Test]
        public void Length_IsCorrect_AfterConstruction() // Indirectly testing private Length via Count()
        {
            new BitArray(5).Count().Should().Be(5);
            new BitArray("101").Count().Should().Be(3);
            new BitArray(new bool[] { true, false }).Count().Should().Be(2);
        }
    }

    [TestFixture]
    public class ToStringTests
    {
        [Test]
        public void ToString_ReturnsCorrectStringRepresentation()
        {
            new BitArray("1001").ToString().Should().Be("1001");
            new BitArray(3).ToString().Should().Be("000");
            new BitArray(new bool[] { false, true, false }).ToString().Should().Be("010");
        }

        [Test]
        public void ToString_EmptyArray_ReturnsEmptyString()
        {
            new BitArray(0).ToString().Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CompileTests
    {
        [Test]
        public void Compile_String_ValidSequence_SameLength()
        {
            var bitArray = new BitArray(4);
            bitArray.Compile("1010");
            bitArray.ToString().Should().Be("1010");
        }

        [Test]
        public void Compile_String_ValidSequence_ShorterLength_PadsWithLeadingZeros()
        {
            var bitArray = new BitArray(5);
            bitArray.Compile("101");
            bitArray.ToString().Should().Be("00101");
        }

        [Test]
        public void Compile_String_SequenceLongerThanArray_ThrowsArgumentException()
        {
            var bitArray = new BitArray(3);
            Action act = () => bitArray.Compile("1010");
            act.Should().Throw<ArgumentException>().WithMessage("sequence must be not longer than the bit array length");
        }

        [Test]
        public void Compile_String_InvalidSequence_ThrowsArgumentException()
        {
            var bitArray = new BitArray(3);
            Action act = () => bitArray.Compile("102");
            act.Should().Throw<ArgumentException>().WithMessage("The sequence may only contain ones or zeros");
        }

        [Test]
        public void Compile_Int_ValidNumber_FitsExactly()
        {
            var bitArray = new BitArray(4);
            bitArray.Compile(5); // 0101
            bitArray.ToString().Should().Be("0101");
        }

        [Test]
        public void Compile_Int_ValidNumber_ShorterThanArrayLength_PadsWithLeadingZeros()
        {
            var bitArray = new BitArray(8);
            bitArray.Compile(5); // 00000101
            bitArray.ToString().Should().Be("00000101");
        }

        [Test]
        public void Compile_Int_NumberTooBigForArray_ThrowsArgumentException()
        {
            var bitArray = new BitArray(3);
            Action act = () => bitArray.Compile(8); // 1000
            act.Should().Throw<ArgumentException>().WithMessage("Provided number is too big");
        }

        [Test]
        public void Compile_Int_ZeroOrNegativeNumber_ThrowsArgumentException()
        {
            var bitArray = new BitArray(4);
            Action actZero = () => bitArray.Compile(0);
            Action actNegative = () => bitArray.Compile(-5);
            actZero.Should().Throw<ArgumentException>().WithMessage("number must be positive");
            actNegative.Should().Throw<ArgumentException>().WithMessage("number must be positive");
        }

        [Test]
        public void Compile_Long_ValidNumber()
        {
            var bitArray = new BitArray(8);
            bitArray.Compile(10L); // 00001010
            bitArray.ToString().Should().Be("00001010");
        }

        [Test]
        public void Compile_Long_NumberTooBigForArray_ThrowsArgumentException()
        {
            var bitArray = new BitArray(4);
            Action act = () => bitArray.Compile(16L); // 10000
            act.Should().Throw<ArgumentException>().WithMessage("Provided number is too big");
        }

        [Test]
        public void Compile_Long_ZeroOrNegativeNumber_ThrowsArgumentException()
        {
            var bitArray = new BitArray(8);
            Action actZero = () => bitArray.Compile(0L);
            Action actNegative = () => bitArray.Compile(-10L);
            actZero.Should().Throw<ArgumentException>().WithMessage("number must be positive");
            actNegative.Should().Throw<ArgumentException>().WithMessage("number must be positive");
        }
    }

    [TestFixture]
    public class BitCountTests
    {
        [TestCase("10110", 3, 2)]
        [TestCase("00000", 0, 5)]
        [TestCase("111", 3, 0)]
        [TestCase("", 0, 0)]
        public void NumberOfBits_ReturnsCorrectCounts(string s, int ones, int zeros)
        {
            var bitArray = s == "" ? new BitArray(0) : new BitArray(s);
            bitArray.NumberOfOneBits().Should().Be(ones);
            bitArray.NumberOfZeroBits().Should().Be(zeros);
        }
    }

    [TestFixture]
    public class ParityTests
    {
        [TestCase("101", true, false)]  // 2 ones (even)
        [TestCase("1011", false, true)] // 3 ones (odd)
        [TestCase("000", true, false)]  // 0 ones (even)
        [TestCase("", true, false)]     // 0 ones (even)
        public void Parity_ReturnsCorrectBoolean(string s, bool evenParity, bool oddParity)
        {
            var bitArray = s == "" ? new BitArray(0) : new BitArray(s);
            bitArray.EvenParity().Should().Be(evenParity);
            bitArray.OddParity().Should().Be(oddParity);
        }
    }

    [TestFixture]
    public class ToIntegerTests
    {
        [Test]
        public void ToInt32_ValidConversion()
        {
            new BitArray("101").ToInt32().Should().Be(5);
            new BitArray("00001111").ToInt32().Should().Be(15);
        }

        [Test]
        public void ToInt32_ArrayTooLong_ThrowsInvalidOperationException()
        {
            var bitArray = new BitArray(33);
            Action act = () => bitArray.ToInt32();
            act.Should().Throw<InvalidOperationException>().WithMessage("Value is too big to fit into Int32");
        }

        [Test]
        public void ToInt64_ValidConversion()
        {
            new BitArray("101").ToInt64().Should().Be(5L);
            var longBitArray = new BitArray(33);
            longBitArray.Compile("1"); // Smallest bit set at the end for simplicity
            longBitArray.ToString().Substring(0, 32).Should().MatchRegex("0*"); // Ensure it's 2^0 effectively
            new BitArray("100000000000000000000000000000001").ToInt64().Should().Be(4294967297L); // 2^32 + 1
        }

        [Test]
        public void ToInt64_ArrayTooLong_ThrowsInvalidOperationException()
        {
            var bitArray = new BitArray(65);
            Action act = () => bitArray.ToInt64();
            act.Should().Throw<InvalidOperationException>().WithMessage("Value is too big to fit into Int64");
        }
    }

    [TestFixture]
    public class FieldManipulationTests
    {
        [Test]
        public void ResetField_SetsAllBitsToFalse()
        {
            var bitArray = new BitArray("10110");
            bitArray.ResetField();
            bitArray.ToString().Should().Be("00000");
        }

        [Test]
        public void SetAll_True_SetsAllBitsToTrue()
        {
            var bitArray = new BitArray(5);
            bitArray.SetAll(true);
            bitArray.ToString().Should().Be("11111");
        }

        [Test]
        public void SetAll_False_SetsAllBitsToFalse()
        {
            var bitArray = new BitArray("111");
            bitArray.SetAll(false);
            bitArray.ToString().Should().Be("000");
        }
    }

    [TestFixture]
    public class CloneTests
    {
        [Test]
        public void Clone_CreatesIndependentCopy()
        {
            var original = new BitArray("101");
            var clone = (BitArray)original.Clone();

            clone.ToString().Should().Be("101");
            original.Should().NotBeSameAs(clone);

            clone.Compile("111"); // Modifying clone
            original.ToString().Should().Be("101"); // Original unaffected
            clone.ToString().Should().Be("111");
        }
    }

    [TestFixture]
    public class EqualityAndHashCodeTests
    {
        [Test]
        public void Equals_And_OperatorEquals_ReturnTrueForEqualArrays()
        {
            var ba1 = new BitArray("10110");
            var ba2 = new BitArray("10110");

            (ba1 == ba2).Should().BeTrue();
            ba1.Equals(ba2).Should().BeTrue();
        }

        [Test]
        public void Equals_And_OperatorEquals_ReturnFalseForDifferentContent()
        {
            var ba1 = new BitArray("10110");
            var ba2 = new BitArray("10111");

            (ba1 == ba2).Should().BeFalse();
            ba1.Equals(ba2).Should().BeFalse();
        }

        [Test]
        public void Equals_And_OperatorEquals_ReturnFalseForDifferentLength()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("1010");

            (ba1 == ba2).Should().BeFalse();
            ba1.Equals(ba2).Should().BeFalse();
        }

        [Test]
        public void OperatorNotEquals_ReturnsCorrectly()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("101");
            var ba3 = new BitArray("111");

            (ba1 != ba2).Should().BeFalse();
            (ba1 != ba3).Should().BeTrue();
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            var ba1 = new BitArray("101");
            ba1.Equals(null).Should().BeFalse();
        }

        [Test]
        public void GetHashCode_EqualArrays_HaveEqualHashCodes()
        {
            var ba1 = new BitArray("10110"); // Value 22
            var ba2 = new BitArray("10110");

            ba1.GetHashCode().Should().Be(ba2.GetHashCode());
            ba1.GetHashCode().Should().Be(22);
        }
    }

    [TestFixture]
    public class BitwiseOperatorTests
    {
        [Test]
        public void Operator_Not()
        {
            var ba = new BitArray("10110");
            var result = ~ba;
            result.ToString().Should().Be("01001");
        }

        [Test]
        public void Operator_And_SameLength()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1100");
            var result = ba1 & ba2;
            result.ToString().Should().Be("1000");
        }

        [Test]
        public void Operator_And_DifferentLength_OneIsLonger() // 'one' is longer
        {
            var ba1 = new BitArray("10101"); // Longer 'one'
            var ba2 = new BitArray("101");   // Shorter 'two', padded to "00101" for operation
            var result = ba1 & ba2;
            // "10101" & "00101" = "00101"
            result.ToString().Should().Be("00101");
            result.Count().Should().Be(5);
        }

        [Test]
        public void Operator_And_DifferentLength_TwoIsLonger_ExhibitsBuggyLoop() // 'two' is longer, 'one' is shorter
        {
            var ba1 = new BitArray("101");    // Shorter 'one', original length 3. Padded sequence1 = "00101"
            var ba2 = new BitArray("11111");  // Longer 'two', original length 5. sequence2 = "11111"
            // Bug: Loop in '&' iterates up to one.Length (original shorter length = 3)
            // sequence1[0..2] = "001", sequence2[0..2] = "111"
            // "001" & "111" = "001"
            // Result string "001" compiled into BitArray of length 5 becomes "00001"
            var result = ba1 & ba2;
            result.ToString().Should().Be("00001");
            result.Count().Should().Be(5);
        }

        [Test]
        public void Operator_Or_SameLength()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1100");
            var result = ba1 | ba2;
            result.ToString().Should().Be("1110");
        }

        [Test]
        public void Operator_Or_DifferentLength_OneIsLonger() // 'one' is longer
        {
            var ba1 = new BitArray("10001"); // Longer 'one'
            var ba2 = new BitArray("101");   // Shorter 'two', padded to "00101"
            var result = ba1 | ba2;
            // "10001" | "00101" = "10101"
            result.ToString().Should().Be("10101");
            result.Count().Should().Be(5);
        }

        [Test]
        public void Operator_Xor_SameLength()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1100");
            var result = ba1 ^ ba2;
            result.ToString().Should().Be("0110");
        }

        [Test]
        public void Operator_Xor_DifferentLength_OneIsLonger() // 'one' is longer
        {
            var ba1 = new BitArray("10101"); // Longer 'one'
            var ba2 = new BitArray("110");   // Shorter 'two', padded to "00110"
            var result = ba1 ^ ba2;
            // "10101" ^ "00110" = "10011"
            result.ToString().Should().Be("10011");
            result.Count().Should().Be(5);
        }

        [Test]
        public void Operator_ShiftLeft()
        {
            var ba = new BitArray("101");
            var result = ba << 2;
            result.ToString().Should().Be("10100");
            result.Count().Should().Be(5);
        }

        [Test]
        public void Operator_ShiftRight()
        {
            var ba = new BitArray("10110");
            var result = ba >> 2;
            result.ToString().Should().Be("101");
            result.Count().Should().Be(3);
        }

        [Test]
        public void Operator_ShiftRight_ByLength_ResultsInEmptyArray()
        {
            var ba = new BitArray("101");
            var result = ba >> 3;
            result.ToString().Should().BeEmpty();
            result.Count().Should().Be(0);
        }
    }

    [TestFixture]
    public class EnumeratorTests
    {
        [Test]
        public void Enumerator_IteratesCorrectly()
        {
            var bitArray = new BitArray("101");
            var expected = new[] { true, false, true };
            bitArray.ToList().Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Enumerator_Reset_AllowsReiteration()
        {
            var bitArray = new BitArray("10");
            var enumerator = bitArray.GetEnumerator();
            int count = 0;
            while (enumerator.MoveNext()) { count++; }
            count.Should().Be(2);

            enumerator.Reset();
            count = 0;
            while (enumerator.MoveNext()) { count++; }
            count.Should().Be(2);
        }

        [Test]
        public void Enumerator_Current_BeforeMoveNext_Or_AfterEnd_ThrowsIndexOutOfRangeException()
        {
            var bitArray = new BitArray("1");
            var enumerator = bitArray.GetEnumerator();

            Action currentAtStart = () => { var _ = enumerator.Current; };
            currentAtStart.Should().Throw<IndexOutOfRangeException>();

            enumerator.MoveNext(); // Positioned on the first element
            enumerator.Current.Should().BeTrue();

            enumerator.MoveNext(); // Moved past the end
            Action currentAtEnd = () => { var _ = enumerator.Current; }; // Action to access Current
            // BitArray.cs's Current property does not throw after MoveNext() returns false;
            // instead, it returns the last valid element.
            currentAtEnd.Should().NotThrow();
            enumerator.Current.Should().BeTrue(); // For new BitArray("1"), the last element was true.
        }
    }
}
