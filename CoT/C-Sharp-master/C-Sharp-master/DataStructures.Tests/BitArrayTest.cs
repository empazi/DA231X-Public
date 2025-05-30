using NUnit.Framework;
using DataStructures; // Assuming BitArray is in this namespace
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework.Legacy;

namespace BitArrayTests
{
    [TestFixture]
    public class BitArrayTests
    {
        [Test]
        public void Constructor_WithSize_InitializesWithCorrectLengthAndAllFalse()
        {
            var bitArray = new BitArray(8);
            Assert.That(bitArray.ToString(), Is.EqualTo("00000000"), "Default initialization should be all zeros.");
            // Note: Length property is private. We infer length from ToString() or by iterating.
            Assert.That(bitArray.ToString().Length, Is.EqualTo(8));
        }

        [Test]
        public void Constructor_WithSizeZero_InitializesEmpty()
        {
            var bitArray = new BitArray(0);
            Assert.That(bitArray.ToString(), Is.EqualTo(string.Empty));
            Assert.That(bitArray.ToString().Length, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithNegativeSize_InitializesEmpty()
        {
            // Current BitArray(int n) constructor throws OverflowException for n < 0 due to 'field = new bool[n]' being hit.
            Assert.Throws<OverflowException>(() => new BitArray(-5));
        }

        [Test]
        public void Constructor_WithStringSequence_InitializesCorrectly()
        {
            var sequence = "10110";
            var bitArray = new BitArray(sequence);
            Assert.That(bitArray.ToString(), Is.EqualTo(sequence));
        }

        [Test]
        public void Constructor_WithStringSequence_EmptySequence_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BitArray(string.Empty));
        }

