using TicTacToeLibrary.Constants;

namespace TicTacToeLibrary;

public static class Extensions
{
    /// <summary>
    /// An extension method for Move type gets player type for specific object of <see cref="Move"/ type.>
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static char GetSign(this Move input)
    {
        //in case the coordinates of the move go beyond the board.
        if (input == null)
        {
            return '*';
        }
        //in case the coordinates coordinate to the board ranges, but no move is set to the index / null case
        if (input.PlayerType == PlayerTypes.NoPlayer)
        {
            //set to specific character, as in case of defualt, the first character of the string(if default) was not used by compiler
            return 'Z';
        }
        else if (input.PlayerType == PlayerTypes.Human)
        {
            return Signs.HumanSign;
        }
        else
        {
            return Signs.ComputerSign;
        }
    }
}
