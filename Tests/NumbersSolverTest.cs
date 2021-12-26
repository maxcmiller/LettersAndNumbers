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
                          + "Solved after 273,009 attempts." + Environment.NewLine
                          + "(10 × (5 × ((1 + 4) × 2)))" + Environment.NewLine;

        AssertSolvesTo(input, expected);
    }

    [Test]
    public void DuplicateNumbersTest()
    {
        int target = 593;
        int[] numbers = {50, 100, 3, 1, 1, 8};
        string input = GenerateInput(target, numbers);
        string expected = GenerateOutput(target, numbers)
                          + "Solved after 2,842,327 attempts." + Environment.NewLine
                          + "(((50 + 100) × (3 + 1)) + (1 - 8))" + Environment.NewLine;
        
        AssertSolvesTo(input, expected);
    }

    [Test]
    public void MultipleSolutionsTest()
    {
        int target = 712;
        int[] numbers = {25, 50, 3, 1, 6, 7};
        string input = GenerateInput(target, numbers, NumbersSolver.SolveMode.All);
        string expected = GenerateOutput(target, numbers)
                          + "Found solution: (((50 × 7) + 6) × (3 - 1))" + Environment.NewLine
                          + "Found solution: ((((25 × (50 + 7)) - 1) × 3) ÷ 6)" + Environment.NewLine
                          // + "Found solution: (((25 × (50 + 7)) - 1) ÷ (6 ÷ 3))" + Environment.NewLine // TODO should this be considered a distinct solution?
                          + "Found solution: ((25 × 3) - ((1 - 50) × (6 + 7)))" + Environment.NewLine
                          + "Found 3 solutions in 33,802,560 attempts" + Environment.NewLine;

        AssertSolvesTo(input, expected);
    }
    
    [Test]
    public void MultipleSolutionsOnlyOneFoundTest()
    {
        // from Countdown 2012-06-04
        int target = 507;
        int[] numbers = {25, 100, 6, 10, 4, 5};
        string input = GenerateInput(target, numbers, NumbersSolver.SolveMode.All);
        string expected = GenerateOutput(target, numbers)
                          + "Found solution: (25 + (((100 × (6 × 4)) + 10) ÷ 5))" + Environment.NewLine
                          + "Found 1 solution in 33,802,560 attempts" + Environment.NewLine;

        AssertSolvesTo(input, expected);
    }

    [Test]
    public void MostIntuitiveTest()
    {
        int target = 719;
        int[] numbers = {50, 100, 2, 7, 4, 3};
        string input = GenerateInput(target, numbers, NumbersSolver.SolveMode.MostIntuitive);
        string expected = GenerateOutput(target, numbers)
                          + "Found solution: (((100 + 3) × 7) - 2) [intuition score: 120]" + Environment.NewLine
                          + "Found solution: (50 + (((100 - 4) × 7) - 3)) [intuition score: 160]" + Environment.NewLine
                          + "Found solution: (50 - ((7 × (4 - 100)) + 3)) [intuition score: 160]" + Environment.NewLine
                          + "Found solution: (((100 + 3) × 7) + (2 - 4)) [intuition score: 150]" + Environment.NewLine
                          + "Found solution: (((100 + 3) × 7) - (4 ÷ 2)) [intuition score: 180]" + Environment.NewLine
                          + "Found solution: (50 + (((100 - (2 + 3)) × 7) + 4)) [intuition score: 180]" + Environment.NewLine
                          + "Found solution: (((((50 × 100) + 2) + 3) ÷ 7) + 4) [intuition score: 200]" + Environment.NewLine
                          + "Found solution: (((50 + (100 - 7)) × (2 + 3)) + 4) [intuition score: 180]" + Environment.NewLine
                          + "Found solution: (((50 + ((100 - 4) × 2)) × 3) - 7) [intuition score: 200]" + Environment.NewLine
                          + "Found solution: ((50 - (((2 - 100) + 3) × 7)) + 4) [intuition score: 190]" + Environment.NewLine
                          + "Found solution: (((50 - (2 × (4 - 100))) × 3) - 7) [intuition score: 210]" + Environment.NewLine
                          + "Found solution: ((((50 × 7) - 3) × 2) + (100 ÷ 4)) [intuition score: 220]" + Environment.NewLine
                          + "Found solution: (((((50 × 4) - 100) + 3) × 7) - 2) [intuition score: 200]" + Environment.NewLine
                          + "Found solution: ((((100 × 2) - 7) × 4) - (50 + 3)) [intuition score: 200]" + Environment.NewLine
                          + "Found solution: ((100 ÷ 4) - (2 × (3 - (50 × 7)))) [intuition score: 230]" + Environment.NewLine
                          + "Most intuitive solution: (((100 + 3) × 7) - 2)" + Environment.NewLine;

        AssertSolvesTo(input, expected);
    }

    [Test]
    public void NoExactSolutionsTest()
    {
        int target = 824;
        int[] numbers = {3, 7, 6, 2, 1, 7};
        string input = GenerateInput(target, numbers);
        string expected = GenerateOutput(target, numbers)
                          + "No solution after 33,802,560 attempts" + Environment.NewLine;
        
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

    private string GenerateInput(int target, int[] numbers, NumbersSolver.SolveMode? solveMode = null)
    {
        if (solveMode == null)
        {
            solveMode = NumbersSolver.SolveMode.First;
        }
        
        return $"{target}\n{string.Join("\n", numbers)}\n{solveMode.ResponseCode}\n";
    }

    private string GenerateOutput(int target, int[] numbers)
    {
        return "Enter target number (0-999): " +
               $"Target is {target}." + Environment.NewLine +
               "Enter chosen number #1: " +
               "Enter chosen number #2: " +
               "Enter chosen number #3: " +
               "Enter chosen number #4: " +
               "Enter chosen number #5: " +
               "Enter chosen number #6: " +
               $"Numbers are {String.Join(", ", numbers)}." + Environment.NewLine +
               "Solution mode: first/all/intuitive (f/a/i): ";
    }
}