using TicTacToeLibrary.Constants;

namespace TicTacToeLibrary;

public partial class GameLogic : IDisposable
{
    //loca variables
    private Move _humanLastMove;
    private Move _computerLastMove;
    private int _computerMovesCount;
    private int _humanMovesCount;
    private Move[,] _movesBoard;
    private Levels _level;

    /// <summary>
    /// Constructor initializes local variable for moves' board with given row and column ranges from <see cref="GlobalConstants"/>.
    /// </summary>
    public GameLogic()
    {
        _movesBoard = new Move[GlobalConstants.I_Size, GlobalConstants.J_Size];
    }

    /// <summary>
    /// Checks if the argument is not null and respective i,j coordinates are not allocated with existing object; assigns the given index of the board to the given object reference.
    /// </summary>
    /// <param name="playerType"><see cref="PlayerTypes"/> object indicating current player type.</param>
    /// <param name="move"><see cref="Move"/> type object carrying information on th current move's i,j index and player type.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool AddMove(PlayerTypes playerType, Move move)
    {
        //if the object is null or board already has an object reference for the given index, return false
        if (move == null || _movesBoard[move.I, move.J] != null)
        {
            return false;
        }
        
        //double check for player type
        if (move.PlayerType != playerType)
        {
            throw new ArgumentException(message: "PlayerType error", nameof(move));
        }

        //assign current player's last move, and increment total moves' number
        else
        {
            switch (playerType)
            {
                case PlayerTypes.Human:
                    _humanLastMove = move;
                    _humanMovesCount++;
                    break;
                case PlayerTypes.Computer:
                    _computerLastMove = move;
                    _computerMovesCount++;
                    break;
                //non-alloweble case
                case PlayerTypes.NoPlayer:
                    throw new ArgumentException(message: "PlayerType should be either human or computer.", nameof(move));
            }

            // assign the object referens to specified baord index adn return true
            _movesBoard[move.I, move.J] = new Move(move.I, move.J, playerType);
            return true;
        }
    }

    #region Get methods
    /// <summary>
    /// Gets next mvoe for current player. If the player has not ay registered move, <see cref="GetMoveNearToProvidedMove"/> is called, else <seealso cref="GetMoveByPatterns"/>. If no any moves where found based on patterns, <see cref="GetDefaultMove"/> will be called.
    /// </summary>
    /// <param name="playerType">Shows type of the current player.</param>
    /// <returns>Returns suggested move for the current player.</returns>
    public Move GetMove(PlayerTypes playerType)
    {
        Move move = null!;
        int countOfMovesOfCurrentPlayer = 0;
        Move opponentLastMove = null!;

        //identify player and assign local variables
        switch (playerType)
        {
            case PlayerTypes.Computer:
                countOfMovesOfCurrentPlayer = _computerMovesCount;
                opponentLastMove = _humanLastMove;
                break;
            case PlayerTypes.Human:
                countOfMovesOfCurrentPlayer = _humanMovesCount;
                opponentLastMove = _computerLastMove;
                break;
        }

        // if it's current player's first move, place it near to opponent's first move
        if (countOfMovesOfCurrentPlayer == 0)
        {
            move = GetMoveNearToProvidedMove(playerType, opponentLastMove);
        }
        //if it is not current player's first move, get current player's first move based on patterns
        else
        {
            move = GetMoveByPatterns(playerType, _level)!;
        }

        // if no possible move was identified based on patterns, a default move will be found
        if (move == null)
        {

            move = GetDefaultMove(playerType)!;
        }

        //null can be returned if no move is found (tie game)
        return move;
    }
    
