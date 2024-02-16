using System;

class Program
{
    static void Main()
    {

    }


    public int ScorePickup(char spotContains, int pickupRow, int pickupCol)
    {
        int pickupScore = 0;

        //We want to flip new pieces
        if (spotContains == ' ')
        {
            pickupScore += 100;
        }

        //Don't remove your own corners
        if (IsACorner(pickupRow, pickupCol))
        {
            pickupScore -= 5;
        }

        return pickupScore;
    }

    public string GetOpponent(string playingAs)
    {
        switch (playingAs)
        {
            case "X":
                return "O";
            case "O":
                return "X";
        }
    }

    public static (int, int) StrToIntSpotData(string x)
    {
        string[] spotData = x.Substring(1, 3).Split(',');
        return (int.Parse(spotData[0]), int.Parse(spotData[1]));
    }

    public static bool IsACorner(int spotRow, int spotCol)
    {
        return (spotRow % 4 == 0) && (spotCol % 4 == 0);
    }

    public static List<string> GetPlacements(int row, int col)
    {
        List<string> spots = new List<string>();
        spots.Add("(" + row.ToString() + "," + "0" + ")");
        spots.Add("(" + row.ToString() + "," + "4" + ")");
        spots.Add("(" + "0" + "," + col.ToString() + ")");
        spots.Add("(" + "4" + "," + col.ToString() + ")");
        spots.Remove("(" + row.ToString() + "," + col.ToString() + ")");
        return spots;
    }

    public static int[] CheckForStreaks(char[,] board, char teamLookingAt)
    {
        int[] streaks = new int[3];

        // Horizontal streaks.
        for (int i = 0; i < board.GetLength(0); i++)
        {
            int streak = 0;
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i][j] == teamLookingAt)
                {
                    streak++;
                }
                else
                {
                    break;
                }
            }
            streaks[0] = Math.Max(streaks[0], streak);
        }

        // Vertical streaks.
        for (int j = 0; j < board.GetLength(1); j++)
        {
            int streak = 0;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                if (board[i][j] == teamLookingAt)
                {
                    streak++;
                }
                else
                {
                    break;
                }
            }
            streaks[1] = Math.Max(streaks[1], streak);
        }

        // Downward diagonal streak.
        int downwardDiagonalStreak = 0;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (board[i][i] == teamLookingAt)
            {
                downwardDiagonalStreak++;
            }
            else
            {
                break;
            }
        }
        streaks[2] = Math.Max(streaks[2], downwardDiagonalStreak);

        // Upward diagonal streak.
        int upwardDiagonalStreak = 0;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            if (board[i][board.GetLength(1) - 1 - i] == teamLookingAt)
            {
                upwardDiagonalStreak++;
            }
            else
            {
                break;
            }
        }
        streaks[2] = Math.Max(streaks[2], upwardDiagonalStreak);

        // Sorting in descending order.
        Array.Sort(streaks, (x, y) => x - y);

        return streaks;
    }

    public static Board generate_future_board(Board board, int player_turn, int pickup_row, int pickup_col, int placement_row, int placement_col)
    {
        Board futureBoard = new Board(board);
        string moveBlockFrom = $"({pickup_row},{pickup_col})";
        string moveBlockTo = $"({placement_row},{placement_col})";
        apply_move(futureBoard, moveBlockFrom, moveBlockTo, player_turn);
        return futureBoard;
    }

    public static void apply_move(int[][] board, int[] move_block_from, int[] move_block_to, char player_turn)
    {
        if (move_block_to[1] != move_block_from[1])
        {
            // Moving rows.
            int old_row = move_block_from[1];
            int new_row = move_block_to[1];
            int Col = move_block_from[3];

            int blocks_to_shift = new_row - old_row;
            if (blocks_to_shift > 0)
            {
                for (int i = 0; i < blocks_to_shift; i++)
                {
                    board[old_row + i][Col] = board[old_row + i + 1][Col];
                }
            }
            else
            {
                for (int i = 0; i < -blocks_to_shift; i++)
                {
                    board[old_row - i - 1][Col] = board[old_row - i][Col];
                }
            }
        }
        else
        {
            // Moving cols.
            int old_col = move_block_from[3];
            int new_col = move_block_to[3];
            int Row = move_block_from[1];

            int blocks_to_shift = new_col - old_col;
            if (blocks_to_shift > 0)
            {
                for (int i = 0; i < blocks_to_shift; i++)
                {
                    board[Row][old_col + i + 1] = board[Row][old_col + i];
                }
            }
            else
            {
                for (int i = 0; i < -blocks_to_shift; i++)
                {
                    board[Row][old_col - i - 1] = board[Row][old_col - i];
                }
            }
        }

        board[move_block_to[1]][move_block_to[3]] = (char)player_turn;
    }
}