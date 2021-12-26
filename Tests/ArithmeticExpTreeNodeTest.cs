using System.Collections.Generic;
using LettersAndNumbers;
using NUnit.Framework;

namespace Tests;

public class ArithmeticExpTreeNodeTest
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void ToListBasicTest()
    {
        var node1 = new ArithmeticExpTreeNode(1);
        var node2 = new ArithmeticExpTreeNode(2);
        var tree = new ArithmeticExpTreeNode(node1, node2);
        
        Assert.AreEqual(new List<ArithmeticExpTreeNode>() {node1, tree, node2}, tree.ToList());
    }
    
    [Test]
    public void ToListLargeTest()
    {
        var ll = new ArithmeticExpTreeNode(1);
        var lr = new ArithmeticExpTreeNode(2);
        var rr = new ArithmeticExpTreeNode(5);
        var rll = new ArithmeticExpTreeNode(3);
        var rlr = new ArithmeticExpTreeNode(4);
        var rl = new ArithmeticExpTreeNode(rll, rlr);
        var l = new ArithmeticExpTreeNode(ll, lr);
        var r = new ArithmeticExpTreeNode(rl, rr);
        var tree = new ArithmeticExpTreeNode(l, r);
        
        Assert.AreEqual(new List<ArithmeticExpTreeNode>() {ll, l, lr, tree, rll, rl, rlr, r, rr}, tree.ToList());
    }
    
    [Test]
    public void IsEquivalentToTrueTest()
    {
        var tree1 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(1), new ArithmeticExpTreeNode(2));
        tree1.OpType = OperatorType.Multiply;
        
        var tree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(2), new ArithmeticExpTreeNode(1));
        tree2.OpType = OperatorType.Multiply;
        
        Assert.True(tree1.IsEquivalentTo(tree2));
    }
    
    [Test]
    public void IsEquivalentToFalseTest()
    {
        var tree1 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(1), new ArithmeticExpTreeNode(2));
        tree1.OpType = OperatorType.Subtract;
        
        var tree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(2), new ArithmeticExpTreeNode(1));
        tree2.OpType = OperatorType.Subtract;
        
        Assert.False(tree1.IsEquivalentTo(tree2));
    }
    
    [Test]
    public void IsEquivalentToSimpleTest()
    {
        var subtree1 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(100), new ArithmeticExpTreeNode(3));
        subtree1.OpType = OperatorType.Subtract;
        var tree1 = new ArithmeticExpTreeNode(subtree1, new ArithmeticExpTreeNode(6));
        tree1.OpType = OperatorType.Multiply;
        
        var subtree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(100), new ArithmeticExpTreeNode(3));
        subtree2.OpType = OperatorType.Subtract;
        var tree2 = new ArithmeticExpTreeNode(new ArithmeticExpTreeNode(6), subtree2);
        tree2.OpType = OperatorType.Multiply;
        
        Assert.True(tree1.IsEquivalentTo(tree2));
    }

    [Test]
    public void IsEquivalentToDifferentStructureTest()
    {
        var lll1 = new ArithmeticExpTreeNode(100);
        var llr1 = new ArithmeticExpTreeNode(50);
        var lrl1 = new ArithmeticExpTreeNode(5);
        var lrr1 = new ArithmeticExpTreeNode(3);
        var r1 = new ArithmeticExpTreeNode(6);
        var ll1 = new ArithmeticExpTreeNode(lll1, llr1)
        {
            OpType = OperatorType.Multiply
        };
        var lr1 = new ArithmeticExpTreeNode(lrl1, lrr1)
        {
            OpType = OperatorType.Add
        };
        var l1 = new ArithmeticExpTreeNode(ll1, lr1)
        {
            OpType = OperatorType.Subtract
        };
        var tree1 = new ArithmeticExpTreeNode(l1, r1)
        {
            OpType = OperatorType.Divide
        };
        
        var llll2 = new ArithmeticExpTreeNode(100);
        var lllr2 = new ArithmeticExpTreeNode(50);
        var llr2 = new ArithmeticExpTreeNode(5);
        var lr2 = new ArithmeticExpTreeNode(3);
        var r2 = new ArithmeticExpTreeNode(6);
        var lll2 = new ArithmeticExpTreeNode(llll2, lllr2)
        {
            OpType = OperatorType.Multiply
        };
        var ll2 = new ArithmeticExpTreeNode(lll2, llr2)
        {
            OpType = OperatorType.Subtract
        };
        var l2 = new ArithmeticExpTreeNode(ll2, lr2)
        {
            OpType = OperatorType.Subtract
        };
        var tree2 = new ArithmeticExpTreeNode(l2, r2)
        {
            OpType = OperatorType.Divide
        };
        
        Assert.True(tree1.IsEquivalentTo(tree2));
    }
}