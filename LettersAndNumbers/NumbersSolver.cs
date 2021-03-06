using Ardalis.SmartEnum;

namespace LettersAndNumbers
{
    public class NumbersSolver
    {
        public class SolveMode : SmartEnum<SolveMode>
        {
            /// <summary>
            /// Stop searching after the first valid solution is found
            /// </summary>
            public static readonly SolveMode First = new(nameof(First), 1, "f");
            
            /// <summary>
            /// Search until all valid solutions are found
            /// </summary>
            public static readonly SolveMode All = new(nameof(All), 2, "a");
            
            /// <summary>
            /// Find all valid solutions, then only show the most intuitive one
            /// </summary>
            public static readonly SolveMode MostIntuitive = new(nameof(MostIntuitive), 3, "i");

            public String ResponseCode { get; }

            private SolveMode(string name, int value, string responseCode) : base(name, value)
            {
                ResponseCode = responseCode;
            }

            public static SolveMode? FromResponseCode(string responseCode)
            {
                try
                {
                    return List.First(m => m.ResponseCode.Equals(responseCode));
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }
        
        private const int NumChosenNumbers = 6;
        private SolveMode _mode;

        /// <summary>
        /// Prompts the user to enter the mode of operation (single or multiple solutions).
        /// </summary>
        /// <returns>True if multi-solution mode is selected, false otherwise.</returns>
        private SolveMode AskMode()
        {
            while (true)
            {
                Console.Write("Solution mode: first/all/intuitive (f/a/i): ");
                string? input = Console.ReadLine();
                var solveMode = SolveMode.FromResponseCode(input);
                
                if (solveMode != null)
                {
                    return solveMode;
                }

                Console.WriteLine("Invalid response. Must be \"y\" or \"n\".");
            }
        }
        
        /// <summary>
        /// Prompts the user to enter a target number.
        /// </summary>
        /// The target must be a three digit number, i.e. between 0 and 1000.
        /// <returns>The target number entered by the user.</returns>
        private int AskTarget()
        {
            const int targetMin = 0;
            const int targetMax = 1000;
            int target = 0;
            bool success = false;
            while (!success)
            {
                Console.Write("Enter target number (0-999): ");
                string input = Console.ReadLine();
                success = int.TryParse(input, out target) && target is >= targetMin and < targetMax;
                if (!success)
                {
                    Console.WriteLine("Invalid number entered. " +
                                      "Must be between " + targetMin + " and " + targetMax + ".");
                }
            }

            return target;
        }

        /// <summary>
        /// Prompts the user to enter their selected numbers and returns them in a list.
        /// </summary>
        /// Each number must be either a 'large' or a 'small'. Large numbers are 25, 50, 75 and 100.
        /// Small numbers are 1 through 10 inclusive. Large numbers may only be entered once each.
        /// Duplicate small numbers may be entered.
        /// <returns>List of numbers entered by the user.</returns>
        private List<int> AskNumbers()
        {
            List<int> validNumbers = new List<int> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 25, 50, 75, 100};

            List<int> numbers = new();
            
            for (int i = 0; i < NumChosenNumbers; i++)
            {
                int number = 0;
                bool success = false;
                while (!success)
                {
                    Console.Write("Enter chosen number #" + (i + 1) + ": ");
                    string input = Console.ReadLine();
                    success = int.TryParse(input, out number)
                              && validNumbers.Contains(number) // number must be one of those specified above
                              && (number <= 10 || !numbers.Contains(number)); // cannot have duplicate large numbers
                    if (!success)
                    {
                        Console.WriteLine("Invalid number entered.");
                    }
                }

                numbers.Add(number);
            }
            
            return numbers;
        }

