//This project scans through all the prompt pattern projects and count:
//This number of asserts per function
//The lines of comments

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ComparingPrompts
{

    public class AssertCounter
    {

        public static void Main(string[] args)
        {
            // Example: list of files to process
            var files = new List<string>
            {
                ////BASELINE FILES
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Graph/MinimumSpanningTree/KruskalTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/KochSnowflakeTest.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/MandelbrotTest.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/RGBHSVConversionTest.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/CoinChange/GenerateChangesDictionaryTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/CoinChange/GenerateSingleCoinChangesTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/CoinChange/GetMinimalNextCoinTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/CoinChange/MakeCoinChangeDynamicTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/BitArrayTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/TimelineTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Graph/DirectedWeightedGraphTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Heap/FibonacciHeaps/FibonacciHeapTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/RedBlackTreeTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/ScapegoatTree/ExtensionsTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/ScapegoatTree/ScapegoatTreeNodeTests.cs",
                //"./Baseline/C-Sharp-master/C-Sharp-master/DataStructures.Tests/ScapegoatTree/ScapegoatTreeTests.cs"
                

                //ZEROSHOT PROMPT FILES
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Graph/MinimumSpanningTree/KruskalTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/KochSnowflakeTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/MandelbrotTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/RGBHSVConversionTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/CoinChange/DynamicCoinChangeSolverTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/BitArrayTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/TimelineTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Graph/DirectedWeightedGraphTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Heap/FibonacciHeap/FibonacciHeapTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/RedBlackTree/RedBlackTreeTest.cs",
                //"./ZeroShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/ScapegoatTree/ScapegoatTreeTest.cs"
                


                 //FEWSHOT PROMPT FILES
                //"./FewShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Graph/MinimumSpanningTree/KruskalTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/KochSnowflakeTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/MandelbrotTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/RGBHSVConversionTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/DynamicCoinChangeSolverTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/BitArrayTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/TimelineTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Graph/DirectedWeightedGraphTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Heap/FibonacciHeapTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/RedBlackTree/RedBlackTreeTest.cs",
                //"./FewShot/C-Sharp-master/C-Sharp-master/DataStructures.Tests/ScapegoatTree/ScapegoatTreeTest.cs"


                //COT PROMPT FILES
                //"./CoT/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Graph/MinimumSpanningTree/KruskalTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/KochSnowflakeTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/MandelbrotTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/Algorithms.Tests/Other/RGBHSVConversionTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/Algorithms.Tests/problems/DynamicProgramming/DynamicCoinChangeSolverTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/DataStructures.Tests/BitArrayTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/DataStructures.Tests/TimelineTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Graph/DirectedWeightedGraphTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/DataStructures.Tests/Heap/FibonacciHeapTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/DataStructures.Tests/RedBlackTree/RedBlackTreeTest.cs",
                //"./CoT/C-Sharp-master/C-Sharp-master/DataStructures.Tests/ScapegoatTree/ScapegoatTreeTest.cs"
            };

            foreach (var file in files)
            {
                //Console.WriteLine($"Processing file: {file}");
                //ProcessFile(file);
                CountComments(file);
            }
        }

        private static void CountComments(string filePath)
        {
            int commentCount = 0;
            var code = File.ReadAllText(filePath);
            string commentPattern = @"//.*|/\*[\s\S]*?\*/";
            MatchCollection commentMatches = Regex.Matches(code, commentPattern);
            foreach (Match match in commentMatches)
            {
                commentCount++;
            }
            Console.WriteLine($"File '{filePath}' has {commentCount} comments.\r\n");
        }

        private static void ProcessFile(string filePath)
        {
            int numberOfFunctions = 0;
            int numberOfAsserts = 0;
            int averageAsserts = 0;
            var code = File.ReadAllText(filePath);
            string methodPattern = @"(?:(public|private|protected|internal)\s+(?:static\s+)?\S+\s+(\w+)\s*\([^)]*\)\s*\{)";
            MatchCollection methodMatches = Regex.Matches(code, methodPattern);

            List<(string MethodName, int StartIndex)> methods = new();

            // Collect method names and their start indices
            foreach (Match match in methodMatches)
            {
                string methodName = match.Groups[2].Value;
                int startIndex = match.Index;
                methods.Add((methodName, startIndex));
            }

            for (int i = 0; i < methods.Count; i++)
            {
                var (methodName, startIndex) = methods[i];
                int endIndex = (i < methods.Count - 1) ? methods[i + 1].StartIndex : code.Length;
                string methodBlock = ExtractMethodBody(code, startIndex, endIndex);

                //int assertCount = Regex.Matches(methodBlock, @"Assert\.That\s*\(").Count;
                int assertCount = Regex.Matches(methodBlock, @"Assert\.").Count;

                if(assertCount >= 6)
                    Console.WriteLine($"Method '{methodName}' has {assertCount} Assert.That calls. \r\n");
                numberOfAsserts += assertCount;
            }
            numberOfFunctions = methods.Count;
            Console.WriteLine($"File '{filePath}' has {numberOfAsserts/numberOfFunctions} Asserts on average");
        }

        static string ExtractMethodBody(string code, int startIndex, int endSearchLimit)
        {
            int braceCount = 0;
            int endIndex = startIndex;

            for (int i = startIndex; i < endSearchLimit; i++)
            {
                if (code[i] == '{') braceCount++;
                else if (code[i] == '}') braceCount--;

                if (braceCount == 0 && code[i] == '}')
                {
                    endIndex = i + 1;
                    break;
                }
            }

            return code.Substring(startIndex, endIndex - startIndex);
        }
    }
}