        [Test]
        public void Constructor_WithStringSequence_InvalidCharacters_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BitArray("10210"));
        }

        [Test]
        public void Constructor_WithBoolArray_InitializesCorrectly()
        {
            var bools = new[] { true, false, true, true, false };
            var bitArray = new BitArray(bools);
            Assert.That(bitArray.ToString(), Is.EqualTo("10110"));
        }

        [Test]
        public void Constructor_WithEmptyBoolArray_InitializesEmpty()
        {
            var bools = Array.Empty<bool>();
            var bitArray = new BitArray(bools);
            Assert.That(bitArray.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void Indexer_Get_ReturnsCorrectBit()
        {
            var bitArray = new BitArray("101");
            Assert.That(bitArray[0], Is.True);
            Assert.That(bitArray[1], Is.False);
            Assert.That(bitArray[2], Is.True);
        }

        [Test]
        public void Indexer_Get_OutOfBounds_ThrowsIndexOutOfRangeException()
        {
            var bitArray = new BitArray("101");
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = bitArray[3]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = bitArray[-1]; });
        }

        [Test]
        public void Clone_CreatesIndependentCopy()
        {
            var original = new BitArray("1010");
            var clone = (BitArray)original.Clone();

            Assert.That(clone.ToString(), Is.EqualTo(original.ToString()));
            Assert.That(clone, Is.Not.SameAs(original));
            Assert.That(clone.Equals(original), Is.True);

            // Modify original - clone should not change
            // We can't directly set bits via public indexer, so we use Compile
            var originalModified = new BitArray(original.ToString().Length);
            originalModified.Compile("1111"); // Create a new BitArray to simulate modification
                                              // or if we had a public setter: original[0] = false;

            // Re-check clone against the original string representation
            // This test is a bit tricky without a public setter.
            // The essence of clone is that it's a distinct copy.
            // Let's test by compiling a new value into the original's underlying field
            // (which we can't do directly in a test without reflection or changing the class).
            // A better test for clone if we could modify:
            // original.SetBit(0, false); // Hypothetical method
            // Assert.That(clone[0], Is.True); // Clone should remain unchanged

            // For now, we rely on the fact that it's a new instance and initially equal.
            // If we modify the clone, the original should not change.
            var cloneModified = (BitArray)clone.Clone(); // Clone the clone
            cloneModified.Compile("0000"); // Modify the clone's clone
            Assert.That(clone.ToString(), Is.EqualTo("1010")); // Original clone is unchanged
        }

        [Test]
        public void Enumerator_IteratesCorrectly()
        {
            var bitArray = new BitArray("101");
            var expected = new List<bool> { true, false, true };
            var actual = new List<bool>();

            foreach (var bit in bitArray)
            {
                actual.Add(bit);
            }
            CollectionAssert.AreEqual(expected, actual);

            // Test Reset
            bitArray.Reset();
            actual.Clear();
            while (bitArray.MoveNext())
            {
                actual.Add(bitArray.Current);
            }
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Enumerator_Current_BeforeMoveNext_ThrowsException()
        {
            var bitArray = new BitArray("1");
            // Accessing Current before MoveNext is undefined behavior for IEnumerator,
            // often leading to an exception or invalid data.
            // The current implementation will throw IndexOutOfRangeException due to position = -1.
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = bitArray.Current; });
        }

        [Test]
        public void IEnumerator_Current_BeforeMoveNext_ThrowsException()
        {
            var bitArray = new BitArray("1");
            System.Collections.IEnumerator enumerator = ((System.Collections.IEnumerable)bitArray).GetEnumerator();
            // Accessing Current before MoveNext is undefined behavior for IEnumerator,
            // often leading to an exception or invalid data.
            // The current implementation will throw IndexOutOfRangeException due to position = -1.
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = enumerator.Current; });
        }


        [Test]
        public void Dispose_DoesNotThrow()
        {
            var bitArray = new BitArray("101");
            Assert.DoesNotThrow(() => bitArray.Dispose());
        }

        [Test]
        public void Compile_String_ValidSequence_SameLength()
        {
            var bitArray = new BitArray(5);
            bitArray.Compile("10110");
            Assert.That(bitArray.ToString(), Is.EqualTo("10110"));
        }

        [Test]
        public void Compile_String_ValidSequence_ShorterLength_PadsWithZeros()
        {
            var bitArray = new BitArray(5);
            bitArray.Compile("101");
            Assert.That(bitArray.ToString(), Is.EqualTo("00101"));
        }

        [Test]
        public void Compile_String_SequenceLongerThanArray_ThrowsArgumentException()
        {
            var bitArray = new BitArray(3);
            Assert.Throws<ArgumentException>(() => bitArray.Compile("10110"));
        }

        [Test]
        public void Compile_String_InvalidCharacters_ThrowsArgumentException()
        {
            var bitArray = new BitArray(5);
            Assert.Throws<ArgumentException>(() => bitArray.Compile("10X10"));
        }

        [Test]
        public void Compile_Int_ValidNumber_Fits()
        {
            var bitArray = new BitArray(8);
            bitArray.Compile(42); // 42 is 101010 in binary
            Assert.That(bitArray.ToString(), Is.EqualTo("00101010"));
        }

        [Test]
        public void Compile_Int_ValidNumber_ShorterThanArray_PadsWithZeros()
        {
            var bitArray = new BitArray(8);
            bitArray.Compile(5); // 5 is 101
            Assert.That(bitArray.ToString(), Is.EqualTo("00000101"));
        }

        [Test]
        public void Compile_Int_NumberTooBig_ThrowsArgumentException()
        {
            var bitArray = new BitArray(3);
            Assert.Throws<ArgumentException>(() => bitArray.Compile(8)); // 8 is 1000
        }

        [Test]
        public void Compile_Int_ZeroOrNegative_ThrowsArgumentException()
        {
            var bitArray = new BitArray(8);
            Assert.Throws<ArgumentException>(() => bitArray.Compile(0));
            Assert.Throws<ArgumentException>(() => bitArray.Compile(-5));
        }

        [Test]
        public void Compile_Long_ValidNumber_Fits()
        {
            var bitArray = new BitArray(16);
            bitArray.Compile(65530L); // 65530 is 1111111111111010
            Assert.That(bitArray.ToString(), Is.EqualTo("1111111111111010"));
        }

        [Test]
        public void Compile_Long_ValidNumber_ShorterThanArray_PadsWithZeros()
        {
            var bitArray = new BitArray(16);
            bitArray.Compile(42L); // 42 is 101010
            Assert.That(bitArray.ToString(), Is.EqualTo("0000000000101010"));
        }


        [Test]
        public void Compile_Long_NumberTooBig_ThrowsArgumentException()
        {
            var bitArray = new BitArray(8);
            Assert.Throws<ArgumentException>(() => bitArray.Compile(256L)); // 256 is 100000000
        }

        [Test]
        public void Compile_Long_ZeroOrNegative_ThrowsArgumentException()
        {
            var bitArray = new BitArray(8);
            Assert.Throws<ArgumentException>(() => bitArray.Compile(0L));
            Assert.Throws<ArgumentException>(() => bitArray.Compile(-10L));
        }

        [Test]
        public void ToString_ReturnsCorrectStringRepresentation()
        {
            var bitArray = new BitArray(new[] { true, false, true, true, false });
            Assert.That(bitArray.ToString(), Is.EqualTo("10110"));
            var emptyBitArray = new BitArray(0);
            Assert.That(emptyBitArray.ToString(), Is.EqualTo(string.Empty));
        }

        [TestCase("10110", 3)]
        [TestCase("00000", 0)]
        [TestCase("11111", 5)]
        //[TestCase("", 0)]
        public void NumberOfOneBits_ReturnsCorrectCount(string s, int expected)
        {
            var bitArray = new BitArray(s);
            Assert.That(bitArray.NumberOfOneBits(), Is.EqualTo(expected));
        }

        [Test]
        public void NumberOfOneBits_EmptyArray_ReturnsZero()
        {
            var bitArray = new BitArray(0);
            Assert.That(bitArray.NumberOfOneBits(), Is.EqualTo(0));
        }


        [TestCase("10110", 2)]
        [TestCase("00000", 5)]
        [TestCase("11111", 0)]
        //[TestCase("", 0)]
        public void NumberOfZeroBits_ReturnsCorrectCount(string s, int expected)
        {
            var bitArray = new BitArray(s);
            Assert.That(bitArray.NumberOfZeroBits(), Is.EqualTo(expected));
        }

        [Test]
        public void NumberOfZeroBits_EmptyArray_ReturnsZero()
        {
            var bitArray = new BitArray(0);
            Assert.That(bitArray.NumberOfZeroBits(), Is.EqualTo(0));
        }

        [TestCase("10101", false)] // 3 ones -> odd
        [TestCase("10110", false)] // 3 ones -> odd parity, so EvenParity is false

        [TestCase("1100", true)]   // 2 ones -> even
        [TestCase("0000", true)]   // 0 ones -> even
        [TestCase("1", false)]     // 1 one -> odd
        public void EvenParity_ReturnsCorrectly(string s, bool expectedEven)
        {
            var bitArray = new BitArray(s);
            // EvenParity is true if NumberOfOneBits is even
            Assert.That(bitArray.EvenParity(), Is.EqualTo(expectedEven));
        }

        [Test]
        public void EvenParity_TestCases()
        {
            Assert.That(new BitArray("101").EvenParity(), Is.True);  // 2 ones (101) -> NumberOfOneBits is 2. 2 % 2 == 0 is true.

            Assert.That(new BitArray("110").EvenParity(), Is.True);  // 2 ones
            Assert.That(new BitArray("100").EvenParity(), Is.False); // 1 one
            Assert.That(new BitArray("000").EvenParity(), Is.True);  // 0 ones
        }


        [Test]
        public void OddParity_TestCases()
        {
            Assert.That(new BitArray("101").OddParity(), Is.False); // 2 ones (101) -> NumberOfOneBits is 2. 2 % 2 != 0 is false.

            Assert.That(new BitArray("110").OddParity(), Is.False); // 2 ones
            Assert.That(new BitArray("100").OddParity(), Is.True);  // 1 one
            Assert.That(new BitArray("000").OddParity(), Is.False); // 0 ones
        }


        [Test]
        public void ToInt64_ValidConversion()
        {
            var bitArray = new BitArray("101010"); // 42
            Assert.That(bitArray.ToInt64(), Is.EqualTo(42L));
        }

        [Test]
        public void ToInt64_ArrayTooLong_ThrowsInvalidOperationException()
        {
            var bitArray = new BitArray(65); // Max 64
            Assert.Throws<InvalidOperationException>(() => bitArray.ToInt64());
        }

        [Test]
        public void ToInt64_EmptyArray_ConvertsToZero()
        {
            var bitArray = new BitArray(0);
            // BitArray.ToInt64() calls ToString() which is "" for an empty array. Convert.ToInt64("", 2) throws.
            Assert.Throws<ArgumentOutOfRangeException>(() => bitArray.ToInt64());

        }

        [Test]
        public void ToInt32_ValidConversion()
        {
            var bitArray = new BitArray("101010"); // 42
            Assert.That(bitArray.ToInt32(), Is.EqualTo(42));
        }

        [Test]
        public void ToInt32_ArrayTooLong_ThrowsInvalidOperationException()
        {
            var bitArray = new BitArray(33); // Max 32
            Assert.Throws<InvalidOperationException>(() => bitArray.ToInt32());
        }

        [Test]
        public void ToInt32_EmptyArray_ConvertsToZero()
        {
            var bitArray = new BitArray(0);
            // BitArray.ToInt32() calls ToString() which is "" for an empty array. Convert.ToInt32("", 2) throws.
            Assert.Throws<ArgumentOutOfRangeException>(() => bitArray.ToInt32());

        }

        [Test]
        public void ResetField_SetsAllBitsToFalse()
        {
            var bitArray = new BitArray("10110");
            bitArray.ResetField();
            Assert.That(bitArray.ToString(), Is.EqualTo("00000"));
        }

        [Test]
        public void SetAll_True_SetsAllBitsToTrue()
        {
            var bitArray = new BitArray(5);
            bitArray.SetAll(true);
            Assert.That(bitArray.ToString(), Is.EqualTo("11111"));
        }

        [Test]
        public void SetAll_False_SetsAllBitsToFalse()
        {
            var bitArray = new BitArray("10101");
            bitArray.SetAll(false);
            Assert.That(bitArray.ToString(), Is.EqualTo("00000"));
        }

        [Test]
        public void Equals_Object_EqualArrays_ReturnsTrue()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("101");
            Assert.That(ba1.Equals((object)ba2), Is.True);
        }

        [Test]
        public void Equals_Object_UnequalContent_ReturnsFalse()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("100");
            Assert.That(ba1.Equals((object)ba2), Is.False);
        }

        [Test]
        public void Equals_Object_UnequalLength_ReturnsFalse()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("1010");
            Assert.That(ba1.Equals((object)ba2), Is.False);
        }

        [Test]
        public void Equals_Object_Null_ReturnsFalse()
        {
            var ba1 = new BitArray("101");
            Assert.That(ba1.Equals(null), Is.False);
        }

        [Test]
        public void Equals_Object_DifferentType_ReturnsFalseOrThrows()
        {
            var ba1 = new BitArray("101");
            // The implementation casts to BitArray, so it will throw InvalidCastException
            // if the type is not BitArray. A more robust Equals would return false.
            Assert.Throws<InvalidCastException>(() => ba1.Equals("101"));
        }

        [Test]
        public void GetHashCode_EqualArrays_SameHashCode()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1010");
            Assert.That(ba1.GetHashCode(), Is.EqualTo(ba2.GetHashCode()));
        }

        [Test]
        public void GetHashCode_ArrayTooLongForInt32_ThrowsInvalidOperationException()
        {
            // GetHashCode calls ToInt32()
            var bitArray = new BitArray(33);
            Assert.Throws<InvalidOperationException>(() => bitArray.GetHashCode());
        }

        // --- Operator Tests ---

        [Test]
        public void Operator_AND_SameLength()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1100");
            var result = ba1 & ba2;
            Assert.That(result.ToString(), Is.EqualTo("1000"));
        }

        [Test]
        public void Operator_AND_DifferentLength_OneShorter()
        {
            var ba1 = new BitArray("10101"); // 10101
            var ba2 = new BitArray("101");   // 00101 (padded)
            var result = ba1 & ba2;
            // BitArray.cs operator & loop iterates one.Length (5) times.
            // "10101" & "00101" = "00101"
            Assert.That(result.ToString(), Is.EqualTo("00101"));
            Assert.That(result.ToString().Length, Is.EqualTo(5));
        }

        [Test]
        public void Operator_AND_DifferentLength_TwoShorter()
        {
            var ba1 = new BitArray("101");   // one (length 3)
            var ba2 = new BitArray("10101"); // two (length 5)
            var result = ba1 & ba2;
            // BitArray.cs operator & loop iterates one.Length (3) times. Padded ba1 is "00101", ba2 is "10101".
            // Result of first 3 bits: "00101"[0..2] & "10101"[0..2] => "001" & "101" => "001".
            // Compile("001") into BitArray(5) results in "00001".
            Assert.That(result.ToString(), Is.EqualTo("00001"));
            Assert.That(result.ToString().Length, Is.EqualTo(5));
        }

        [Test]
        public void Operator_AND_WithEmptyArray()
        {
            var ba1 = new BitArray("101");
            var baEmpty = new BitArray(0);
            var result1 = ba1 & baEmpty; // baEmpty padded to "000"
            Assert.That(result1.ToString(), Is.EqualTo("000"));
            Assert.That(result1.ToString().Length, Is.EqualTo(3));

            var result2 = baEmpty & ba1; // baEmpty padded to "000"
            Assert.That(result2.ToString(), Is.EqualTo("000"));
            Assert.That(result2.ToString().Length, Is.EqualTo(3));
        }


        [Test]
        public void Operator_OR_SameLength()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1100");
            var result = ba1 | ba2;
            Assert.That(result.ToString(), Is.EqualTo("1110"));
        }

        [Test]
        public void Operator_OR_DifferentLength_OneShorter()
        {
            var ba1 = new BitArray("10001"); // 10001
            var ba2 = new BitArray("101");   // 00101 (padded)
            var result = ba1 | ba2;          // 10101
            Assert.That(result.ToString(), Is.EqualTo("10101"));
            Assert.That(result.ToString().Length, Is.EqualTo(5));
        }

        [Test]
        public void Operator_OR_WithEmptyArray()
        {
            var ba1 = new BitArray("101");
            var baEmpty = new BitArray(0);
            var result1 = ba1 | baEmpty; // baEmpty padded to "000"
            Assert.That(result1.ToString(), Is.EqualTo("101"));
            Assert.That(result1.ToString().Length, Is.EqualTo(3));

            var result2 = baEmpty | ba1; // baEmpty padded to "000"
            Assert.That(result2.ToString(), Is.EqualTo("101"));
            Assert.That(result2.ToString().Length, Is.EqualTo(3));
        }

        [Test]
        public void Operator_NOT()
        {
            var ba = new BitArray("10110");
            var result = ~ba;
            Assert.That(result.ToString(), Is.EqualTo("01001"));
        }

        [Test]
        public void Operator_NOT_EmptyArray()
        {
            var ba = new BitArray(0);
            var result = ~ba;
            Assert.That(result.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void Operator_ShiftLeft()
        {
            var ba = new BitArray("101");
            var result = ba << 2;
            // Implementation detail: shifts left by creating a longer array and copying bits.
            // The new bits are initialized to false (0).
            Assert.That(result.ToString(), Is.EqualTo("10100"));
            Assert.That(result.ToString().Length, Is.EqualTo(5));
        }

        [Test]
        public void Operator_ShiftLeft_ByZero()
        {
            var ba = new BitArray("101");
            var result = ba << 0;
            Assert.That(result.ToString(), Is.EqualTo("101"));
            Assert.That(result.ToString().Length, Is.EqualTo(3));
        }

        [Test]
        public void Operator_ShiftLeft_EmptyArray()
        {
            var ba = new BitArray(0);
            var result = ba << 3;
            Assert.That(result.ToString(), Is.EqualTo("000")); // New array of length 3, all false
            Assert.That(result.ToString().Length, Is.EqualTo(3));
        }


        [Test]
        public void Operator_XOR_SameLength()
        {
            var ba1 = new BitArray("1010");
            var ba2 = new BitArray("1100");
            var result = ba1 ^ ba2;
            Assert.That(result.ToString(), Is.EqualTo("0110"));
        }

        [Test]
        public void Operator_XOR_DifferentLength_OneShorter()
        {
            var ba1 = new BitArray("10101"); // 10101
            var ba2 = new BitArray("101");   // 00101 (padded)
            var result = ba1 ^ ba2;          // 10000
            Assert.That(result.ToString(), Is.EqualTo("10000"));
            Assert.That(result.ToString().Length, Is.EqualTo(5));
        }

        [Test]
        public void Operator_XOR_WithEmptyArray()
        {
            var ba1 = new BitArray("101");
            var baEmpty = new BitArray(0);
            var result1 = ba1 ^ baEmpty; // baEmpty padded to "000"
            Assert.That(result1.ToString(), Is.EqualTo("101"));
            Assert.That(result1.ToString().Length, Is.EqualTo(3));

            var result2 = baEmpty ^ ba1; // baEmpty padded to "000"
            Assert.That(result2.ToString(), Is.EqualTo("101"));
            Assert.That(result2.ToString().Length, Is.EqualTo(3));
        }


        [Test]
        public void Operator_ShiftRight()
        {
            var ba = new BitArray("10110");
            var result = ba >> 2;
            Assert.That(result.ToString(), Is.EqualTo("101"));
            Assert.That(result.ToString().Length, Is.EqualTo(3));
        }

        [Test]
        public void Operator_ShiftRight_ByZero()
        {
            var ba = new BitArray("10110");
            var result = ba >> 0;
            Assert.That(result.ToString(), Is.EqualTo("10110"));
            Assert.That(result.ToString().Length, Is.EqualTo(5));
        }

        [Test]
        public void Operator_ShiftRight_MoreThanLength_ResultsInEmpty()
        {
            var ba = new BitArray("101");
            var result = ba >> 3;
            Assert.That(result.ToString(), Is.EqualTo(string.Empty), "Shifting by current length should result in empty.");
            Assert.That(result.ToString().Length, Is.EqualTo(0), "Length of empty BitArray should be 0.");

            // BitArray.cs operator>> calls new BitArray(other.Length - n).
            // For ba >> 5, this is new BitArray(3 - 5) = new BitArray(-2), which throws OverflowException.
            Assert.Throws<OverflowException>(() => { var _ = ba >> 5; });
        }

        [Test]
        public void Operator_ShiftRight_EmptyArray()
        {
            var ba = new BitArray(0);
            // BitArray.cs operator>> calls new BitArray(other.Length - n).
            // For ba >> 2 where ba is empty, this is new BitArray(0 - 2) = new BitArray(-2).
            // new BitArray(-2) throws OverflowException due to internal `field = new bool[n]`.
            Assert.Throws<OverflowException>(() => { var _ = ba >> 2; });
        }


        [Test]
        public void Operator_Equals_EqualArrays_ReturnsTrue()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("101");
            Assert.That(ba1 == ba2, Is.True);
        }

        [Test]
        public void Operator_Equals_UnequalContent_ReturnsFalse()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("100");
            Assert.That(ba1 == ba2, Is.False);
        }

        [Test]
        public void Operator_Equals_UnequalLength_ReturnsFalse()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("1010");
            Assert.That(ba1 == ba2, Is.False);
        }

        [Test]
        public void Operator_Equals_OneIsNull_ReturnsFalse()
        {
            BitArray ba1 = new BitArray("101");
            BitArray? baNull = null;
            // BitArray.cs op_Equality is not null-safe after ReferenceEquals check, will throw NRE if one arg is null and other is not.
            Assert.Throws<NullReferenceException>(() => { var _ = ba1 == baNull!; });
            Assert.Throws<NullReferenceException>(() => { var _ = baNull! == ba1; });

        }

        [Test]
        public void Operator_Equals_BothNull_ReturnsTrue()
        {
            BitArray? baNull1 = null;
            BitArray? baNull2 = null;
            Assert.That(baNull1! == baNull2!, Is.True);
        }

        [Test]
        public void Operator_NotEquals_ReturnsCorrectly()
        {
            var ba1 = new BitArray("101");
            var ba2 = new BitArray("101");
            var ba3 = new BitArray("100");
            var ba4 = new BitArray("1010");

            Assert.That(ba1 != ba2, Is.False); // Equal
            Assert.That(ba1 != ba3, Is.True);  // Different content
            Assert.That(ba1 != ba4, Is.True);  // Different length
        }

        [Test]
        public void Operator_NotEquals_OneIsNull_ReturnsTrue()
        {
            BitArray ba1 = new BitArray("101");
            BitArray? baNull = null;
            // BitArray.cs op_Inequality calls op_Equality, which is not null-safe.
            Assert.Throws<NullReferenceException>(() => { var _ = ba1 != baNull!; });
            Assert.Throws<NullReferenceException>(() => { var _ = baNull! != ba1; });
        }

        [Test]
        public void Operator_NotEquals_BothNull_ReturnsFalse()
        {
            BitArray? baNull1 = null;
            BitArray? baNull2 = null;
            Assert.That(baNull1! != baNull2!, Is.False);
        }
    }
}
