using TicTacToeLibrary.Constants;

namespace TicTacToeLibrary;

public class Move
{
    //row coordinate of the move on the board
    public int I { get; set; }

    //column coordinate of the move on the board
    public int J { get; set; }
    
    //player type who amde the move 
    public PlayerTypes? PlayerType { get; set; }

    //builds and object with the given row, column and player type input data
    public Move(int i, int j, PlayerTypes playerType)
    {
        I = i;
        J = j;
        PlayerType = playerType;
    }
}
