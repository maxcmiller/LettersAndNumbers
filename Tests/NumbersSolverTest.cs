using System;
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
                          + "Solved after 258,280 attempts." + Environment.NewLine
                          + "(10 × (5 × (2 × (1 + 4))))" + Environment.NewLine;

        AssertSolvesTo(input, expected);
    }

    [Test]
    public void DuplicateNumbersTest()
    {
        int target = 593;
        int[] numbers = {50, 100, 3, 1, 1, 8};
        string input = GenerateInput(target, numbers);
        string expected = GenerateOutput(target, numbers)
                          + "Solved after 4,779,296 attempts." + Environment.NewLine
                          + "(1 - (8 - ((50 + 100) × (3 + 1))))" + Environment.NewLine;
        
        AssertSolvesTo(input, expected);
    }

    [Test]
    public void MultipleSolutionsTest()
    {
        int target = 712;
        int[] numbers = {25, 50, 3, 1, 6, 7};
        string input = GenerateInput(target, numbers, NumbersSolver.SolveMode.All);
        string expected = GenerateOutput(target, numbers)
                          + "Found solution: ((3 - 1) × (6 + (50 × 7)))" + Environment.NewLine
                          + "Found solution: ((25 × 3) - ((1 - 50) × (6 + 7)))" + Environment.NewLine
                          // + "Found solution: ((((25 × (50 + 7)) - 1) × 3) ÷ 6)" + Environment.NewLine // TODO should this be considered a distinct solution?
                          + "Found solution: (((25 × (50 + 7)) - 1) ÷ (6 ÷ 3))" + Environment.NewLine
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
                          + "Found solution: (25 + ((10 + (100 × (6 × 4))) ÷ 5))" + Environment.NewLine
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
                          + "Found solution: ((7 × (100 + 3)) - 2) [intuition score: 120]" + Environment.NewLine +
                          "Found solution: (50 - (3 - (7 × (100 - 4)))) [intuition score: 170]" + Environment.NewLine +
                          "Found solution: (50 - (3 + (7 × (4 - 100)))) [intuition score: 160]" + Environment.NewLine +
                          "Found solution: (2 - (4 - (7 × (100 + 3)))) [intuition score: 160]" + Environment.NewLine +
                          "Found solution: ((7 × (100 + 3)) - (4 ÷ 2)) [intuition score: 180]" + Environment.NewLine +
                          "Found solution: (50 + (4 + (7 × (100 - (2 + 3))))) [intuition score: 180]" + Environment.NewLine +
                          "Found solution: (50 + (4 - (7 × (2 - (100 - 3))))) [intuition score: 200]" + Environment.NewLine +
                          "Found solution: (4 + ((2 + 3) × (50 + (100 - 7)))) [intuition score: 180]" + Environment.NewLine +
                          "Found solution: (4 + ((2 + (3 + (50 × 100))) ÷ 7)) [intuition score: 200]" + Environment.NewLine +
                          "Found solution: ((100 ÷ 4) - (2 × (3 - (50 × 7)))) [intuition score: 230]" + Environment.NewLine +
                          "Found solution: ((100 ÷ 4) + (2 × ((50 × 7) - 3))) [intuition score: 220]" + Environment.NewLine +
                          "Found solution: ((2 × (4 × (100 - 3))) - (50 + 7)) [intuition score: 200]" + Environment.NewLine +
                          "Found solution: ((7 × (3 - (100 - (50 × 4)))) - 2) [intuition score: 210]" + Environment.NewLine +
                          "Found solution: ((3 × (50 + (2 × (100 - 4)))) - 7) [intuition score: 200]" + Environment.NewLine +
                          "Found solution: ((3 × (50 - (2 × (4 - 100)))) - 7) [intuition score: 210]" + Environment.NewLine +
                          "Most intuitive solution: ((7 × (100 + 3)) - 2)" + Environment.NewLine;

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
        using StringWriter sw = new StringWriter();
        using StringReader sr = new StringReader(input);
        
        Console.SetIn(sr);
        Console.SetOut(sw);
                
        _solver?.Run();

        Assert.AreEqual(expectedOutput, sw.ToString());
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