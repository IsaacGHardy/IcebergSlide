#NOTES:
#AI Should always go first as the 3x3 and 4x4 versions of Quixo
#are first player win games, and so are the majority of board games

#START:
#Pickle to save
#numpy
#tensorflow from tensorflow.keras import layers

#Deep Reinforcement Learning or Value Iteration (VI)


#Evaluate a given 5x5 grid
    #Know what piece you are (X or O)
    #Reward more for controlling middle +(25, 15, 5)
    #Control Corners +(20)

#GetBestMove
    #Should take a board, team to move for, and difficulty

#https://neptune.ai/blog/saving-trained-model-in-python#:~:text=To%20save%20a%20deep%20learning,loaded%20later%20to%20make%20predictions.
#https://www.activestate.com/resources/quick-reads/how-to-create-a-neural-network-in-python-with-and-without-keras/

#Quxio but in Python
import numpy

board = [[0]*5]*5

def request_input(prompt, acceptable_responses):
    result = None
    while (result not in acceptable_responses):
        result = input(prompt)
    return result

def print_board():
    for row in board:
        print(row)
    
def check_for_win():
    return {} #Tuple of winners

def start_match():
    current_move = "X"
    has_winner = False
    
    player_count = int(request_input("Would you like 2 or 4 players for this match ", {'2', '4'}))
    x_or_o = request_input("Type your team: X or O ", {'x', 'o', 'X', 'O'})

    while (has_winner):
        #play move 
        #check fo a win
        if ("X" or "O" in check_for_win):
            #Check for the winner and output it
            has_winner = True


start_match()