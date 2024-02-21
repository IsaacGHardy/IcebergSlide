from AI import request_ai_move

def chars_to_board(chars):
    grid = [['' for _ in range(5)] for _ in range(5)]

    for i in range(5):
        for j in range(5):
            grid[i][j] = chars[i * 5 + j]

    return grid

#CSharp_input will contain 26 chars, The 1st 25 will be board data, the final char is the team to generate a move for
CSharp_input = input()

team_playing_for = CSharp_input[25:26:1]
board_data = CSharp_input[0:25:1]
generated_board = chars_to_board(board_data)

print(request_ai_move(generated_board, team_playing_for))
