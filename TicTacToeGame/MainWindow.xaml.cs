using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TicTacToeLibrary;
using TicTacToeLibrary.Constants;

namespace TicTacToeGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    //local variable for game logic
    private GameLogic _gameLogic;

    // 2d array to store board buttons
    private Button[,] _buttons;

    //local variable for difficulcy level
    Levels _level;

    /// <summary>
    /// Constructor initialises components and runs <see cref="PlayNewGame"/> method.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        PlayNewGame();
    }

    /// <summary>
    /// Calls <see cref="RunMenu"/> method, then initializes local variable <see cref="_gameLogic"/>, set's <see cref="_level"/> for 
    /// game difficulcy, and calls <see cref="InitializeGame"/> method.
    /// </summary>
    public void PlayNewGame()
    {
        RunMenu();
        _gameLogic = new GameLogic();
        _gameLogic.SetLevel(_level);
        InitializeGame();
    }

    /// <summary>
    /// Creates an object reference of <see cref="Menu"/> type, displays the menu window as a dialog box, and assigns the user input or default value to <see cref="_level"/> lcoal variable.
    /// </summary>
    private void RunMenu()
    {
        Menu menu = new Menu();
        menu.ShowDialog();
        
        _level = menu.GetLevel();
    }

    /// <summary>
    /// Clears the <see cref="gameGrid"/>, creates new column and row definitions, buttons and adds them to the grid.
    /// </summary>
    private void InitializeGame()
    {
        //clear the grid
        _buttons = new Button[GlobalConstants.I_Size, GlobalConstants.J_Size];
        gameGrid.ColumnDefinitions.Clear();
        gameGrid.RowDefinitions.Clear();
        gameGrid.Children.Clear();


        //create column and row definitions for the grid
        for (int i = 0; i < GlobalConstants.I_Size; i++)
        {
            //set rows for the grid
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
            
            for (int j = 0; j < GlobalConstants.J_Size; j++)
            {
                // set columns
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });
            }
        }

        //create buttons and add them to the grid
        for (int row = 0; row < GlobalConstants.I_Size; row++)
        {
            for (int column = 0; column < GlobalConstants.J_Size; column++)
            {
                Button button = new Button();
                button.Name = $"btn{row}_{column}";
                button.Click += Button_Click; // attach a common event handler, subscribe to Button_Clock method
                button.Content = string.Empty;

                // make it look pretty
                button.BorderThickness = new Thickness(0.4);
                button.Background = new SolidColorBrush(Colors.Ivory);

                //set row and column coordinates
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                //add button to the array and grid
                _buttons[row, column] = button;
                gameGrid.Children.Add(button);
            }
        }
    }

    /// <summary>
    /// Handles <see cref="Button_Click(object, EventArgs)"/> event for each button click. 
    /// Also adds human player's moves to board, then 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;

        //get i, j for the button on the board.
        int row = Grid.GetRow(clickedButton);
        int column = Grid.GetColumn(clickedButton);

        //create a move object
        Move humanMove = new Move(row, column, playerType: PlayerTypes.Human);

 
        //check if the move is eligible add to moves board and assign player's sign to button content
        if (_gameLogic.AddMove(PlayerTypes.Human, humanMove)) //checked - ok
        {
            //assign human sign to current button
            clickedButton.Foreground = Brushes.Blue;
            clickedButton.Content = Signs.HumanSign;
        }
        // else do nothing
        else
        {
            return;
        }

        //check if human is winner. if yes -> display win message and restart the game
        if (_gameLogic.IsWinner(PlayerTypes.Human))
        {
            DisplayMessage(Messages.WinMessage, Titles.MessageBoxTitle);
            PlayNewGame();
        }
        //if human does not win, it is computer's turn
        else
        {
            // get valid response for computer, add it to the board and assign computer sign to the respective button on the grid.
            Move computerMove = _gameLogic.GetMove(PlayerTypes.Computer);
            if (_gameLogic.AddMove(PlayerTypes.Computer, computerMove)) 
            {
                AssignButtonContent(PlayerTypes.Computer, computerMove);
                
                //check if computer winns: if yes -> display game-over message and restart the game
                if (_gameLogic.IsWinner(PlayerTypes.Computer))
                {
                    DisplayMessage(Messages.GameOverMessage, Titles.MessageBoxTitle);
                    PlayNewGame();
                }
            }
            //in case computer does not have any more moves, the game is finished with tie situation: no winner.
            else
            {
                DisplayMessage(Messages.TieGameMessage, Titles.MessageBoxTitle);
                PlayNewGame();
            }
        }
    }

    /// <summary>
    /// Displayes game resutls.
    /// </summary>
    /// <param name="message">Mesage box content.</param>
    /// <param name="title">Massege box title.</param>
    private void DisplayMessage(string message, string title)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Assigns current player's sign to button content through finding the corresponding button on the grid.
    /// </summary>
    /// <param name="playerType"></param>
    /// <param name="move"></param>
    /// <exception cref="Exception"></exception>
    private void AssignButtonContent(PlayerTypes playerType, Move move)
    {
        // update a specific button with given row and column
        Button specificButton = gameGrid.Children.Cast<Button>().FirstOrDefault(x => Grid.GetRow(x) == move.I && Grid.GetColumn(x) == move.J)!;

        //if the button is found assign the content to current player's sign.
        if (specificButton != null)
        {
            switch (playerType)
            {
                case PlayerTypes.Computer:
                    specificButton.Content = Signs.ComputerSign;
                    break;
                case PlayerTypes.Human:
                    specificButton.Content = Signs.HumanSign;
                    break;
            }
        }
        //if the button is not found on the grid, this means that the move's coordinates are not in the predefined ranges, adn method will throw ArgumentOutOfRangeException.
        else
        {
            throw new ArgumentOutOfRangeException(nameof(specificButton), message: "Move's coordinates are out of the predefined range of rows and columns of the grid.");
        }
    }
}