def check_for_win(turns_to_think_ahead):
    return True

def check_for_loss(turns_to_think_ahead):
    return True

def search_for_unflipped_pieces():
    #check_for_loss
    #check_for_win
    return True

def is_a_corner(spot_row, spot_col):
    if ((spot_row % 4 == 0) and spot_col % 4 == 0):
        return True
    return False

def distance_from_center(spot_row, spot_col):
    if (spot_row % 4 == 0 or spot_col % 4 == 0):
        return 2
    elif(spot_row == 2 and spot_col == 2):
        return 0
    else:
        return 1

def score_move(spot_contains, playing_as, spot_row, spot_col):
    reward_for_untaken, reward_for_corner, reward_for_middle = 0, 0, 0

    if (spot_contains == " "):
       reward_for_untaken = 25
    if (is_a_corner(spot_row, spot_col)):
        reward_for_corner = 10
    reward_for_middle = (2 - distance_from_center(spot_row, spot_col)) * 15
    
    return reward_for_untaken + reward_for_corner + reward_for_middle

def get_all_moves(board, EDGES_OF_THE_BOARD, playing_as):
    possible_moves = {}
    for x in EDGES_OF_THE_BOARD:
        spot_data = x[1:4].split(",")
        spot_row, spot_col = int(spot_data[0]), int(spot_data[1])

        if (board[spot_col][spot_row] == " " or board[spot_col][spot_row] == playing_as):
            possible_moves[x] = score_move(board[spot_col][spot_row], playing_as, spot_row, spot_col)
    
    return possible_moves

def request_ai_move(board, EDGES_OF_THE_BOARD, playing_as):
    possible_moves = get_all_moves(board, EDGES_OF_THE_BOARD, playing_as)

    dict(sorted(possible_moves.items(), key=lambda item:item[1]))

    best_move = next(possible_moves[0])
    spot_data = best_move[1:4].split(",")
    move_from, move_to = int(spot_data[0]), int(spot_data[1])
    
    return move_from, move_to 