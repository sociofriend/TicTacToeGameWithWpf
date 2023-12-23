using System.Windows;
using System.Windows.Controls;
using TicTacToeLibrary.Constants;

namespace TicTacToeGame;

/// <summary>
/// Interaction logic for Menu.xaml
/// </summary>
public partial class Menu : Window
{
    //local variable for game difficulcy level
    private Levels _level;
    public Menu()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Common method for handling button events. In XAML code all three battuns' clicks point to this method.
    /// In case the user closes the window without choosing in default case the 'Hard' level will be set for game difficulcy.   /// </summary>
    /// <param name="sender">Buttuons with the names of 'EasyButton', 'MediumButton' and 'HardButton'.</param>
    /// <param name="e"></param>
    private void CommonButtonClick(object sender, RoutedEventArgs e)
    {
        //local variable for the button
        Button clickedButton = (sender as Button)!;

        //get current button name
        string buttonName = clickedButton.Name;

        //set level based on button name
        switch (buttonName)
        {
            case "EasyButton":
                _level = Levels.Easy;
                break;
            case "MediumButton":
                _level = Levels.Medium;
                break;
            case "HardButton":
                _level = Levels.Hard;
                break;
        }

        this.Close();
    }

    /// <summary>
    /// Gets level of difficulcy. In case user has just closed the window, the method will return default value.
    /// </summary>
    /// <returns></returns>
    public Levels GetLevel()
    {
        return _level;
    }
}
