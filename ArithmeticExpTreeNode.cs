using System;
using System.Text;

namespace LettersAndNumbers
{
    public enum OperatorType
    {
        Multiply,
        Divide,
        Add,
        Subtract
    }
    
    public class ArithmeticExpTreeNode
    {
        public OperatorType OpType { get; set; }
        private int Number { get; }
        public ArithmeticExpTreeNode Left { get; set; }
        public ArithmeticExpTreeNode Right { get; set; }

        public ArithmeticExpTreeNode(ArithmeticExpTreeNode copy)
        {
            OpType = copy.OpType;
            Number = copy.Number;
            if (copy.Left != null) Left = new ArithmeticExpTreeNode(copy.Left);
            if (copy.Right != null) Right = new ArithmeticExpTreeNode(copy.Right);
        }

        public ArithmeticExpTreeNode(int number)
        {
            Number = number;
        }

        public ArithmeticExpTreeNode(ArithmeticExpTreeNode left, ArithmeticExpTreeNode right)
        {
            Left = left;
            Right = right;
        }

        private string OperatorTypeString(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.Multiply:
                    return "×";
                case OperatorType.Divide:
                    return "÷";
                case OperatorType.Add:
                    return "+";
                default:
                    return "-";
            }
        }

        public int Evaluate()
        {
            if (Left == null && Right == null)
            {
                // leaf node containing a number, simply return the number
                return Number;
            }
            
            // internal node, evaluate the subtrees and apply this node's operator
            int leftResult = Left.Evaluate();
            int rightResult = Right.Evaluate();
            switch (OpType)
            {
                case OperatorType.Multiply:
                    return leftResult * rightResult;
                case OperatorType.Divide:
                    try
                    {
                        if (leftResult % rightResult != 0) return 0; // only integer division is allowed
                        return leftResult / rightResult;
                    }
                    catch (DivideByZeroException)
                    {
                        return 0;
                    }
                case OperatorType.Add:
                    return leftResult + rightResult;
                default:
                    return leftResult - rightResult;
            }
        }

        public override string ToString()
        {
            if (Left == null && Right == null)
            {
                // leaf node, must be a number node
                return Number.ToString();
            }

            // must be an operator node
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            builder.Append(Left);
            builder.Append(' ');
            builder.Append(OperatorTypeString(OpType));
            builder.Append(' ');
            builder.Append(Right);
            builder.Append(')');
            return builder.ToString();
        }
    }
}