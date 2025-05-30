# Few-Shot Prompt sessions.

*NOTES:*
This used example of "how to complete the task" according to few-shot practices in the prompt pattern area. The prompt used two classes (Welfordsvariance, BellmanFord) by adding references to the classes and
the prompt also contain an answer to both of the prompt example which is nothing more than the baseline written unit-tests for the classes in the baseline project.

For readability purposes the whole code are not included in this file but accessible through their respectice class and test files in the baseline project.

## RedBlackTree
**PROMPT:**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class RedBlackTree.cs
Answer:
**GEMINI:**
-code- compiled and ran w.o. errors

## BitArray 
**PROMPT:**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class BitArray.cs
Answer:
**GEMINI:**
-code- all code but an unnecessary helper function compiled. I removed the helper and everything compiled but 4 test cases failed.

**PROMPT:**
The following tests fail. Fix these. 
Test: Enumerator_Current_BeforeMoveNext_Or_AfterEnd_ThrowsIndexOutOfRangeException  
Source: BitArrayTests.cs line 539 
Message:  Expected a <System.IndexOutOfRangeException> to be thrown, but no exception was thrown.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message) DelegateAssertionsBase2.ThrowInternal[TException](Exception exception, String because, Object[] becauseArgs) DelegateAssertions2.Throw[TException](String because, Object[] becauseArgs) EnumeratorTests.Enumerator_Current_BeforeMoveNext_Or_AfterEnd_ThrowsIndexOutOfRangeException() line 552 
Test: Parity_ReturnsCorrectBoolean("101",False,True)  
Source: BitArrayTests.cs line 246 
Message:  Expected bitArray.EvenParity() to be False, but found True.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message, Object[] args) BooleanAssertions`1.Be(Boolean expected, String because, Object[] becauseArgs) ParityTests.Parity_ReturnsCorrectBoolean(String s, Boolean evenParity, Boolean oddParity) line 249
Test:Parity_ReturnsCorrectBoolean("1011",True,False)  
Source: BitArrayTests.cs line 246 
Message:  Expected bitArray.EvenParity() to be True, but found False.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message, Object[] args) BooleanAssertions1.Be(Boolean expected, String because, Object[] becauseArgs) ParityTests.Parity_ReturnsCorrectBoolean(String s, Boolean evenParity, Boolean oddParity) line 249 InvokeStub_ParityTests.Parity_ReturnsCorrectBoolean(Object, Span1)
**GEMINI:**
suggested changes to the source code

**PROMPT:**
Dont change my source file code, fix the tests
**GEMINI:**
Denna gången gav gemini bara diffar mot tidigare skickad kod istället för helt nya stycken. MED ändringarna så funkar alla test dock.

## ScapegoatTree
**PROMPT:**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class ScapegoatTree.cs
Answer:
**GEMINI:**
-kod- den kompilerade utan probs första försöket, men två test failar.

**PROMPT:**
The following tests fail. Fix these. 
Test: Delete_NodeWithOneLeftChild_ShouldPromoteChild  
Source: ScapegoatTreeTests.cs line 196 
Message:  Expected tree.Root!.Key to be 10, but found 3 (difference of -7).
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) 
Test: Delete_NodeWithOneRightChild_ShouldPromoteChild  
Source: ScapegoatTreeTests.cs line 210 
Message:  Expected tree.Root!.Key to be 10, but found 7 (difference of -3).
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message, Object[] args) NumericAssertions`2.Be(T expected, String because, Object[] becauseArgs) ScapegoatTreeTests.Delete_NodeWithOneRightChild_ShouldPromoteChild() line 218
DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message, Object[] args) NumericAssertions`2.Be(T expected, String because, Object[] becauseArgs) ScapegoatTreeTests.Delete_NodeWithOneLeftChild_ShouldPromoteChild() line 204
**GEMINI:**
Gav “Diff” med ändringar som fixade failsen

## Timeline
**PROMPT:**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class ScapegoatTree.cs
Answer:
**GEMINI:**
-kod- som hade lite null tester som jag suppressade för testsyften. 3 tester som failade.

**PROMPT:**
Reprompt1
**GEMINI:**
-kod- ny source code.

**PROMPT:**
Dont change my source file code, fix the tests
**GEMINI:**
Diff förslag i testerna - manuellt jobb dock då den parsade halvt?

**PROMPT:**
The code is not compiling. Fix this Function:
public void OperatorNotEquals_ShouldBehaveCorrectly() 
{ 
    var time1 = new DateTime(2023, 1, 1); 
    var time2 = new DateTime(2023, 1, 2);
    ...
   var tl1 = new Timeline&lt;string&gt;((time1, "A"), (time2, "B"));
    var tl2 = new Timeline&lt;string&gt;((time1, "A"), (time2, "B"));
    var tl3 = new Timeline&lt;string&gt;((time1, "A"), (time2, "C"));

    (tl1 != tl2).Should().BeFalse();
    // Adjusted for Timeline.cs operator== behavior: (tl1 == tl3) is true, so (tl1 != tl3) is false.
    +(tl1 != tl3).Should().BeFalse();
}
Description: Operator '+' cannot be applied to operand of type 'AndConstraint<BooleanAssertions>' & Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement
**GEMINI:**
-code- function compiles but test still fails.
*NOTES:* Kommenterade ut.



## FibonacciHeap
**PROMPT:**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class FibonacciHeap.cs
Answer:
**GEMINI:**
-kod- compiles but 2 test fail during RT.

**PROMPT:**
The following tests fail. Fix these. 
Test: DecreaseKey_CascadingCut_MaintainsHeapProperty  
Source: FibonacciHeapTests.cs line 185 
Message:  Expected sorted to be equal to {0, 1, 2, 3, 4, 5, 6, 7, 9, 10}, but {0, 5, 6, 7, 9, 10, 20, 30, 40} contains 1 item(s) less.
Stack Trace:  LateBoundTestFramework.Throw(String message)
Test: DecreaseKey_NodeNotInHeap_ThrowsArgumentException  
Source: FibonacciHeapTests.cs line 171 
Message:  Expected a <System.ArgumentException> to be thrown, but no exception was thrown.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message) DelegateAssertionsBase2.ThrowInternal[TException](Exception exception, String because, Object[] becauseArgs) DelegateAssertions2.Throw[TException](String because, Object[] becauseArgs) FibonacciHeapTests.DecreaseKey_NodeNotInHeap_ThrowsArgumentException() line 181
TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message, Object[] args) GivenSelector1.FailWith(String message, Object[] args) GivenSelector1.FailWith(String message, Func2[] args) GivenSelectorExtensions.AssertCollectionHasEnoughItems[T](GivenSelector1 givenSelector, Int32 length) GivenSelectorExtensions.AssertCollectionsHaveSameCount[T](GivenSelector1 givenSelector, Int32 length) GenericCollectionAssertions3.AssertSubjectEquality[TExpectation](IEnumerable1 expectation, Func3 equalityComparison, String because, Object[] becauseArgs) GenericCollectionAssertions3.Equal(IEnumerable1 expected, String because, Object[] becauseArgs) FibonacciHeapTests.DecreaseKey_CascadingCut_MaintainsHeapProperty() line 237
**GEMINI:**
-kod- koden ändrade på source

**PROMPT**
Dont change my source file code, fix the tests
**GEMINI:**
Ny kod med diff för att visa, manuel process men lösningarna funkade.



## DirectedWeightedGraph
**PROMPT**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class FibonacciHeap.cs
Answer:
**GEMINI:**
-kod- compiles and runs w.o errors


## RGBHSVConversion
**PROMPT**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class RGBHSVConversion.cs
Answer:
**GEMINI:**
-kod- compiles and runs w.o errors



## Kruskal
**PROMPT**
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref to Welfordsvariance}
Answer: {all code from WelfordsVarianceTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class {ref toBellmanFord}
Answer: {all code from BellmanFordTest.cs}
Prompt: Using NUnit framework version 4.0.1, generate unit tests for the class Kruskal.cs
Answer:
**GEMINI:**
-kod- compiles and runs w.o errors



## Mandelbrot
**PROMPT**
Prompt:
**GEMINI:**
-kod- Det var nästan felfritt men den använde “SKBitmap?” som den varnade för att det kunde vara null så jag satte “?” för att den kan vara null och det var fine. Det var dock två till null refs som var bökiga.

**PROMPT**
The code is not compiling. Fix this Function: 
public void GetBitmap_InvalidParameters_ThrowsArgumentOutOfRangeException(int width, string expectedParamNameForWidth, int height = 100, string expectedParamNameForHeight = null, int maxStep = 50, string expectedParamNameForMaxStep = null) 
{ 
    var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(width, height, maxStep: maxStep)); 
    Assert.That(ex.ParamName, Is.EqualTo(expectedParamNameForMaxStep ?? expectedParamNameForHeight ?? expectedParamNameForWidth));
} 
Description: Severity Code Description Project File Line Suppression State Details Error (active) CS8625 Cannot convert null literal to non-nullable reference type. Algorithms.Tests .\FewShot\C-Sharp-master\C-Sharp-master\Algorithms.Tests\Other\MandelbrotTest.cs 101
**GEMINI:**
-kod- som fixade ett av problemen. 

**PROMPT** - reprompt2
The code is not compiling. Fix this Function: 
public void GetBitmap_InvalidParameters_ThrowsArgumentOutOfRangeException(int width, string expectedParamNameForWidth, int height = 100, string? expectedParamNameForHeight = null, int maxStep = 50, string? expectedParamNameForMaxStep = null) 
{ 
    var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Mandelbrot.GetBitmap(width, height, maxStep: maxStep)); 
    Assert.That(ex.ParamName, Is.EqualTo(expectedParamNameForMaxStep ?? expectedParamNameForHeight ?? expectedParamNameForWidth)); 
} 
Description: Dereference of a possibly null reference.
**GEMINI:**
-kod- som inte löste problemet (null ref problem)

**PROMPT**
Reprompt3
**GEMINI:**
 failed still.



## DynamicCoinChangeSolver
**PROMPT**
Prompt
**GEMINI:**
-kod- koden kompilerade men 5 test case failar

**PROMPT**
The following tests fail. Fix these. 
Test:GenerateSingleCoinChanges_CoinMatchesADenomination_IncludesZeroChange  
Source: DynamicCoinChangeSolverTest.cs line 31 
Message:  Expected result[0] to be 9, but found 0. Expected result[2] to be 0, but found 9.
With configuration:
Use declared types and members
Compare enums by value
Compare tuples by their properties
Compare anonymous types by their properties
Compare records by their members
Include non-browsable members
Include all non-private properties
Include all non-private fields
Match member by name (or throw)
Always be strict about the collection order
Without automatic conversion.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) CollectingAssertionStrategy.ThrowIfAny(IDictionary2 context) AssertionScope.Dispose() EquivalencyValidator.AssertEquality(Comparands comparands, EquivalencyValidationContext context) GenericCollectionAssertions3.BeEquivalentTo[TExpectation](IEnumerable1 expectation, Func2 config, String because, Object[] becauseArgs) DynamicCoinChangeSolverTests.GenerateSingleCoinChanges_CoinMatchesADenomination_IncludesZeroChange() line 35
Test: GenerateSingleCoinChanges_UnsortedCoins_ReturnsCorrectChanges  
Source: DynamicCoinChangeSolverTest.cs line 39 
Message:  Expected result[0] to be 5, but found 2. Expected result[2] to be 2, but found 5.
With configuration:
Use declared types and members
Compare enums by value
Compare tuples by their properties
Compare anonymous types by their properties
Compare records by their members
Include non-browsable members
Include all non-private properties
Include all non-private fields
Match member by name (or throw)
Always be strict about the collection order
Without automatic conversion.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) CollectingAssertionStrategy.ThrowIfAny(IDictionary2 context) AssertionScope.Dispose() EquivalencyValidator.AssertEquality(Comparands comparands, EquivalencyValidationContext context) GenericCollectionAssertions3.BeEquivalentTo[TExpectation](IEnumerable1 expectation, Func2 config, String because, Object[] becauseArgs) DynamicCoinChangeSolverTests.GenerateSingleCoinChanges_UnsortedCoins_ReturnsCorrectChanges() line 43
Test: GenerateSingleCoinChanges_ValidInput_ReturnsCorrectChanges  
Source: DynamicCoinChangeSolverTest.cs line 15 
Message:  Expected result[0] to be 5, but found 2. Expected result[2] to be 2, but found 5.
With configuration:
Use declared types and members
Compare enums by value
Compare tuples by their properties
Compare anonymous types by their properties
Compare records by their members
Include non-browsable members
Include all non-private properties
Include all non-private fields
Match member by name (or throw)
Always be strict about the collection order
Without automatic conversion.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) CollectingAssertionStrategy.ThrowIfAny(IDictionary2 context) AssertionScope.Dispose() EquivalencyValidator.AssertEquality(Comparands comparands, EquivalencyValidationContext context) GenericCollectionAssertions3.BeEquivalentTo[TExpectation](IEnumerable1 expectation, Func2 config, String because, Object[] becauseArgs) DynamicCoinChangeSolverTests.GenerateSingleCoinChanges_ValidInput_ReturnsCorrectChanges() line 19
Test: MakeCoinChangeDynamic_CoinNotPositive_ThrowsInvalidOperationException  Source: DynamicCoinChangeSolverTest.cs line 254 Message:  Expected a <System.InvalidOperationException> to be thrown, but found <System.Collections.Generic.KeyNotFoundException>: System.Collections.Generic.KeyNotFoundException: The given key '0' was not present in the dictionary. at System.Collections.Generic.Dictionary2.get_Item(TKey key) at Algorithms.Problems.DynamicProgramming.CoinChange.DynamicCoinChangeSolver.GetMinimalNextCoin(Int32 coin, Dictionary2 exchanges) in .\FewShot\C-Sharp-master\C-Sharp-master\Algorithms\Problems\DynamicProgramming\CoinChange\DynamicCoinChangeSolver.cs:line 83 at Algorithms.Problems.DynamicProgramming.CoinChange.DynamicCoinChangeSolver.MakeCoinChangeDynamic(Int32 coin, Int32[] coins) in .\FewShot\C-Sharp-master\C-Sharp-master\Algorithms\Problems\DynamicProgramming\CoinChange\DynamicCoinChangeSolver.cs:line 123 at Algorithms.Tests.Problems.DynamicProgramming.CoinChange.DynamicCoinChangeSolverTests.<>c.<MakeCoinChangeDynamic_CoinNotPositive_ThrowsInvalidOperationException>b__21_0() in .\FewShot\C-Sharp-master\C-Sharp-master\Algorithms.Tests\Problems\DynamicProgramming\DynamicCoinChangeSolverTest.cs:line 256 at FluentAssertions.Specialized.ActionAssertions.InvokeSubject() at FluentAssertions.Specialized.DelegateAssertions`2.InvokeSubjectWithInterception().
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) DefaultAssertionStrategy.HandleFailure(String message) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(Func1 failReasonFunc) AssertionScope.FailWith(String message, Object[] args) DelegateAssertionsBase2.ThrowInternal[TException](Exception exception, String because, Object[] becauseArgs) DelegateAssertions2.Throw[TException](String because, Object[] becauseArgs) DynamicCoinChangeSolverTests.MakeCoinChangeDynamic_CoinNotPositive_ThrowsInvalidOperationException() line 257
Test: MakeCoinChangeDynamic_GreedyFails_DynamicSucceeds  Source: DynamicCoinChangeSolverTest.cs line 220 Message:  Expected result to be a collection with 3 item(s), but {25, 25, 10, 1, 1, 1} contains 3 item(s) more than {21, 21, 21}.
With configuration:
Use declared types and members
Compare enums by value
Compare tuples by their properties
Compare anonymous types by their properties
Compare records by their members
Include non-browsable members
Include all non-private properties
Include all non-private fields
Match member by name (or throw)
Always be strict about the collection order
Without automatic conversion.
Stack Trace:  LateBoundTestFramework.Throw(String message) TestFrameworkProvider.Throw(String message) CollectingAssertionStrategy.ThrowIfAny(IDictionary2 context) AssertionScope.Dispose() EquivalencyValidator.AssertEquality(Comparands comparands, EquivalencyValidationContext context) GenericCollectionAssertions3.BeEquivalentTo[TExpectation](IEnumerable1 expectation, Func2 config, String because, Object[] becauseArgs) DynamicCoinChangeSolverTests.MakeCoinChangeDynamic_GreedyFails_DynamicSucceeds() line 231
**GEMINI:**
-kod- suggests changes to source code.

**PROMPT**
Dont change my source file code, fix the tests
**GEMINI:**
-kod- compiles and runs w.o errors


## Kochsnowflake
**PROMPT**
prompt
**GEMINI:**
-kod- compiles and runs w.o errors


# Notes on qualitative metrics

## Lines of comments

KruskalTest.cs' has 41 comments.
KochSnowflakeTest.cs' has 12 comments.
MandelbrotTest.cs' has 11 comments.
RGBHSVConversionTest.cs' has 4 comments.
DynamicCoinChangeSolverTest.cs' has 50 comments.
BitArrayTest.cs' has 45 comments.
TimelineTest.cs' has 22 comments.
DirectedWeightedGraphTest.cs' has 8 comments.
FibonacciHeapTest.cs' has 44 comments.
RedBlackTreeTest.cs' has 23 comments.
ScapegoatTreeTest.cs' has 79 comments.

Total number of comments = 339
Average comments/class = 339/11 = 31

## Concering readability and maintainability

Sometime the number of Asserts per unit testing function is very high. At max, 30 assertions in a single function. Scanning for functions that contain more or equal than 8 provides the following result.


Baseline tester för kruskal har halvcoola grafiska representationer av grafer i komments, det har inte GEMINI.









