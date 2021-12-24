# LettersAndNumbers

A C# solution finder for games used in the [_Letters and Numbers_](https://en.wikipedia.org/wiki/Letters_and_Numbers) television program (similar to _Des chiffres et des lettres_ and _Countdown_).

Runs as a console application.

## Numbers Solver

Finds solutions to the numbers game.
If a solution exists, the solver is guaranteed to find one.

Currently, the solver does not attempt to find solutions that are "close" to the target if no perfect solution could be found. 

### Multiple solutions

By default, the solver will stop once the first valid solution is found.
To find all possible solutions, specify "y" when prompted for the option to find multiple solutions.

### Example usage

```
Enter target number (0-999): 593
Target is 593.
Enter chosen number #1: 50
Enter chosen number #2: 100
Enter chosen number #3: 3
Enter chosen number #4: 1
Enter chosen number #5: 1
Enter chosen number #6: 8
Numbers are 50, 100, 3, 1, 1, 8.
Try to find multiple solutions? (y/n): n
Solved after 2,860,135 attempts.
(((50 + 100) x (3 + 1)) + (1 - 8))
```

### Implementation

The solver uses a brute-force approach of evaluating all possible expressions combining the chosen numbers.

Arithmetic expressions are represented by expression trees, which are evaluated by recursively traversing the tree.