    /// <summary>
    /// Selects a valid move near to the provided move as an argument. For all directions a posisble near place is checked for validity, and if the place is found, the method returns the move for the current player type.
    /// </summary>
    /// <param name="playerType">Current player type.</param>
    /// <param name="move">The move, near to which the next move shall be identified.</param>
    /// <returns>Returns a valid move suggestion for the current player.</returns>
    public Move GetMoveNearToProvidedMove(PlayerTypes playerType, Move move)
    {
        int i = move.I;
        int j = move.J;

        // (i, j+1) case - horizontal right
        if (j + 1 < GlobalConstants.J_Size && GetMoveByCoordinates(i, j + 1) == null)
        {
            return new Move(i, j + 1, playerType);
        }
        //(i, j-1) case - horizontal left
        else if ((j - 1 > 0 && GetMoveByCoordinates(i, j - 1) == null))
        {
            return new Move(i, j - 1, playerType);
        }
        //(i+1, j) case - vertical down
        else if (i + 1 < GlobalConstants.I_Size && GetMoveByCoordinates(i + 1, j) == null)
        {
            return new Move(i + 1, j, playerType);
        }
        //(i-1, j) case - vertical up
        else if (i - 1 > 0 && GetMoveByCoordinates(i - 1, j) == null)
        {
            return new Move(i - 1, j, playerType);
        }
        //(i+1, j+1) case - diagonal up down, right from the comparison move
        else if (i + 1 < GlobalConstants.I_Size && j + 1 < GlobalConstants.J_Size && GetMoveByCoordinates(i + 1, j + 1) == null)
        {
            return new Move(i + 1, j + 1, playerType);
        }
        //(i-1, j-1) case - diagonal up down, left from the comparison move
        else if (i - 1 > 0 && j - 1 > 0 && GetMoveByCoordinates(i - 1, j - 1) == null)
        {
            return new Move(i - 1, j - 1, playerType);
        }
        //(i-1, j+1) - diagonal down up, right from the comparison move
        else if (i - 1 > 0 && j + 1 < GlobalConstants.J_Size && GetMoveByCoordinates(i - 1, j + 1) == null )
        {
            return new Move(i - 1, j + 1, playerType);
        }
        //(i+1, j-1) - diagonal down up, left from the comparison move
        else /*(i + 1 < GlobalConstants.I_Size && j - 1 > 0 && GetMoveByCoordinates(i + 1, j - 1) == default)*/
        {
            return new Move(i + 1, j - 1, playerType);
        }
    }
    
    /// <summary>
    ///Identifies the opponent player type, defines the length of patterns to be search for based on the level of difficulcy.
    /// </summary>
    /// <param name="currentPlayerType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public Move? GetMoveByPatterns(PlayerTypes currentPlayerType, Levels level)
    {
        PlayerTypes opponentPlayerType;
        
        switch (currentPlayerType)
        {
            case PlayerTypes.Computer:
                opponentPlayerType = PlayerTypes.Human;
                break;
            case PlayerTypes.Human:
                opponentPlayerType = PlayerTypes.Computer;
                break;
            case PlayerTypes.NoPlayer:
                throw new ArgumentException(message: "'NoPlayer' type is not eligible for this method.", nameof(currentPlayerType));
            default:
                opponentPlayerType = _humanMovesCount > _computerMovesCount ? PlayerTypes.Human : PlayerTypes.Computer;
                break;
        }

        // number of defualt elements in the pattern to search for
        int count;
        int countForSecondAnalysis;

        //define the level of difficulcy
        switch (level)
        {
            //when the winning number of steps is 5 (winning pattern equals 'XXXXX' or 'OOOOO'), for easy level the algorithm will analyse only 3-character patterns (e.g. 'ZXX' / 'OZO', where 'Z' is the default charachter for empty places and 'X' and 'O' are players' signs/
            case Levels.Easy:
                count = countForSecondAnalysis = GlobalConstants.WinningMovesNumber - 3;
                break;
            // analyzes only 4 characters' pattern
            case Levels.Medium:
                count = countForSecondAnalysis = GlobalConstants.WinningMovesNumber - 2;
                break;
            //will check 5-to-2 character patterns
            case Levels.Hard:
                count = countForSecondAnalysis = GlobalConstants.WinningMovesNumber - 1;
                break;
            default:
                //inc ase of any error the algorithm will analyse from 5 two 2 charachter patterns.
                count = countForSecondAnalysis = GlobalConstants.WinningMovesNumber - 1;
                break;
        }
        
        //create a local object reference for pattern helper to deal with pattern creation or obtaining
        var patternHelper = new PatternHelper();

        //for the given number of of defualt elements in the pattern, check for a valid move for both players, and in case the default player has move, select it; if no any move for current player is suggested but a move based on opponent's last move is suggested, select it.
        while (count >= 1)
        {
            //get patterns for both players with the given number of characters. As a default character shall be present in the pattern, the count will always be less then the winning moves number.
            var oppositeSignPatterns = patternHelper.GetPatterns(opponentPlayerType, count);
            var currentSignPatterns = patternHelper.GetPatterns(currentPlayerType, count);


            // if the player has an available move for the given-number-of-characterns pattern, get the move, to make progress.
            Move moveByCurrentPlayerLastMove = GetMoveBySinglePattern(currentPlayerType, currentSignPatterns);
            Move moveByOppositePlayerLastMove = GetMoveBySinglePattern(opponentPlayerType, oppositeSignPatterns);

            //current player has a move, get it
            if (moveByCurrentPlayerLastMove != null)
            {
                return moveByCurrentPlayerLastMove;
            }
            // if no pattern match was found for the current player (e.g. the computer does not have a 4-character pattern match), but the opposive player has, get the move to prevent its progress.
            else if (moveByOppositePlayerLastMove != null)
            {
                return new Move(moveByOppositePlayerLastMove.I, moveByOppositePlayerLastMove.J, currentPlayerType);
            }

            //if no any pattern match for either player was identified, decrement the number of pattern characters and look for the next set of patterns.
            count--;
        }

        // if still no valid move found, check the entire board with the same logic as above: for both players' last mvoes, prioritize curent player's last move-based suggestion.
        if (_level == Levels.Hard)
        {
            while (countForSecondAnalysis >= 1)
            {
                //get patterns'sets for both players based on defined 
                var oppositeSignPatterns = patternHelper.GetPatterns(opponentPlayerType, countForSecondAnalysis);
                var currentSignPatterns = patternHelper.GetPatterns(currentPlayerType, countForSecondAnalysis);

                Move moveByCurrentPlayerLastMove = GetMoveByBoardAnalysis(currentPlayerType, currentSignPatterns);
                Move moveByOppositePlayerLastMove = GetMoveByBoardAnalysis(opponentPlayerType, oppositeSignPatterns);

                // if current player has a move, get it
                if (moveByCurrentPlayerLastMove != null)
                {
                    return moveByCurrentPlayerLastMove;
                }
                // if no pattern match was found for the current player (e.g. the computer does not have a 4-character pattern match like 'ZOOO'), but the opponent player has ('XXXZ'), get the move to prevent opponent's progress.
                else if (moveByOppositePlayerLastMove != null)
                {
                    return new Move(moveByOppositePlayerLastMove.I, moveByOppositePlayerLastMove.J, currentPlayerType);
                }

                countForSecondAnalysis--;
            }
        }

        //return null if no valid move identified
        return null;
    }

