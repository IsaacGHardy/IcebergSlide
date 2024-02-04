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

def check_for_connections(board, spot_row, spot_col, team):
    connections = []
    for row in range(-1, 2, 1):
        for col in range(-1, 2, 1):
            if (spot_row + row > -1 and spot_row + row < 5 and spot_col + col > -1 and spot_col + col < 5):
                if (board[spot_row + row][spot_col + col] == team):
                    connections.append("(" + str(spot_row + row) + "," + str(spot_col + col) + ")")
    return connections

board = [ ["O"] * 5 ] * 5
print(check_for_connections(board, 3, 2, "O"))
board = [ ["X"] * 5 ] * 5
print(check_for_connections(board, 4, 1, "O"))

#Need a function that takes a desired team, a board state, and returns how many in a row the opp will have
def analyze_board(board, team_looking_at):
    #for x in range():
        #Check for connections

    #You look at opp piece
    #You see how many connect
    #if streak gets to 3 or more
    #add it
    return True


def score_pickup(spot_contains, playing_as, spot_row, spot_col):
    reward_for_untaken, reward_for_corner, reward_for_middle = 0, 0, 0

    if (spot_contains == " "):
       reward_for_untaken = 25
    #Needs to add a score amount for brekaing up a 4
        #make sure dont create a kill scenario when breaking a 4
    #Will build a line for team
    #Will break a line for opponent
    
    return reward_for_untaken + reward_for_corner + reward_for_middle

def score_placement(spot_contains, playing_as, spot_row, spot_col):
    reward_for_untaken, reward_for_corner, reward_for_middle = 0, 0, 0

    if (is_a_corner(spot_row, spot_col)):
        reward_for_corner = 10
    #Will push a piece closer to the middle
    #will fork
    #Will bait
    reward_for_middle = (2 - distance_from_center(spot_row, spot_col)) * 15

    return reward_for_corner

def get_all_moves(board, EDGES_OF_THE_BOARD, playing_as, mode, pickup_spot):
    possible_pick_ups = {}
    for x in EDGES_OF_THE_BOARD:
        spot_data = x[1:4].split(",")
        spot_row, spot_col = int(spot_data[0]), int(spot_data[1])

        if (mode == "Picking"):
            if (board[spot_col][spot_row] == " " or board[spot_col][spot_row] == playing_as):
                possible_pick_ups[x] = score_pickup(board[spot_col][spot_row], playing_as, spot_row, spot_col)
        elif (mode == "Placing"):
            if (x != pickup_spot):
                possible_pick_ups[x] = score_placement(board[spot_col][spot_row], playing_as, spot_row, spot_col)
    
    return possible_pick_ups

def request_ai_move(board, EDGES_OF_THE_BOARD, playing_as):
    possible_pick_ups = get_all_moves(board, EDGES_OF_THE_BOARD, playing_as, "Picking", None)
    best_pickup = max(possible_pick_ups.items(), key=lambda item: item[1])[0]

    possible_place_downs = get_all_moves(board, EDGES_OF_THE_BOARD, playing_as, "Placing", best_pickup)
    best_placement = max(possible_place_downs.items(), key=lambda item: item[1])[0]

    return best_pickup, best_placement 