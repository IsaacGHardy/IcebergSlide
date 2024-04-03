from AI import request_ai_move

#Building Towards Middle but dont get too much crazy
row1 = ["O", "O", "O", " ", " "]
row2 = [" ", " ", " ", " ", " "]
row3 = [" ", " ", " ", " ", " "]
row4 = [" ", " ", " ", " ", " "]
row5 = ["X", "X", "X", "X", " "]
start_board = [row1, row2, row3, row4, row5]
request_ai_move(start_board, "O", 0)



#Check ahead for winnies
'''if (get_pieces_on_board(board) > 6):
        for key, data in possible_moves.items():
            if (data[0] > 0):
                moves_data = key.split(" ")
                pickup_row, pickup_col = str_to_int_spot_data(moves_data[0])
                placement_row, placement_col = str_to_int_spot_data(moves_data[1])

                future_board = generate_future_board(board, playing_as, pickup_row, pickup_col, placement_row, placement_col)
                future_possible_moves = get_all_moves(future_board, playing_as)
                best_score = max(future_possible_moves.values())

                if (int(best_score[0]) > 90,000):
                    data[0] += 1000
                    data[1] + " " + " Attempting to win in 2 Moves"
    '''