    /// <summary>
    /// Checks pattern match for arrays containing curent move.
    /// </summary>
    /// <param name="playerType">Current player.</param>
    /// <param name="patterns"></param>
    /// <returns>Returns a valid move or null.</returns>
    public Move? GetMoveBySinglePattern(PlayerTypes playerType, string[] patterns)
    {
        Move lastMove = null!;
        string text = string.Empty;

        //identify player type and assign las move reference to local variable
        switch (playerType)
        {
            case PlayerTypes.Human:
                lastMove = _humanLastMove;
                break;
            case PlayerTypes.Computer:
                lastMove = _computerLastMove;
                break;
        }

        //segementate 4 arrays (horizontal, vertical, down up and up down diagonals, where the current move is in the center of each array
        Move[][] actualMovesArray = Segmentate(lastMove);

        //for each of 4 arrays
        for (int i = 0; i < 4; i++)
        {
            //create a text consisting of all moves' signs
            for (int j = 0; j < actualMovesArray[i].Length; j++)
            {
                text += actualMovesArray[i][j].GetSign();
            }

            // foreach pattern of the pattern's array argument
            foreach (var pattern in patterns)
            {
                //check if the text contains the pattern
                if (text.Contains(pattern))
                {
                    //if yes -> check that pattern is not a winning pattern (includes default sign)
                    //if includes -> find out move's row and column and return the move
                    if (pattern.Contains(Signs.DefaultSign))
                    {
                        int indexOfPatternInText = text.IndexOf(pattern);
                        int indexOfSignInPattern = pattern.IndexOf(Signs.DefaultSign);

                        actualMovesArray[i][0 + indexOfPatternInText + indexOfSignInPattern].PlayerType = playerType;

                        return actualMovesArray[i][0 + indexOfPatternInText + indexOfSignInPattern];
                    }
                    else //for the winning case 'XXXXX' not containing default value (in case of number of winning moves equals 5
                    {
                        //in this case a fake move will return, for HasWinner method
                        return new Move(0, 0, PlayerTypes.NoPlayer); //fake mvoe not to return null
                    }
                }

            }
            
            //clear the text forthe next array
            text = string.Empty;
        }

        //return
        return null;
    }
    
