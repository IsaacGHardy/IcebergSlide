from AI import request_ai_move, chars_to_board

#28 Chars Incoming
#The first 25 chars are the board
#The 26th is what player the move is for
#The 27th is the difficulty (0 Hardest - 5 Silly)
#The 28th tells if we are in the tutorial (0 is not, 1 is hard, 2 is a silly tutorial move)

#CSharp_input = "X                        O00" #Test Data
CSharp_input = input()

#Breaking down the incoming data
board_data = CSharp_input[0:25:1]
generated_board = chars_to_board(board_data)
team_playing_for = CSharp_input[25:26:1]
difficulty = int(CSharp_input[26:27:1])
tutorial = int(CSharp_input[27:28:1])

if (tutorial == 0):
    #Normal Gameplay Move
    print(request_ai_move(generated_board, team_playing_for, difficulty))
elif (tutorial == 1):
    #Hard Move For Tutorial
    print(request_ai_move(generated_board, team_playing_for, 0))
elif (tutorial == 2):
    #Easy Move For Tutorial
    print(request_ai_move(generated_board, team_playing_for, 7))
