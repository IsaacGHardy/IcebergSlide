from AI import request_ai_move

#Init
tests_passed = {}
tests_failed = {}

#Building Towards Middle but dont get too much crazy
row1 = ["X", "O", "O", "X", "X"]
row2 = ["O", " ", " ", "O", "O"]
row3 = ["X", " ", " ", "X", "X"]
row4 = ["O", "O", " ", "X", "O"]
row5 = ["X", "X", "O", "X", "X"]
start_board = [row1, row2, row3, row4, row5]
print(request_ai_move(start_board, "O"))
