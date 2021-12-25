using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using LettersAndNumbers;

namespace Tests;

public class Tests
{
    private NumbersSolver? _solver;
    
    [SetUp]
    public void Setup()
    {
        _solver = new NumbersSolver();
    }

    [Test]
    public void BasicTest()
    {
        int target = 500;
        int[] numbers = {10, 5, 1, 2, 3, 4};
        string input = GenerateInput(target, numbers);
        string expected = GenerateOutput(target, numbers)
                          + "Solved after 270,685 attempts.\r\n"
                          + "(10 × (5 × ((1 + 4) × 2)))\r\n";

        AssertSolvesTo(input, expected);
    }

    [Test]
    public void DuplicateNumbersTest()
    {
        int target = 593;
        int[] numbers = {50, 100, 3, 1, 1, 8};
        string input = GenerateInput(target, numbers);
        string expected = GenerateOutput(target, numbers)
                          + "Solved after 2,860,135 attempts.\r\n"
                          + "(((50 + 100) × (3 + 1)) + (1 - 8))\r\n";
        
        AssertSolvesTo(input, expected);
    }

    [Test]
    public void MultipleSolutionsTest()
    {
        int target = 712;
        int[] numbers = {25, 50, 3, 1, 6, 7};
        string input = GenerateInput(target, numbers, true);
        string expected = GenerateOutput(target, numbers)
                          + "Found solution: (((50 × 7) + 6) × (3 - 1))\r\n"
                          + "Found solution: ((((25 × (50 + 7)) - 1) × 3) ÷ 6)\r\n"
                          // + "Found solution: (((25 × (50 + 7)) - 1) ÷ (6 ÷ 3))\r\n" // TODO should this be considered a distinct solution?
                          + "Found solution: ((25 × 3) - ((1 - 50) × (6 + 7)))\r\n"
                          + "Found 3 solutions in 33,802,560 attempts\r\n";

        AssertSolvesTo(input, expected);
    }

    private void AssertSolvesTo(string input, string expectedOutput)
    {
        using (StringWriter sw = new StringWriter())
        {
            using (StringReader sr = new StringReader(input))
            {
                Console.SetIn(sr);
                Console.SetOut(sw);
                
                _solver?.Run();

                Assert.AreEqual(expectedOutput, sw.ToString());
            }
        }
    }

    private string GenerateInput(int target, int[] numbers, bool multipleSols = false)
    {
        var elems = new List<int>(numbers);
        elems.Insert(0, target);
        return $"{string.Join("\n", elems)}\n{(multipleSols ? "y" : "n")}\n";
    }

    private string GenerateOutput(int target, int[] numbers)
    {
        return "Enter target number (0-999): " +
               $"Target is {target}.\r\n" +
               "Enter chosen number #1: " +
               "Enter chosen number #2: " +
               "Enter chosen number #3: " +
               "Enter chosen number #4: " +
               "Enter chosen number #5: " +
               "Enter chosen number #6: " +
               $"Numbers are {String.Join(", ", numbers)}.\r\n" +
               "Try to find multiple solutions? (y/n): ";
    }
}