using System;

namespace bulls_and_cows___jonatan;

public class GuessResult
    {
        public int Bulls { get; set; }
        public int Cows { get; set; }

        /// <summary>
        /// True when Bulls equals 4 (a computed/derived value —
        /// it should not be set directly, just calculated from Bulls).
        /// </summary>
        public bool IsWinningGuess => Bulls == 4;
        public string Feedback => $"{Bulls} Bulls, {Cows} Cows";
    }