using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    
    [Test, Order(1)]
    public void TreesEquivalentTrueTest()
    {
        var tree1 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(1), new ArithmeticExpTreeNode(2));
        tree1.OpType = OperatorType.Multiply;
        
        var tree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(2), new ArithmeticExpTreeNode(1));
        tree1.OpType = OperatorType.Multiply;
        
        Assert.True(tree1.EquivalentTo(tree2));
    }
    
    [Test, Order(1)]
    public void TreesEquivalentFalseTest()
    {
        var tree1 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(1), new ArithmeticExpTreeNode(2));
        tree1.OpType = OperatorType.Subtract;
        
        var tree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(2), new ArithmeticExpTreeNode(1));
        tree1.OpType = OperatorType.Subtract;
        
        Assert.False(tree1.EquivalentTo(tree2));
    }
    
    [Test, Order(1)]
    public void TreesEquivalentSimpleTest()
    {
        var subtree1 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(100), new ArithmeticExpTreeNode(3));
        subtree1.OpType = OperatorType.Subtract;
        var tree1 = new ArithmeticExpTreeNode(subtree1, new ArithmeticExpTreeNode(6));
        tree1.OpType = OperatorType.Multiply;
        
        var subtree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(100), new ArithmeticExpTreeNode(3));
        subtree2.OpType = OperatorType.Subtract;
        var tree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(6), subtree2);
        tree2.OpType = OperatorType.Multiply;
        
        Assert.True(tree1.EquivalentTo(tree2));
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
                          + "Found solution: (((25 × (50 + 7)) - 1) ÷ (6 ÷ 3))\r\n"
                          + "Found solution: ((25 × 3) + ((50 - 1) × (6 + 7)))\r\n"
                          + "Found solution: ((25 × 3) - ((1 - 50) × (6 + 7)))\r\n"
                          + "Found 5 solutions in 33,802,560 attempts\r\n";

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