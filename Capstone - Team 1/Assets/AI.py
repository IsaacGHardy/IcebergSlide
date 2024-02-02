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

#Useless change to see if it goes through