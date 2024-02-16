using System;

class Program
{
    static void Main()
    {

    }

    static string[] getAllMoves(board, EDGES_OF_THE_BOARD, playing_as):
        possible_moves = {}

        for x in EDGES_OF_THE_BOARD:
            pickup_row, pickup_col = str_to_int_spot_data(x)
            spot_contents = board[pickup_row][pickup_col]

            if (spot_contents == " " or spot_contents == playing_as):
                pickup_score = 0
                pickup_score += score_pickup(spot_contents, pickup_row, pickup_col)

                for spot in get_placements(pickup_row, pickup_col):
                    placement_row, placement_col = str_to_int_spot_data(spot)
                    placement_score = 0
                    placement_score += score_placement(board, playing_as, pickup_row, pickup_col, placement_row, placement_col)

                    combined_move = x + " " + spot
                    possible_moves[combined_move] = pickup_score + placement_score

        return possible_moves

    //Needs to have a global edges of the board variable
    static bool RequestAiMove(board, teamMovingFor):
        //possible_moves = get_all_moves(board, EDGES_OF_THE_BOARD, playing_as)
        //best_move_set = max(possible_moves.items(), key=lambda item: item[1])[0]

        spot_data = best_move_set.split(" ")

        return spot_data[0], spot_data[1]

    // Custom function to add two numbers
    static int AddNumbers(int a, int b)
    {
        int sum = a + b;
        return sum;
    }
}