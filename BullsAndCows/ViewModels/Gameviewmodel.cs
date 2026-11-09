using System.ComponentModel;
using System.Windows.Input;
using bulls_and_cows___jonatan;   // your Model classes: BullsAndCowsGame, GuessResult

namespace BullsAndCows.ViewModels;

// The ViewModel sits between the page (View) and the game rules (Model).
// The page binds to the public properties and commands below.
// INotifyPropertyChanged is what tells the page "this value changed, redraw it".
public class GameViewModel : INotifyPropertyChanged
{
    
    // ---------- The Model (all the game rules live here, not in the ViewModel) ----------
    private readonly BullsAndCowsGame _game = new BullsAndCowsGame();

    // ---------- Fields that store the values behind each property ----------
    private string _statusMessage = "";
    private string _guessText = "";
    private string _lastResultText = "";
    private string _errorMessage = "";
    private bool _isGameActive = true;
    private string _guessNumberText = "Guess Number: 0";
    private string _randomNumberText = "";

    // ---------- Commands (the page's buttons call these) ----------
    public ICommand SubmitCommand { get; }
    public ICommand QuitCommand { get; }
    public ICommand RandomNumberCommand { get; }
    

    // ---------- Required by INotifyPropertyChanged ----------
    public event PropertyChangedEventHandler? PropertyChanged;

    public GameViewModel()
    {
        // Each command is told which ViewModel it belongs to ("this").
        SubmitCommand = new SubmitCommand(this);
        QuitCommand = new QuitCommand(this);
        StatusMessage = "I picked a secret 4-digit number with all different digits. Can you guess it?";
        RandomNumberCommand = new RandomNumberCommand(this);
    }

    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ---------- Properties the page binds to ----------

    // Instruction / progress message shown under the title.
    public string StatusMessage
    {
        get { return _statusMessage; }
        set
        {
            if (_statusMessage == value) return;
            _statusMessage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
        }
    }

    // What the player has typed in the guess box (two-way: the Entry also writes to it).
    public string GuessText
    {
        get { return _guessText; }
        set
        {
            if (_guessText == value) return;
            _guessText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GuessText)));
        }
    }

    // "Result: 1 Bulls, 2 Cows" for the last guess.
    public string LastResultText
    {
        get { return _lastResultText; }
        set
        {
            if (_lastResultText == value) return;
            _lastResultText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastResultText)));
        }
    }

    // Red error text. Empty means "no error".
    public string ErrorMessage
    {
        get { return _errorMessage; }
        set
        {
            if (_errorMessage == value) return;
            _errorMessage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            // HasError is worked out from ErrorMessage, so tell the page it changed too.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasError)));
        }
    }
    
   
   

    // True when there is an error to show. Read-only: no setter, no field.
    public bool HasError
    {
        get { return _errorMessage != ""; }
    }
    public string GuessNumberText
{
    get { return _guessNumberText; }
    set
    {
        if (_guessNumberText == value) return;
        _guessNumberText = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GuessNumberText)));
    }
}

    // True while the player can still guess. False after a win or after Quit,
    // which disables the Entry and the Submit button.
    public bool IsGameActive
    {
        get { return _isGameActive; }
        set
        {
            if (_isGameActive == value) return;
            _isGameActive = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGameActive)));
        }
    }
    public string RandomNumberText
    {
        get { return _randomNumberText; }
        set
        {
            if (_randomNumberText == value) return;
        _randomNumberText = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RandomNumberText)));
        }
    }
    
    

    // ---------- What the commands do ----------

    // Runs when the player taps Submit (or presses Enter on the keyboard).
    // Same steps as the console loop in Program.cs.
    

        public void Submit()
    {
        // Trim spaces. If nothing was typed, do nothing (the console app ignores blank input too).
        string input = (GuessText ?? "").Trim();
        if (input == "")
        {
            return;
        }

        // The game is already over -> no more guesses.
        if (!IsGameActive)
        {
            ErrorMessage = "The game is already over!";
            return;
        }

        // Check the guess format using the Model's rules.
        if (!BullsAndCowsGame.IsValidGuess(input, out string errorMessage))
        {
            ErrorMessage = $"Invalid guess: {errorMessage}";
            return;
        }
        
        // The guess is valid: clear any old error, score it, and show the result.
        ErrorMessage = "";
        
        GuessResult result = _game.SubmitGuess(input);
        GuessNumberText = $"Guess Number: {_game.AttemptCount}";   // use your property's real name and text format
        LastResultText = $"Result: {result.Feedback}";
        GuessText = "";

if (result.IsWinningGuess)
{
    StatusMessage = $"Congratulations! You guessed the secret number in {_game.AttemptCount} attempts!";
    IsGameActive = false;
}
        
        // A winning guess ends the game (no guesses are accepted after a win).
        if (result.IsWinningGuess)
        {
            StatusMessage = $"Congratulations! You guessed the secret number in {_game.AttemptCount} attempts!";
            IsGameActive = false;
        }
    }

    // Runs when the player taps Quit.
    public void Quit()
    {
        StatusMessage = "Thanks for playing Bulls and Cows. Goodbye!";
        IsGameActive = false;

        // Closes the app on desktop. Phones (especially iOS) may ignore this.
        Application.Current?.Quit();
    }
    // Runs when the player taps the random number button.
// The Model creates the number; the ViewModel only copies it into the property.
public void GenerateRandomNumber()
{
    _game.GenerateRandomNumber();
    RandomNumberText = _game.RandomNumber.ToString();
}
}

// ============================================================================
// Command classes. A command is just an object with Execute(), which the
// page calls when a button is tapped. Each one simply calls a ViewModel method.
// ============================================================================

public class SubmitCommand : ICommand
{
    private readonly GameViewModel _viewModel;

    public SubmitCommand(GameViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    // Part of ICommand. We never need to disable the command itself
    // (the page uses IsGameActive for that), so this event does nothing.
    public event EventHandler? CanExecuteChanged { add { } remove { } }

    // The button is always allowed to run this command.
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _viewModel.Submit();
    }
}

public class QuitCommand : ICommand
{
    private readonly GameViewModel _viewModel;

    public QuitCommand(GameViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public event EventHandler? CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _viewModel.Quit();
    }
}
public class RandomNumberCommand : ICommand
{
    private readonly GameViewModel _viewModel;

    public RandomNumberCommand(GameViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public event EventHandler? CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object? parameter)
    {
        return true;   // always allowed, even when the game is over
    }

    public void Execute(object? parameter)
    {
        _viewModel.GenerateRandomNumber();
    }
}