    /// <summary>
    /// This method checks all moves of current player and returns firs valid move near to player's move.
    /// </summary>
    /// <param name="playerType">Current player.</param>
    /// <returns>Return valid move. or null if no move is found (tie game).</returns>
    public Move? GetDefaultMove(PlayerTypes playerType)
    {
        Move move = null!;

        List<Move> ComputerMoves = GetAllMovesForPlayer(PlayerTypes.Computer);
        List<Move> HumanMoves = GetAllMovesForPlayer(PlayerTypes.Computer);

        List<Move> currentPlayerMoves = null!;
        List<Move> opponentPlayerMoves = null!;
        //
        switch (playerType)
        {
            case PlayerTypes.Computer:
                currentPlayerMoves = ComputerMoves;
                opponentPlayerMoves = HumanMoves;
                break;
            case PlayerTypes.Human:
                currentPlayerMoves = HumanMoves;
                opponentPlayerMoves = ComputerMoves;
                break;
        }

        //get first valid move near to current player's move
        if (currentPlayerMoves != null && currentPlayerMoves.Count > 0)
        {
            for (int i = 0; i < currentPlayerMoves.Count; i++)
            {
                move = GetMoveNearToProvidedMove(playerType, currentPlayerMoves[i]);
                if (move != null)
                {
                    break;
                }
            }
        }
        else
        {
            //get first valid move near to opponent player's move
            if (opponentPlayerMoves != null && opponentPlayerMoves.Count > 0)
            {
                for (int i = 0; i < opponentPlayerMoves.Count; i++)
                {
                    move = GetMoveNearToProvidedMove(playerType, opponentPlayerMoves[i]);
                    if (move != null)
                    {
                        break;
                    }
                }
            }
        }

        //return valid move or null
        return move;
    }
    
    /// <summary>
    /// If given row (i) and column (j) coordinates are in the default ranges defined in <see cref="GlobalConstants"/> value of the given index of the moves' board is returned.
    /// </summary>
    /// <param name="i">Row index.</param>
    /// <param name="j">Column index.</param>
    /// <returns>Returns valid move or null.</returns>
    public Move? GetMoveByCoordinates(int i, int j)
    {
        Move move = null!;
        if ((i >= 0 && i <= GlobalConstants.I_Size) && (j >= 0 && j <= GlobalConstants.J_Size))
        {
            move = _movesBoard[i, j];
        }

        return move;
    }
   
    /// <summary>
    /// Gets all moves of the current player.
    /// </summary>
    /// <param name="playerType">Current player given as argument.</param>
    /// <returns>Returns list of all existing moves or an empty lsit, if no any moves are found.</returns>
    public List<Move> GetAllMovesForPlayer(PlayerTypes playerType)
    {
        List<Move> playerMoves = _movesBoard.Cast<Move>().
            Where(playerMove => playerMove != null && playerMove.PlayerType == playerType).
            ToList();

        return playerMoves;
    }
    #endregion

