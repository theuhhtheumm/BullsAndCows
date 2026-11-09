using System;

namespace bulls_and_cows___jonatan;

public class BullsAndCowsGame
    {
        private const int CodeLength = 4;
        private readonly Random _random;
        private string _secretNumber;

        // ---- Properties (public, readable from outside the class) ----
        public int AttemptCount { get; private set; }
        public bool IsGameOver { get; private set; }


        public BullsAndCowsGame()
        {
            _random = new Random();
            StartNewGame();
        }

        /// <summary>
        /// Picks a brand-new secret number and resets AttemptCount to 0
        /// and IsGameOver to false. Can be called at any time, even mid-game
        /// — this is what makes the 'n' command work.
        /// </summary>
        public void StartNewGame()
        {
            _secretNumber = GenerateSecretNumber();
            AttemptCount = 0;
            IsGameOver = false;
        }

        /// <summary>
        /// Builds the secret number: shuffle the digits 0–9, take the first 4,
        /// and make sure the first digit isn't 0. Returns a 4-character string.
        /// </summary>
        private string GenerateSecretNumber()
        {
            while (true)
            {
                // Shuffle the digits 0-9 (Fisher-Yates shuffle)
                var digits = Enumerable.Range(0, 10).ToList();
                for (int i = digits.Count - 1; i > 0; i--)
                {
                    int j = _random.Next(i + 1);
                    (digits[i], digits[j]) = (digits[j], digits[i]);
                }

                // Take the first 4 shuffled digits
                var chosen = digits.Take(CodeLength).ToList();

                // Make sure the first digit isn't 0 - if it is, reshuffle and try again
                if (chosen[0] != 0)
                {
                    return string.Concat(chosen);
                }
            }
        }

        /// <summary>
        /// Checks that a guess is properly formatted BEFORE it's scored.
        /// Returns true/false, and fills in errorMessage explaining what's
        /// wrong if it returns false. Static because it doesn't need any
        /// information about a specific game in progress — it's just checking a string.
        /// </summary>
        public static bool IsValidGuess(string guess, out string errorMessage)
        {
            if (string.IsNullOrEmpty(guess))
            {
                errorMessage = "Your guess can't be empty.";
                return false;
            }

            if (guess.Length != CodeLength)
            {
                errorMessage = $"Your guess must be exactly {CodeLength} digits long.";
                return false;
            }

            if (!guess.All(char.IsDigit))
            {
                errorMessage = "Your guess must contain digits only (0-9).";
                return false;
            }

            if (guess.Distinct().Count() != CodeLength)
            {
                errorMessage = "All digits in your guess must be different (no repeats).";
                return false;
            }

            errorMessage = "";
            return true;
        }

        /// <summary>
        /// Scores an already-validated guess against the secret number,
        /// increases AttemptCount by 1, sets IsGameOver to true if it's a
        /// winning guess, and returns a GuessResult. Throws an exception
        /// if called after the game is already over.
        /// </summary>
        public GuessResult SubmitGuess(string guess)
        {
            if (IsGameOver)
            {
                throw new InvalidOperationException(
                    "The game is already over. Start a new game before submitting another guess.");
            }

            AttemptCount++;

            int bulls = 0;
            int cows = 0;

            for (int i = 0; i < CodeLength; i++)
            {
                if (guess[i] == _secretNumber[i])
                {
                    bulls++;
                }
                else if (_secretNumber.Contains(guess[i]))
                {
                    cows++;
                }
            }

            var result = new GuessResult { Bulls = bulls, Cows = cows };

            if (result.IsWinningGuess)
            {
                IsGameOver = true;
            }

            return result;
        }
       // The most recently generated random number (0 until the first click).
        public int RandomNumber { get; set; }
        public void GenerateRandomNumber()
        {
            RandomNumber = _random.Next(1000, 10000);
        
        }
    }
	



    /// This class is NOT one of the Model classes — it's the part of the app
    /// that talks to the keyboard and screen. It's kept separate on purpose:
    /// it uses the three Model classes above but never contains any game rules itself.