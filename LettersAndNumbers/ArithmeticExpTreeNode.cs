using System;
using System.Text;
using Ardalis.SmartEnum;

namespace LettersAndNumbers
{
    public class OperatorType : SmartEnum<OperatorType>
    {
        public static readonly OperatorType Multiply = new(nameof(Multiply), 1, "×");
        public static readonly OperatorType Divide = new(nameof(Divide), 2, "÷");
        public static readonly OperatorType Add = new(nameof(Add), 3, "+");
        public static readonly OperatorType Subtract = new(nameof(Subtract), 4, "-");
        
        public string Symbol { get; }

        private OperatorType(string name, int value, string symbol) : base(name, value)
        {
            Symbol = symbol;
        }
    }
    
    public class ArithmeticExpTreeNode
    {
        public OperatorType OpType { get; set; }
        private int Number { get; }
        public ArithmeticExpTreeNode? Left { get; set; }
        public ArithmeticExpTreeNode? Right { get; set; }
        
        // operator flags
        private bool _subtrahend;
        private bool _divisor;

        public ArithmeticExpTreeNode(ArithmeticExpTreeNode copy)
        {
            OpType = copy.OpType;
            Number = copy.Number;
            if (copy.Left != null) Left = new ArithmeticExpTreeNode(copy.Left);
            if (copy.Right != null) Right = new ArithmeticExpTreeNode(copy.Right);
            _subtrahend = copy._subtrahend;
            _divisor = copy._divisor;
        }

        public ArithmeticExpTreeNode(int number)
        {
            OpType = OperatorType.Multiply;
            Number = number;
        }