        private void Solve(int target, List<int> numbers)
        {
            List<ArithmeticExpTreeNode> solutions = new();
            ulong attempts = 0;

            var numsPermutations = GetPermutations(Enumerable.Range(0, numbers.Count),
                numbers.Count).Select(t => t.Select(i => numbers[i]));

            // start with expressions involving only one operator, then increase up to the maximum number possible
            for (int size = 1; size < NumChosenNumbers; size++)
            {
                var opsPermutations = GetPermutationsWithRepetition(OperatorType.List, size);
                
                var trees = new List<ArithmeticExpTreeNode>();
                // copy each tree to ensure each expression tree is a separate object
                foreach (var tree in AllArithmeticExpTrees(size))
                {
                    trees.Add(new ArithmeticExpTreeNode(tree));
                }

                foreach (var tree in trees)
                {
                    // loop through all permutations of the numbers entered by the user
                    // each permutation represents a different arrangement of these numbers in the expression
                    foreach (var numsPermutation in numsPermutations)
                    {
                        // loop through all permutations of arithmetic operators, with repetition of the same
                        // operator allowed
                        foreach (var opTypePermutation in opsPermutations)
                        {
                            // place the current permutation of numbers into the tree
                            FillNumbersInTree(tree, numsPermutation);
                            // place the current permutation of operators into the tree
                            FillOperatorsInTree(tree, opTypePermutation);

                            PruneTree(tree);

                            if (tree.Evaluate() == target)
                            {
                                if (_mode == SolveMode.First)
                                {
                                    Console.WriteLine("Solved after " + attempts.ToString("N0") + " attempts.");
                                    Console.WriteLine(tree.ToString());
                                    return;
                                }

                                bool isEquivToSolution = false;
                                foreach (var solution in solutions)
                                {
                                    if (solution.IsEquivalentTo(tree))
                                    {
                                        isEquivToSolution = true;
                                        break;
                                    }
                                }

                                if (!isEquivToSolution)
                                {
                                    ArithmeticExpTreeNode treeCopy = new ArithmeticExpTreeNode(tree);
                                    solutions.Add(treeCopy);

                                    var intuitionScoreText = _mode == SolveMode.MostIntuitive
                                        ? $" [intuition score: {treeCopy.CalculateIntuitionScore()}]"
                                        : "";

                                    Console.WriteLine("Found solution: " + treeCopy + intuitionScoreText);
                                }
                            }

                            attempts++;

                            ClearTree(tree);
                        }
                    }
                }
            }
            // TODO prune to select least operations
            // TODO try to solve for target +/- 1 if no solution found

            if (_mode == SolveMode.MostIntuitive)
            {
                // rank solutions by their intuition score, then only output the first one
                solutions.Sort((s1, s2) => s1.CalculateIntuitionScore() - s2.CalculateIntuitionScore());
                Console.WriteLine("Most intuitive solution: " + solutions[0]);
                return;
            }

            if (solutions.Count > 0)
            {
                Console.WriteLine("Found " + solutions.Count.ToString("N0")
                                           + $" solution{(solutions.Count == 1 ? "" : "s")} in " 
                                           + attempts.ToString("N0") + " attempts");
            }
            else
            {
                Console.WriteLine("No solution after " + attempts.ToString("N0") + " attempts");
            }
        }

        private void ClearTree(ArithmeticExpTreeNode tree)
        {
            if (tree.Left != null)
            {
                if (tree.Left.Left == null && tree.Left.Right == null)
                {
                    tree.Left = null;
                }
                else
                {
                    ClearTree(tree.Left);
                }
            }

            if (tree.Right != null)
            {
                if (tree.Right.Left == null && tree.Right.Right == null)
                {
                    tree.Right = null;
                }
                else
                {
                    ClearTree(tree.Right);
                }
            }
        }

        private void PruneTree(ArithmeticExpTreeNode tree)
        {
            // prune:
            // - multiply by 1
            // - multiply by 0
            // - divide by 1
            // - add to 0
            if (tree.Left.Left == null && tree.Left.Right == null)
            {
                
            }
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsWithRepetition<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new[] {t});
            }

            return GetPermutationsWithRepetition(list, length - 1)
                    .SelectMany(_ => list, (t1, t2) => t1.Concat(new[] {t2}));
        }
        
        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new[] { t });
            }
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new[] { t2 }));
        }

        private static IEnumerable<ArithmeticExpTreeNode> AllArithmeticExpTrees(int size)
        {
            if (size == 0)
            {
                return new ArithmeticExpTreeNode[] {null};
            }

            return from i in Enumerable.Range(0, size)
                from left in AllArithmeticExpTrees(i)
                from right in AllArithmeticExpTrees(size - 1 - i)
                select new ArithmeticExpTreeNode(left, right);
        }

        private void FillNumbersInTree(ArithmeticExpTreeNode root, IEnumerable<int> numbers)
        {
            var enumerator = numbers.GetEnumerator();
            FillNumbersInTree(root, enumerator);
        }

        private void FillNumbersInTree(ArithmeticExpTreeNode root, IEnumerator<int> numbersEnumerator)
        {
            if (root.Left == null)
            {
                numbersEnumerator.MoveNext();
                root.Left = new ArithmeticExpTreeNode(numbersEnumerator.Current);
            }
            else
            {
                FillNumbersInTree(root.Left, numbersEnumerator);
            }
            
            if (root.Right == null)
            {
                numbersEnumerator.MoveNext();
                root.Right = new ArithmeticExpTreeNode(numbersEnumerator.Current);
            }
            else
            {
                FillNumbersInTree(root.Right, numbersEnumerator);
            }
        }

        private void FillOperatorsInTree(ArithmeticExpTreeNode root, IEnumerable<OperatorType> opTypes)
        {
            var enumerator = opTypes.GetEnumerator();
            FillOperatorsInTree(root, enumerator);
        }

        private void FillOperatorsInTree(ArithmeticExpTreeNode root, IEnumerator<OperatorType> opTypesEnumerator)
        {
            if (root.Left == null && root.Right == null)
            {
                return; // leaf (number) node, does not have an operator
            }

            opTypesEnumerator.MoveNext();
            root.OpType = opTypesEnumerator.Current;

            FillOperatorsInTree(root.Left, opTypesEnumerator);
            FillOperatorsInTree(root.Right, opTypesEnumerator);
        }

        public void Run()
        {
            int target = AskTarget();
            Console.WriteLine("Target is " + target + ".");
            
            List<int> numbers = AskNumbers();
            Console.WriteLine("Numbers are " + string.Join(", ", numbers) + ".");

            _mode = AskMode();

            Solve(target, numbers);
        }
    }
}