    /// <summary>
    /// Creates four arrays for for directions (horizontal, vertical, down up and up down diagonals) taking current move and + (winning moves number - 1) characters to the left and right or up and down.
    /// </summary>
    /// <param name="move">Current player's last move.</param>
    /// <returns>Returns a jagged array with four arrays for four directions.</returns>
    public Move[][] Segmentate(Move move)
    {
        //jagged array to carry 4 arrays containing current player's move.
        Move[][] segmentArrays = new Move[4][];
        
        //for 5-winning-move case the length of the array will be 9 = (4 + 1 + 4) 
        int arrayLength = GlobalConstants.WinningMovesNumber * 2 - 1;

        //4 arrays for four dimensions
        Move[] horizontalMoves = new Move[arrayLength];
        Move[] verticalMoves = new Move[arrayLength];
        Move[] diagonalDownUpMoves = new Move[arrayLength];
        Move[] diagonalUpDownMoves = new Move[arrayLength];

        // the current move will be in the center of each array
        horizontalMoves[GlobalConstants.WinningMovesNumber - 1] = move;
        verticalMoves[GlobalConstants.WinningMovesNumber - 1] = move;
        diagonalDownUpMoves[GlobalConstants.WinningMovesNumber - 1] = move;
        diagonalUpDownMoves[GlobalConstants.WinningMovesNumber - 1] = move;

        int difference = 1;
        //add moves to the righ of the arrays
        for (int i = GlobalConstants.WinningMovesNumber; i < arrayLength; i++)
        {
            // (i, j+1) case - horizontal right
            if (move.J + difference < GlobalConstants.J_Size)
            {
                horizontalMoves[i] = GetMoveByCoordinates(move.I, move.J + difference) ?? new Move(move.I, move.J + difference, PlayerTypes.NoPlayer);
            }

            // (i+1, j) case - vertical down
            if (move.I + difference < GlobalConstants.I_Size)
            {
                verticalMoves[i] = GetMoveByCoordinates(move.I + difference, move.J) ?? new Move(move.I + difference, move.J, PlayerTypes.NoPlayer);
            }

            // (i-1, j+1) case - diagonal down up to the right
            if (move.I - difference >= 0 && move.J + difference < GlobalConstants.J_Size)
            {
                diagonalDownUpMoves[i] = GetMoveByCoordinates(move.I - difference, move.J + difference) ?? new Move(move.I - difference, move.J + difference, PlayerTypes.NoPlayer);
            }

            // (i+1, j+1) case - diagonal up down to the right
            if (move.I + difference < GlobalConstants.I_Size && move.J + difference < GlobalConstants.J_Size)
            {
                diagonalUpDownMoves[i] = GetMoveByCoordinates(move.I + difference, move.J + difference) ?? new Move(move.I + difference, move.J + difference, PlayerTypes.NoPlayer);
            }

            difference++;
        }

        //add moves to the left of the arrays
        difference = 1;
        for (int i = GlobalConstants.WinningMovesNumber - 2; i >= 0; i--)
        {
            //(i, j-1) case - horizontal left
            if (move.J - difference >= 0)
            {
                horizontalMoves[i] = GetMoveByCoordinates(move.I, move.J - difference) ?? new Move(move.I, move.J - difference, PlayerTypes.NoPlayer);
            }

            //(i-1, j) case - vertical up
            if (move.I - difference >= 0)
            {
                verticalMoves[i] = GetMoveByCoordinates(move.I - difference, move.J) ?? new Move(move.I - difference, move.J, PlayerTypes.NoPlayer);
            }

            //(i+1, j-1) case - diagonal down up to the left
            if (move.I + difference < GlobalConstants.I_Size && move.J - difference >= 0)
            {
                diagonalDownUpMoves[i] = GetMoveByCoordinates(move.I + difference, move.J - difference) ?? new Move(move.I + difference, move.J - difference, PlayerTypes.NoPlayer);
            }

            //(i-1, j-1) case - diagonal up down to the left
            if (move.I - difference >= 0 && move.J - difference >= 0)
            {
                diagonalUpDownMoves[i] = GetMoveByCoordinates(move.I - difference, move.J - difference) ?? new Move(move.I - difference, move.J - difference, PlayerTypes.NoPlayer);
            }
            
            difference++;
        }

        segmentArrays[0] = horizontalMoves;
        segmentArrays[1] = verticalMoves;
        segmentArrays[2] = diagonalDownUpMoves;
        segmentArrays[3] = diagonalUpDownMoves;

        return segmentArrays;
    }

    /// <summary>
    /// Assigns user choise for game difficulcy level to local variable <see cref="_level"/>.
    /// </summary>
    /// <param name="level">User choice for game difficulcy.</param>
    public void SetLevel(Levels level)
    {
        _level = level;
    }
    
    /// <summary>
    /// Checks for winning pattern in player's move segment (4 arrays - horizontal, vertical, up down diagnal and down up diagonal
    /// </summary>
    /// <param name="playerType">Current player type</param>
    /// <returns>Returns boolean value indicating the state of winning pattern match.</returns>
    public bool IsWinner(PlayerTypes playerType)
    {
        //assign valid character for winning pattern
        char sign;
        switch (playerType)
        {
            case PlayerTypes.Computer:
                if (_computerMovesCount < GlobalConstants.WinningMovesNumber)
                {
                    return false;
                }
                sign = Signs.ComputerSign;
                break;
            case PlayerTypes.Human:
                if (_humanMovesCount < GlobalConstants.WinningMovesNumber)
                {
                    return false;
                }
                sign = Signs.HumanSign;
                break;
            default:
                throw new ArgumentException(message: "Only human and computer signs are allowed.", nameof(sign));
        }

        //create the winning pattern
        string winningPattern = new string(sign, GlobalConstants.WinningMovesNumber);

        //use GetMoveBySinglePattern() method to get a valid move. If move is not null the winning pattern got match and the method will return true.
        Move? move = GetMoveBySinglePattern(playerType, new string[] { winningPattern });

        if (move != null)
        {
            return true;
        }

        return false;
    }
    
    #region Dispose

    // Destructor (finalizer) - to handle cleanup 
    ~GameLogic()
    {
        Dispose();
    }

    public void Dispose()
    {
        DisposeObjects();
        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeObjects()
    {
        _humanLastMove = null;
        _computerLastMove = null;
        _movesBoard = null;
    }
    #endregion
}