        public ArithmeticExpTreeNode(ArithmeticExpTreeNode left, ArithmeticExpTreeNode right)
        {
            OpType = OperatorType.Multiply;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Returns a metric representing how intuitive this expression tree is to humans.
        /// </summary>
        /// The intuition score is a positive integer. A higher intuition score means a less intuitive expression.
        /// <returns>intuition score of this expression tree</returns>
        public int CalculateIntuitionScore()
        {
            if (Left == null && Right == null)
            {
                return 10;
            }

            int operatorIntuitionScore;
            switch (OpType.Name)
            {
                case nameof(OperatorType.Add):
                    operatorIntuitionScore = 20;
                    break;
                case nameof(OperatorType.Multiply):
                    operatorIntuitionScore = 30;
                    break;
                case nameof(OperatorType.Subtract):
                    operatorIntuitionScore = 30;
                    break;
                case nameof(OperatorType.Divide):
                default:
                    operatorIntuitionScore = 50;
                    break;
            }

            return operatorIntuitionScore + Left.CalculateIntuitionScore() + Right.CalculateIntuitionScore();
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
            switch (OpType.Name)
            {
                case nameof(OperatorType.Multiply):
                    return leftResult * rightResult;
                case nameof(OperatorType.Divide):
                    if (rightResult == 0) return 0; // avoid dividing by zero
                    if (leftResult % rightResult != 0) return 0; // only integer division is allowed
                    return leftResult / rightResult;
                case nameof(OperatorType.Add):
                    return leftResult + rightResult;
                default:
                    return leftResult - rightResult;
            }
        }

        /// <summary>
        /// Returns whether the this tree is mathematically equivalent to the given tree.
        /// </summary>
        /// <param name="other">other tree to check equivalence</param>
        /// <returns>true if equivalent, false otherwise</returns>
        public bool IsEquivalentTo(ArithmeticExpTreeNode other)
        {
            // trees with the same structure must be equivalent
            if (HasEquivalentStructureTo(other)) return true;
            
            /*
             * Tree structures may not be equivalent, but represent the same expression.
             * Fill the operator flags and check that both trees contain the same leaf nodes
             * with the same operator flags.
             */
            
            FillOperatorFlags();
            other.FillOperatorFlags();

            // only leaf nodes (containing individual numbers) should be checked for their operator flags
            var onlyLeaves = (ArithmeticExpTreeNode n) => n.Left == null && n.Right == null;

            var otherNodes = other.ToList().Where(onlyLeaves).ToList();
            
            foreach (var ourNode in ToList().Where(onlyLeaves))
            {
                if (!otherNodes.Remove(ourNode))
                {
                    // one of our nodes was not found in list of other nodes
                    return false;
                }
            }

            return true;
        }

        private bool HasEquivalentStructureTo(ArithmeticExpTreeNode other)
        {
            // if this is a leaf node and other is an internal node, return false
            if (Left == null && Right == null && other.Left != null && other.Right != null)
            {
                return false;
            }

            // if this is an external node and other is a leaf node, return false
            if (Left != null && Right != null && other.Left == null && other.Right == null)
            {
                return false;
            }
            
            // if both are leaf nodes, return whether the two numbers are equal
            if (Left == null && Right == null && other.Left == null && other.Right == null)
            {
                return Number == other.Number;
            }
            
            // both are internal nodes
            // if the operators are different, return false
            if (OpType != other.OpType)
            {
                return false;
            }

            switch (OpType.Name)
            {
                case nameof(OperatorType.Multiply):
                case nameof(OperatorType.Add):
                    // commutative operators
                    return Left.HasEquivalentStructureTo(other.Left) && Right.HasEquivalentStructureTo(other.Right)
                           || Left.HasEquivalentStructureTo(other.Right) && Right.HasEquivalentStructureTo(other.Left);
                case nameof(OperatorType.Divide):
                case nameof(OperatorType.Subtract):
                default:
                    // non-commutative operators
                    return Left.HasEquivalentStructureTo(other.Left) && Right.HasEquivalentStructureTo(other.Right);
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
            builder.Append(OpType.Symbol);
            builder.Append(' ');
            builder.Append(Right);
            builder.Append(')');
            return builder.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            ArithmeticExpTreeNode objAsNode = obj as ArithmeticExpTreeNode;

            if (objAsNode == null) return false;

            return Equals(objAsNode);
        }

        public bool Equals(ArithmeticExpTreeNode? other)
        {
            if (other == null) return false;

            if (OpType != other.OpType) return false;

            if (Number != other.Number) return false;

            if (_divisor != other._divisor) return false;

            if (_subtrahend != other._subtrahend) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(OpType, Number, _subtrahend, _divisor).GetHashCode();
        }

        public void FillOperatorFlags()
        {
            FillOperatorFlags(false, false); // root note has no operator flags, since no parent
        }

        private void FillOperatorFlags(bool isSubtrahend, bool isDivisor)
        {
            _subtrahend = isSubtrahend;
            _divisor = isDivisor;
            
            if (Left == null && Right == null)
            {
                // leaf node (number node), no children to fill
                return;
            }

            var rightSubtrahend = isSubtrahend;
            var rightDivisor = isDivisor;
            
            switch (OpType.Name)
            {
                case nameof(OperatorType.Multiply):
                case nameof(OperatorType.Add):
                    break;
                case nameof(OperatorType.Subtract):
                    rightSubtrahend = !isSubtrahend;
                    break;
                case nameof(OperatorType.Divide):
                    rightDivisor = !isDivisor;
                    break;
            }
            
            Left.FillOperatorFlags(isSubtrahend, isDivisor);
            Right.FillOperatorFlags(rightSubtrahend, rightDivisor);
        }

        /// <summary>
        /// Returns a list containing all the nodes in the tree rooted at this node, in the order given by an
        /// in-order traversal.
        /// </summary>
        /// <returns>List of all nodes in this tree</returns>
        public IList<ArithmeticExpTreeNode> ToList()
        {
            var list = new List<ArithmeticExpTreeNode>();
            return ToList(list);
        }

        private IList<ArithmeticExpTreeNode> ToList(IList<ArithmeticExpTreeNode> list)
        {
            Left?.ToList(list);
            list.Add(this);
            Right?.ToList(list);
            return list;
        }
    }
}