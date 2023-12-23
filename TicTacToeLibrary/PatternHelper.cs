using TicTacToeLibrary.Constants;

namespace TicTacToeLibrary;

public class PatternHelper
{
    // dictionaries' keys correspond to the count of player signs (e.g. 2 ) in the pattern, and value string arrays carry patterns: possible compositions of the default character (e.g.'Z') with the given number of player sign (e.g.'XXZXX' or 'OZOOO')
    public Dictionary<int, string[]> humanSignPatterns;
    public Dictionary<int, string[]> computerSignPatterns;

    //constructor initialises local variables to empty dictionaries and then creates arrays patterns of 2 to (count +1)-characters, and assigns these patterns' arrays to dictionaries' corresponding values.
    public PatternHelper()
    {
        humanSignPatterns = new Dictionary<int, string[]>();
        computerSignPatterns = new Dictionary<int, string[]>();

        for (int i = 1; i < GlobalConstants.WinningMovesNumber; i++)
        {
            humanSignPatterns[i] = CreatePatterns(Signs.HumanSign, i, new string[i + 1]);
            computerSignPatterns[i] = CreatePatterns(Signs.ComputerSign, i, new string[i + 1]);
        }
    }

    /// <summary>
    /// Gets patterns' array based on count of player signs given in argument.
    /// </summary>
    /// <param name="playerType">Player type</param>
    /// <param name="count">Count of player sign in pattern.</param>
    /// <returns></returns>
    public string[] GetPatterns(PlayerTypes playerType, int count)
    {
        //based on player type given as argument
        switch (playerType)
        {
            //check if dictionary contains given count as key, 
                  //if the array for the given key is initialised and not empty
            case PlayerTypes.Human:
                if (humanSignPatterns.Keys.Contains(count) &&
                    !(humanSignPatterns[count] == null || humanSignPatterns[count].Length == 0))
                {
                    //return corresponding array of patthers
                    return humanSignPatterns[count];
                }
                else
                {
                    //else set the key, and create the needed array of patterns, assign to the given ket and retunr it.
                    humanSignPatterns[count] = CreatePatterns(GetSign(playerType), count, humanSignPatterns[count - 1]);
                    return humanSignPatterns[count];
                }
            case PlayerTypes.Computer:
                if (computerSignPatterns.Keys.Contains(count)
                    && !(computerSignPatterns[count] == null || computerSignPatterns[count].Length == 0))
                {
                    return computerSignPatterns[count];
                }
                else
                {
                    computerSignPatterns[count] = CreatePatterns(GetSign(playerType), count, computerSignPatterns[count]);
                    return computerSignPatterns[count];
                }
            default:
                throw new ArgumentException(message: "Only human or computer player types are allowed.", nameof(playerType));
        }
    }

    public string[] CreatePatterns(char sign, int count, string[] patterns)
    {
        string pattern;

        for (int i = 0; i <= count; i++)
        {
            pattern = new string(sign, count).Insert(i, Signs.DefaultSign.ToString());
            patterns[i] = pattern;
        }

        return patterns;
    }


    /// <summary>
    /// Gets sign for the given player type.
    /// </summary>
    /// <param name="playerType">Current player type.</param>
    /// <returns>Returns char value representing sign of the current player type.</returns>
    public char GetSign(PlayerTypes playerType)
    {
        var playerSign = new char();
        switch (playerType)
        {
            case PlayerTypes.Human:
                playerSign = Signs.HumanSign;
                break;
            case PlayerTypes.Computer:
                playerSign = Signs.ComputerSign;
                break;
            default:
                throw new ArgumentException(message: "Only human or computer player types are allowed.", nameof(playerType));
        }

        return playerSign;
    }
}
