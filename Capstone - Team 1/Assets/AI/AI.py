#Hi, I'm OX, the AI for Quixo

from Settings import *
import copy, random

#Reused Function, will break out into a module later
def init_safe_pickup_spots():
    edges_of_The_board = []
    for i in range(5):
        edges_of_The_board.append("(0,"+str(i)+")")
    for i in range(5):
        edges_of_The_board.append("("+str(i)+",0)")
    for i in range(5):
        edges_of_The_board.append("(4,"+str(i)+")")
    for i in range(5):
        edges_of_The_board.append("("+str(i)+",4)")
    return edges_of_The_board

#CONST VARS
EDGES_OF_THE_BOARD = init_safe_pickup_spots()

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

def is_on_crossbar(row, col):
    #I bet other AIs build off the top or left early
    if ((row == 0 and col == 2) or (row == 2 and col == 0)):
        return False
    if (row % 2 == 0 and col % 2 == 0 and not is_a_corner(row, col)):
        return True
    return False

def score_pickup(spot_contains, player_turn, pickup_row, pickup_col, reasoning):
    pickup_score = 0

    reasoning = "Reasoning:"

    if (spot_contains == " "):
       pickup_score += 250
       reasoning += " " + "Unclaimed piece" + " "

    if (is_a_corner(pickup_row, pickup_col) and spot_contains == player_turn):
        pickup_score -= 1
        reasoning += " " + "Don't give up own corner" + " "

    return pickup_score, reasoning

def generate_future_board(board, player_turn, pickup_row, pickup_col, placement_row, placement_col):
    future_board = copy.deepcopy(board)
    move_block_from = "(" + str(pickup_row) + "," + str(pickup_col) + ")"
    move_block_to = "(" + str(placement_row) + "," + str(placement_col) + ")"

    apply_move(future_board, move_block_from, move_block_to, player_turn)

    return future_board

def score_placement(board, playing_as, pickup_row, pickup_col, placement_row, placement_col, reasoning, recursive_counter):
    opponent_as = get_opponent(playing_as)

    #Board States
    future_board = generate_future_board(board, playing_as, pickup_row, pickup_col, placement_row, placement_col)
    
    #Streak Status
    your_current_streaks = check_for_streaks(board, playing_as)[0]
    your_future_streaks = check_for_streaks(future_board, playing_as)[0]
    opp_current_streaks = check_for_streaks(board, opponent_as)[0]
    opp_future_streaks = check_for_streaks(future_board, opponent_as)[0]

    placement_score = 0

    if (is_a_corner(placement_row, placement_col) and board[placement_row][placement_col] != playing_as):
        placement_score += 5
        reasoning += " " + "Takes Corner" + " "

    if (opp_current_streaks > opp_future_streaks):
        placement_score += 100
        reasoning += " " + "Hurts Opponents Max Streak" + " "

    if (opp_current_streaks < 4 and is_on_crossbar(placement_row, placement_col)):
        placement_score += 15
        reasoning += " " + "Push Middle" + " "

    your_max_streak_inc = your_current_streaks < your_future_streaks
    opps_max_streak_does_not_get_scary = opp_future_streaks < 4
    if (your_max_streak_inc and opps_max_streak_does_not_get_scary):
        placement_score += 150
        reasoning += " " + "Builds Streak and does not give opp 4 in a row" + " "

    if (recursive_counter == 0):
        if (your_future_streaks == 5 and opp_future_streaks != 5):
            placement_score += 1000
            reasoning += " " + "Wins" + " "
    elif(recursive_counter == 1):
        if (your_future_streaks == 5 and opp_future_streaks < 4):
            placement_score += 1000
            reasoning += " " + "Wins" + " "

    #Future thinking
    recursive_counter += 1
    if (recursive_counter != AI_MAX_DEPTH):
        possible_moves = get_all_moves(future_board, playing_as, recursive_counter)
    
        for key, value in sorted(possible_moves.items(), key=lambda item: item[1]):
            if (value[0] > 990):
                placement_score += 10
                reasoning += " " + "Setups Win" + " "

    return placement_score, reasoning

def get_placements(row, col):
    spots = []

    spots.append("(" + str(row) + "," + str(0) + ")")
    spots.append("(" + str(row) + "," + str(4) + ")")
    spots.append("(" + str(0) + "," + str(col) + ")")
    spots.append("(" + str(4) + "," + str(col) + ")")
    spots.remove("(" + str(row) + "," + str(col) + ")")
    
    #Corners wll have their own spot twice, so we need an extra delete
    if is_a_corner(row, col):
        spots.remove("(" + str(row) + "," + str(col) + ")")

    return spots

def get_all_moves(board, playing_as, recursive_counter):
    possible_moves = {}

    for x in EDGES_OF_THE_BOARD:
        pickup_row, pickup_col = str_to_int_spot_data(x)
        spot_contents = board[pickup_row][pickup_col]

        if (spot_contents == " " or spot_contents == playing_as):
            pickup_score = 0
            pickup_reasoning = ""
            pickup_score, pickup_reasoning = score_pickup(spot_contents, playing_as, pickup_row, pickup_col, pickup_reasoning)

            for spot in get_placements(pickup_row, pickup_col):
                placement_row, placement_col = str_to_int_spot_data(spot)
                placement_score = 0
                placement_reasoning = ""
                placement_score, placement_reasoning = score_placement(board, playing_as, pickup_row, pickup_col, placement_row, placement_col, placement_reasoning, recursive_counter)

                combined_move = x + " " + spot
                combined_move = combined_move.replace("(" , "<") #Integration with Unity
                combined_move = combined_move.replace(")" , ">") #Integration with Unity

                possible_moves[combined_move] = [(pickup_score + placement_score), (pickup_reasoning + placement_reasoning)]

    return possible_moves

#NEVER EXPOSE AN UKTAKEN PIECE?? Its building a streak instead
#Too many same moves in a row make a change (last 3 or some idk)
#Reward for pushing a piece into the middle?

#UNITTESTS SO ITS EASY MAYBE?

def request_ai_move(board, playing_as):
    possible_moves = get_all_moves(board, playing_as, 0)
    best_moves = [move for move, score in possible_moves.items() if score == max(possible_moves.values())]
    random_best_move = random.choice(best_moves)
    spot_data = random_best_move.split(" ")
 
    if (BUILD_OUTPUT_DATA_ON):
        for key, value in sorted(possible_moves.items(), key=lambda item: item[1]):
            print(f'{key}: {value}')
        print("\n" + str(spot_data) + "\n")

    return spot_data[0], spot_data[1]