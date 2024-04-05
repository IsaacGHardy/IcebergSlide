from AI import request_ai_move

#Building Towards Middle but dont get too much crazy
row1 = ["O", "O", "O", " ", " "]
row2 = [" ", " ", " ", " ", " "]
row3 = [" ", " ", " ", " ", " "]
row4 = [" ", " ", " ", " ", " "]
row5 = ["X", "X", "X", "X", " "]
start_board = [row1, row2, row3, row4, row5]
request_ai_move(start_board, "O", 0)