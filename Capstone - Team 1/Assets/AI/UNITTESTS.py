from AI import request_ai_move

#Init
tests_passed = {}
tests_failed = {}

#Building Towards Middle but dont get too much crazy
row1 = ["O", "O", "X", "O", "O"]
row2 = ["X", " ", "X", " ", "X"]
row3 = ["O", "O", "X", " ", "O"]
row4 = ["O", "X", " ", "X", "O"]
row5 = ["O", "X", "O", "X", "X"]
start_board = [row1, row2, row3, row4, row5]
request_ai_move(start_board, "X")