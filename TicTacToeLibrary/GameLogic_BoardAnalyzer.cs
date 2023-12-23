using TicTacToeLibrary.Constants;

namespace TicTacToeLibrary;

/// <summary>
/// This part of <see cref=" GameLogic"/> class is representing algoritm of checking the entire board for pattern match.
/// </summary>
public partial class GameLogic
{
    /// <summary>
    /// Runs board analysis in four directions - horizontal, vertical, up down and down up diagonals.
    /// </summary>
    /// <param name="playerType">Current player type.</param>
    /// <param name="patterns">Collection of patterns given as argument.</param>
    /// <returns>Returns valid move or null if no move is found.</returns>
    public Move? GetMoveByBoardAnalysis(PlayerTypes playerType, string[] patterns)
    {
        Move? move = null!;
        foreach (var pattern in patterns)
        {
            move = SelectHorizontal(pattern, playerType);

            if (move != null)
            {
                return move;
            }

            move = SelectVertical(pattern, playerType);

            if (move != null)
            {
                return move;
            }

            move = SelectDiagonalDownUp(pattern, playerType);

            if (move != null)
            {
                return move;
            }

            move = SelectDiagonalUpDown(pattern, playerType);

            if (move != null)
            {
                return move;
            }
        }

        //returns move if no valid move is found
        return move;
    }

    //look for all diagonal up down signs concatanated texts (length greate or equal winning number of moves).
    private Move? SelectDiagonalUpDown(string pattern, PlayerTypes playerType)
    {
        Move move = null!;
        string text = string.Empty;


        for (int j = 0; j <= GlobalConstants.J_Size - GlobalConstants.WinningMovesNumber; j++)
        {
            for (int i = 0, k = j; i <= GlobalConstants.I_Size - j &&
                                    k <= GlobalConstants.J_Size - 1; i++, k++)
            {
                text += GetMoveByCoordinates(i, k)!.GetSign();
            }

            if (text.Contains(pattern))
            {
                var firstIndexOfPatternInText = text.IndexOf(pattern);
                var indexOfDefaultInPattern = pattern.IndexOf(Signs.DefaultSign);

                var I = 0 + firstIndexOfPatternInText + indexOfDefaultInPattern;
                var J = j + firstIndexOfPatternInText + indexOfDefaultInPattern;

                return new Move(I, J, playerType);
            }

            text = string.Empty;
        }

        if (move == null)
        {
            for (int i = 1; i <= GlobalConstants.I_Size - GlobalConstants.WinningMovesNumber; i++)
            {
                for (int k = i, j = 0; k <= GlobalConstants.I_Size - 1 &&
                                        j <= GlobalConstants.J_Size - i; k++, j++)
                {
                    text += GetMoveByCoordinates(k, j)!.GetSign();
                }

                if (text.Contains(pattern))
                {
                    var firstIndexOfPatternInText = text.IndexOf(pattern);
                    var indexOfDefaultInPattern = pattern.IndexOf(Signs.DefaultSign);

                    var I = i + firstIndexOfPatternInText + indexOfDefaultInPattern;
                    var J = 0 + firstIndexOfPatternInText + indexOfDefaultInPattern;

                    return new Move(I, J, playerType);
                }

                text = string.Empty;
            }
        }

        return move;
    }

    //look for all diagonal down up signs concatanated texts (length greate or equal winning number of moves).
    private Move? SelectDiagonalDownUp(string pattern, PlayerTypes playerType)
    {
        Move move = null!;
        string text = string.Empty;

        for (int i = GlobalConstants.WinningMovesNumber - 1; i <= GlobalConstants.I_Size - 1; i++)
        {
            for (int k = i, j = 0; k >= 0 && j <= i; k--, j++)
            {
                text += GetMoveByCoordinates(k, j)!.GetSign();
            }

            if (text.Contains(pattern))
            {
                var firstIndexOfPatternInText = text.IndexOf(pattern);
                var indexOfDefaultInPattern = pattern.IndexOf(Signs.DefaultSign);

                var I = i - firstIndexOfPatternInText - indexOfDefaultInPattern;
                var J = 0 + firstIndexOfPatternInText + indexOfDefaultInPattern;

                return new Move(I, J, playerType);
            }

            text = string.Empty;
        }

        for (int j = 1; j <= 24; j++)
        {
            for (int i = GlobalConstants.I_Size - 1, k = j; i >= j && k <= GlobalConstants.J_Size - 1; i--, k++)
            {
                text += GetMoveByCoordinates(i, k)!.GetSign();
            }

            if (text.Contains(pattern))
            {
                var firstIndexOfPatternInText = text.IndexOf(pattern);
                var indexOfDefaultInPattern = pattern.IndexOf(Signs.DefaultSign);

                var I = GlobalConstants.I_Size - 1 - firstIndexOfPatternInText - indexOfDefaultInPattern;
                var J = j + firstIndexOfPatternInText + indexOfDefaultInPattern;

                return new Move(I, J, playerType);
            }

            text = string.Empty;
        }

        return move;
    }

    //looks for all columns for pattern match
    private Move? SelectVertical(string pattern, PlayerTypes playerType)
    {
        Move move = null!;
        string text = string.Empty;

        for (int j = 0; j < GlobalConstants.J_Size; j++)
        {
            for (int i = 0; i < GlobalConstants.I_Size; i++)
            {
                text += GetMoveByCoordinates(i, j)!.GetSign();
            }

            if (text.Contains(pattern))
            {
                var firstIndexOfPatternInText = text.IndexOf(pattern);
                var indexOfDefaultInPattern = pattern.IndexOf(Signs.DefaultSign);

                int i = firstIndexOfPatternInText + indexOfDefaultInPattern;
                return new Move(i, j, playerType);
            }

            text = string.Empty;
        }

        return move;
    }

    //looks for all rows for pattern match
    public Move? SelectHorizontal(string pattern, PlayerTypes playerType)
    {
        Move move = null!;
        string text = string.Empty;

        for (int i = 0; i < GlobalConstants.I_Size; i++)
        {
            for (int j = 0; j < GlobalConstants.J_Size; j++)
            {
                text += GetMoveByCoordinates(i, j)!.GetSign();
            }

            if (text.Contains(pattern))
            {
                var firstIndexOfPatternInText = text.IndexOf(pattern);
                var indexOfDefaultInPattern = pattern.IndexOf(Signs.DefaultSign);

                int j = firstIndexOfPatternInText + indexOfDefaultInPattern;
                return new Move(i, j, playerType);
            }

            text = string.Empty;
        }

        return move;
    }
}
