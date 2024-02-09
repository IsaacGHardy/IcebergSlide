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

def check_win_conditions():
    streaks = []
    #Create a list
    #5 Rows
    #5 cols
    #2 diagonals
    #Store the max streaks and return them
    return streaks

#Need a way to take a board state, 
#see the biggest line in a row and give move post move analysis and see if the streak shrunk or 
#there is no way to win
def analyze_board(board, team_looking_at):
    streaks = []
    
    for row in range(0,4):
        for col in range(0,4):
            if (board[row][col] == team_looking_at):
                streak_for_piece = 1
                #get direction, maybe get the connections, then check in that row how many are aligned, check connections wont work
                    #because you can have an isolated one
                #check for connection has to check in a line for piece connections
                #While still connecting and same direction
                    #Add one to streakforpiece
                #if streakfor piece is more than 3 add it to streaks

    #Maybe just check for 4s?\
    #maybe just check for 3s
    #build streak

    streaks.sort()
    return streaks[-1]

def score_pickup(spot_contains, playing_as, spot_row, spot_col):
    pickup_score = 0

    if (spot_contains == " "):
       pickup_score += 25
    #Needs to add a score amount for brekaing up a 4
        #make sure dont create a kill scenario when breaking a 4
            #pickup_score -= 100
       #pickup_score += 50
    #Will build a line for team
        #pickup_score += 10
    #Will break a line for opponent
        #pickup_score += 5
       
    return pickup_score

def score_placement(spot_contains, playing_as, spot_row, spot_col):
    reward_for_untaken, reward_for_corner, reward_for_middle = 0, 0, 0

    if (is_a_corner(spot_row, spot_col)):
        reward_for_corner = 10
    #Will push a piece closer to the middle
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

'''
board = [ ["O"] * 5 ] * 5
print(check_for_connections(board, 3, 2, "O"))
board = [ ["X"] * 5 ] * 5
print(check_for_connections(board, 4, 1, "O"))
'''

'''
def check_for_connections(board, spot_row, spot_col, team):
    connections = []
    for row in range(-1, 2, 1):
        for col in range(-1, 2, 1):
            if (spot_row + row > -1 and spot_row + row < 5 and spot_col + col > -1 and spot_col + col < 5):
                if (board[spot_row + row][spot_col + col] == team):
                    connections.append("(" + str(spot_row + row) + "," + str(spot_col + col) + ")")
    return connections
'''