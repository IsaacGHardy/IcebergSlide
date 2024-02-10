#Hi, I'm OX, the AI for Quixo

#TODO: Go for a kill

import copy

#Refactor function
def apply_move(board, move_block_from, move_block_to, player_turn):
    if (move_block_to[1] != move_block_from[1]):
        #Were moving rows
        old_row = int(move_block_from[1])
        new_row = int(move_block_to[1])
        Col = int(move_block_from[3])

        blocks_to_shift = new_row - old_row
        if (blocks_to_shift > 0):
            for i in range(0, abs(blocks_to_shift)):            
               board[old_row + i][Col] = board[old_row + i + 1][Col]
        else:
            for i in range(0, abs(blocks_to_shift)):            
                board[old_row - i][Col] = board[old_row - i -+ 1][Col]
    else:
        #Were moving cols
        old_col = int(move_block_from[3])
        new_col = int(move_block_to[3])
        Row = int(move_block_from[1])

        blocks_to_shift = new_col - old_col
        if (blocks_to_shift > 0):
            for i in range(0, abs(blocks_to_shift)):            
                board[Row][old_col + i] = board[Row][old_col + i + 1]
        else:
            for i in range(0, abs(blocks_to_shift)):            
                board[Row][old_col - i] = board[Row][old_col - i -+ 1]

    board[int(move_block_to[1])][int(move_block_to[3])] = player_turn

def get_opponent(playing_as):
    if (playing_as == "X"):
        return "O"
    elif (playing_as == "O"):
        return "X"

def str_to_int_spot_data(x):
    spot_data = x[1:4].split(",")
    return int(spot_data[0]), int(spot_data[1])

def is_a_corner(spot_row, spot_col):
    if ((spot_row % 4 == 0) and spot_col % 4 == 0):
        return True
    return False

def check_for_streaks(board, team_looking_at):
    streaks = []

    for row in board:
        streaks.append(row.count(team_looking_at))

    for col in zip(*board):
        streaks.append(col.count(team_looking_at))

    team_in_downward_diagonal = 0
    for i in range(5):
        if (board[i][i] == team_looking_at):
            team_in_downward_diagonal += 1
    streaks.append(team_in_downward_diagonal)

    team_in_upward_diagonal = 0
    for i in range(5):
        if (board[i][len(board) - 1 - i] == team_looking_at):
            team_in_upward_diagonal += 1
    streaks.append(team_in_upward_diagonal)

    streaks.sort(reverse = True)
    return streaks

def score_pickup(spot_contains):
    pickup_score = 0

    if (spot_contains == " "):
       pickup_score += 25
       
    return pickup_score

def generate_future_board(board, player_turn, pickup_row, pickup_col, placement_row, placement_col):
    future_board = copy.deepcopy(board)
    move_block_from = "(" + str(pickup_row) + "," + str(pickup_col) + ")"
    move_block_to = "(" + str(placement_row) + "," + str(placement_col) + ")"

    apply_move(future_board, move_block_from, move_block_to, player_turn)

    return future_board

def score_placement(board, playing_as, pickup_row, pickup_col, placement_row, placement_col):
    placement_score = 0
    opponent_as = get_opponent(playing_as)
    future_board = generate_future_board(board, playing_as, pickup_row, pickup_col, placement_row, placement_col)

    if (is_a_corner(placement_row, placement_col)):
        placement_score = 10

    if (check_for_streaks(board, opponent_as)[0] > check_for_streaks(future_board, opponent_as)[0]):
        placement_score += 100

    if (check_for_streaks(board, playing_as)[0] > check_for_streaks(future_board, playing_as)[0]):
        placement_score += 15
        if (check_for_streaks(board, opponent_as)[0] < check_for_streaks(future_board, opponent_as)[0]):
            placement_score -= 15

    if (check_for_streaks(future_board, playing_as)[0] == 5 and check_for_streaks(future_board, opponent_as)[0] != 5):
        placement_score += 1000

    return placement_score

def get_placements(row, col):
    spots = []

    spots.append("(" + str(row) + "," + str(0) + ")")
    spots.append("(" + str(row) + "," + str(4) + ")")
    spots.append("(" + str(0) + "," + str(col) + ")")
    spots.append("(" + str(4) + "," + str(col) + ")")
    spots.remove("(" + str(row) + "," + str(col) + ")")

    return spots

def get_all_moves(board, EDGES_OF_THE_BOARD, playing_as):
    possible_moves = {}

    for x in EDGES_OF_THE_BOARD:
        pickup_row, pickup_col = str_to_int_spot_data(x)
        spot_contents = board[pickup_row][pickup_col]

        if (spot_contents == " " or spot_contents == playing_as):
            pickup_score = 0
            pickup_score += score_pickup(spot_contents)

            for spot in get_placements(pickup_row, pickup_col):
                placement_row, placement_col = str_to_int_spot_data(spot)
                placement_score = 0
                placement_score += score_placement(board, playing_as, pickup_row, pickup_col, placement_row, placement_col)

                combined_move = x + " " + spot
                possible_moves[combined_move] = pickup_score + placement_score

    return possible_moves

def request_ai_move(board, EDGES_OF_THE_BOARD, playing_as):
    possible_moves = get_all_moves(board, EDGES_OF_THE_BOARD, playing_as)
    best_move_set = max(possible_moves.items(), key=lambda item: item[1])[0]

    #for key, value in sorted(possible_moves.items(), key=lambda item: item[1]):
    #    print(f'{key}: {value}')
    #print()

    spot_data = best_move_set.split(" ")

    #print(spot_data)
    #print()

    return spot_data[0], spot_data[1]