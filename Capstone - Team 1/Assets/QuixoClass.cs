using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;


public struct Point
{
    public int row, col;

    public Point(int r, int c)
    {
        row = r;
        col = c;
    }

    public override string ToString()
    {
        return $"({row}, {col})";
    }
}


public class QuixoClass : MonoBehaviour
{
    public GameObject CubePrefab;
    public GameObject boardObject;
    public int boardSize = 5;
    public QuixoCube[,] gameBoard;
    public bool isXTurn = true;
    public static float cubeSize = 1; // size of cubes
    public static float cubeSep = 0.125f; // separation between cubes
    private static float boardHeight = 0 + cubeSize / 2; // the y coordinate of the cubes in unity
    public float SlideSpeed;
    public bool moveInProgress = false;
    private Point[] corners = { new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1) };
    public int frow, fcol, trow, tcol; // so that I can see the values of from & to in the unity editor
    private Point _from, _to;
    public Point from
    {
        get { return _from; }
        set { _from = value; frow = _from.row; fcol = from.col; }
    }
    public Point to
    {
        get { return _to; }
        set { _to = value; trow = _to.row; tcol = to.col; }
    }
    public List<Point> poss;
    public float spd = 10f;
    public float acceptableOffset = 0.00001f;


    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new QuixoCube[boardSize, boardSize]; 
        for (int r = 0; r < boardSize; ++r)
        {
            for (int c = 0; c < boardSize; ++c)
            {
                Vector3 pos = getPos(r, c);
                GameObject ncube = Instantiate(CubePrefab, pos, Quaternion.identity);
                QuixoCube ncubeScript = ncube.GetComponent<QuixoCube>();
                ncubeScript.Game = boardObject.GetComponent<QuixoClass>();

                ncubeScript.row = r;
                ncubeScript.col = c;
                
                gameBoard[r, c] = ncubeScript;
            }
        }
    }


    // #########################################################################################################################
    // Helper Functions
    // #########################################################################################################################

    // takes a row or column, and translates it into its unity coordinate value
    private float real(float l)
    {
        return l * (cubeSep + cubeSize);
    }


    // Take a point and return the coresponding unity coordinates
    public Vector3 getPos(Point p)
    {
        return new Vector3(real(p.row), boardHeight, real(p.col));
    }
    public Vector3 getPos(float r, float c)
    {
        return new Vector3(real(r), boardHeight, real(c));
    }



    // Check if a given point is on one of the board's corners
    private bool isCorner(Point p)
    {
        if (p.row == 0)
            return p.col == 0 || p.col == boardSize - 1;
        else if (p.row == boardSize - 1) 
            return p.col == 0 || p.col == boardSize - 1;
        return false;
    }
    
    
    
    // returns a function that can be used to check if a pair of ints match the row and column of the given point and returns true or false
    private Func<int, int, bool> PointTester(int r, int c)
    {
        return (row, col) => r == row && c == col;
    }
    private Func<Point, bool> PointTester(Point point)
    {
        return (Point p) => point.row == p.row && point.col == p.col;
    }



    // returns the GameObject associated with a point on the board
    public GameObject Cube(Point p)
    {
        return gameBoard[p.row, p.col].cube;
    }
    public GameObject Cube(int row, int col)
    {
        return gameBoard[row, col].cube;
    }
    public GameObject Cube(QuixoCube p)
    {
        return p.cube;
    }


    public QuixoCube Data(int row, int col)
    {
        return gameBoard[row, col];
    }
    public QuixoCube Data(Point p)
    {
        return gameBoard[p.row, p.col];
    }
    public QuixoCube Data(GameObject p)
    {
        return p.GetComponent<QuixoCube>();
    }

    private Vector3 prepSlide()
    {
        if (from.row == to.row)
        {
            Data(from).row = to.row;
            Data(from).col = to.col == 0 ? -1 : boardSize;
            return new Vector3(real(to.row), boardHeight, real(to.col == 0 ? -1 : boardSize));
        }
        else
        {
            Data(from).row = to.row == 0 ? -1 : boardSize;
            Data(from).col = to.col;
            return new Vector3(real(to.row == 0 ? -1 : boardSize), boardHeight, real(to.col));
        }
    }

    private bool bounds(Point p)
    {
        return p.row >= 0 && p.row < boardSize && p.col >= 0 && p.col < boardSize;
    }
    private bool bounds(QuixoCube p)
    {
        return p.row >= 0 && p.row < boardSize && p.col >= 0 && p.col < boardSize;
    }
    private bool bounds(int r, int c)
    {
        return r >= 0 && r < boardSize && c >= 0 && c < boardSize;
    }

    private Func<Point, QuixoCube> getNextToSlide()
    {
        if (from.row == to.row)
        {
            int c = to.col > from.col ? 1 : -1;
            return (Point cur) =>
            {
                if (bounds(cur.row, cur.col + c))
                    return Data(cur.row, cur.col + c);
                return null;
            };
        }
        else
        {
            int r = to.row > from.row ? 1 : -1;
            return (Point cur) =>
            {
                if (bounds(cur.row + r, cur.col))
                    return Data(cur.row + r, cur.col);
                return null;
            };
        }
        
    }

    private List<QuixoCube> getCubesToSlide()
    {
        List<QuixoCube> toslide = new List<QuixoCube>();
        Point cur = from;

        UnityEngine.Debug.Log($"Plan To Slide ({cur.row},{cur.col})");
        UnityEngine.Debug.Log($"TO: ({to.row},{to.col})");
        toslide.Add(Data(cur));

        var getNextCube = getNextToSlide();
        QuixoCube nextCube = getNextCube(cur);
        while (nextCube != null && bounds(nextCube))
        {
            toslide.Add(nextCube);
            //UnityEngine.Debug.Log($"Plan To Slide ({nextCube.row},{nextCube.col})");
            cur = nextCube.loc();
            nextCube = getNextCube(cur);
        }
        string log = "Plan To Slide: ";
        for (int i = 0; i < toslide.Count; i++)
        {
            log += $"({toslide[i].row},{toslide[i].col})";
            if (i < toslide.Count - 1) { log += ", "; } // Add a comma unless this is the last element
        } 
        UnityEngine.Debug.Log(log);
        return toslide;
    }


    private void dest(QuixoCube cube)
    {
        if (from.row == to.row)
        {
            int c = to.col < from.col ? 1 : -1;
            cube.toPoint = (new Point(cube.row, cube.col + c));
        }
        else
        {
            int r = to.row < from.row ? 1 : -1;
            cube.toPoint = (new Point(cube.row + r, cube.col));
        }
        
    }

    private IEnumerator doSlide(List<QuixoCube> toSlide)
    {

        bool moveDone = false;

        foreach (QuixoCube qcube in toSlide)
        {
            dest(qcube);
        }
        while (!moveDone)
        {
            moveDone = true; 

            foreach (QuixoCube cube in toSlide)
            {

                cube.step(spd);

                if (cube.dist() >= acceptableOffset) { moveDone = false; }

            }

            yield return null; 
        }
        finalizeMove(toSlide);

    }

    private void finalizeMove(List<QuixoCube> toSlide)
    {
        QuixoCube[,] tempBoard = new QuixoCube[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                tempBoard[i, j] = gameBoard[i, j];
            }
        }

        foreach (QuixoCube cube in toSlide)
        {
            cube.snap();
            tempBoard[cube.row, cube.col] = cube;
        }
        gameBoard = tempBoard;


        from = new Point(-1, -1);
        to = new Point(-1, -1); 
        moveInProgress = false; 
        isXTurn = !isXTurn; 
    }

    // #########################################################################################################################
    // functions composed by Isaac Hardy, and slightly modified to fit with c# and Unity by Caleb Merroto
    // #########################################################################################################################

    public bool canPickPiece(int row, int col)
    {
        if (bounds(row, col))
        {
            if (isXTurn)
            {
                if (gameBoard[row, col].face == 'X' || gameBoard[row,col].face == '_')
                {
                    return true;
                }
                else
                {
                    UnityEngine.Debug.Log($"({row},{col}) is owned by the other player, please select a different cube.");
                    return false;
                }
                
            }
            else 
            {
                if (gameBoard[row, col].face == 'O' || gameBoard[row, col].face == '_')
                {
                    return true;
                }
                else
                {
                    UnityEngine.Debug.Log($"({row},{col}) is owned by the other player, please select a different cube.");
                    return false;  
                }
            }
        }
        UnityEngine.Debug.Log($"({row},{col}) is not a valid move, please select a cube on the edge of the board.");
        return false;
    }
    //gets the moves for a corner piece
    public List<Point> getCornerMoves(int row, int col)
    {
        List<Point> possible = new List<Point>();
        if (row == 0 && col == 0)
        {
            possible.Add(new Point(0, boardSize - 1 ));
            possible.Add(new Point(boardSize - 1, 0 ));
        }
        else if (row == 0 && col == boardSize - 1)
        {
            possible.Add(new Point(0, 0 ));
            possible.Add(new Point(boardSize - 1, boardSize - 1 ));
        }
        else if (row == boardSize - 1 && col == 0)
        {
            possible.Add(new Point(0, 0 ));
            possible.Add(new Point(boardSize - 1, boardSize - 1 ));
        }
        else if (row == boardSize - 1 && col == boardSize - 1)
        {
            possible.Add(new Point(0, boardSize - 1 ));
            possible.Add(new Point(boardSize - 1, 0 ));
        }
        return possible;
    }
    //gets the moves for a middle piece
    public List<Point> getMiddleMoves(int row, int col)
    {
        List<Point> possible = new List<Point>();
        if (row == 0)
        {
            possible.Add(new Point(0, 0 ));
            possible.Add(new Point(0, boardSize - 1 ));
            possible.Add(new Point(boardSize - 1, col ));
        }
        else if (row == boardSize - 1)
        {
            possible.Add(new Point(boardSize - 1, 0 ));
            possible.Add(new Point(boardSize - 1, boardSize - 1 ));
            possible.Add(new Point(0, col ));
        }
        else if (col == 0)
        {
            possible.Add(new Point(0, 0 ));
            possible.Add(new Point(boardSize - 1, 0 ));
            possible.Add(new Point(row, boardSize - 1 ));
        }
        else if (col == boardSize - 1)
        {
            possible.Add(new Point(boardSize - 1, boardSize - 1 ));
            possible.Add(new Point(0, boardSize - 1 ));
            possible.Add(new Point(row, 0 ));
        }
        return possible;
    }




    // #########################################################################################################################
    // functions entirely re-written by Caleb Merroto
    // #########################################################################################################################
    

    //returns list of all possible moves based off given piece selected to move, assumes piece has already been checked to make sure it is a valid move
    public List<Point> GetPossibleMoves()
    {
        List<Point> possible = new List<Point>();
        int row = from.row; 
        int col = from.col;
        int m = boardSize - 1;                      // max row or col value


        int r = (row == 0) ? m : 0;                 // opposite row
        int c = (col == 0) ? m : 0;                 // opposite col


        if (isCorner(from))
        {
            possible.Add(new Point(r, col));
            possible.Add(new Point(row, c));
        }
        else
        {
            if (row == 0 || row == m)
            {
                possible.Add(new Point(r, col));
                possible.Add(new Point(row, 0));
                possible.Add(new Point(row, m));
            }
            else
            {
                possible.Add(new Point(row, c));
                possible.Add(new Point(0, col));
                possible.Add(new Point(m, col));
            }
            
        }
        string log = $"Valid Moves from ({from.row},{from.col}): ";
        for (int i = 0; i < possible.Count; i++)
        {
            log += $"({possible[i].row},{possible[i].col})";
            if (i < possible.Count - 1) { log += ", "; } // Add a comma unless this is the last element
        }
        UnityEngine.Debug.Log(log);
        return possible;
    }


    public bool IsValidMove()
    {
        var test = PointTester(to); // tests if a point (informally represented by a pair of ints) is equal to the test point (improves readability)

        for (int i = 0; i < poss.Count; i++)
        {
            if (test(poss[i])) { return true;  }
        }
        return false;
    }




    //moves the peices based on move made
    public void makeMove()
    {
        char blockVal = isXTurn ? 'X' : 'O';

        Cube(from).SetActive(true);
        
        Data(from).Face(blockVal);

        List<QuixoCube> cubes = getCubesToSlide();
        //UnityEngine.Debug.Log($"Cubes to slide: {cubes.Count}");

        Cube(from).transform.position = prepSlide();


        StartCoroutine(doSlide(cubes));

        

        UnityEngine.Debug.Log($"Move complete! ({from.row},{from.col}) >> ({to.row},{to.col})");

    }

}





//    //gets the piece to be moved
//    Point getMoveFrom()
//    {
//        Point pos;
//        cin >> row >> col;
//        return pos;
//    }

//    //gets the place where the piece will be moved to
//    Point getMoveTo()
//    {
//        Point pos;
//        cin >> row >> col;
//        return pos;
//    }

//    //prints a list of possible moves given a certain piece selection
//    void outputPossibleMoves(const List<Point> &possible) {
//		for (int i = 0; i<possible.size(); ++i) {
//			cout << "(" << possible[i].row << "," << possible[i].col << ")\n";
//		}
//cout << endl;
//	}




//	//uses the List of possible moves to make sure the move inputed is valid
//	bool isValidMove(const List<Point> &possible, Point moveTo)
//{
//    bool valid = false;
//    for (int i = 0; i < possible.size(); ++i)
//    {
//        if (possible[i].row == moveTo.row && possible[i].col == moveTo.col)
//        {
//            valid = true;
//        }
//    }
//    return valid;
//}

////moves the peices based on move made
//void redrawBoard(Point moveFrom, Point moveTo)
//{
//    Block newValue = isXTurn ? Block::X : Block::O;
//    //slide up
//    if (moveTo.row > moveFrom.row)
//    {
//        for (int i = moveFrom.row; i < moveTo.row; ++i)
//        {
//            gameBoard[i][moveFrom.col] = gameBoard[i + 1][moveTo.col];
//        }
//        gameBoard[moveTo.row][moveTo.col] = newValue;
//    }
//    //slide down
//    else if (moveTo.row < moveFrom.row)
//    {
//        for (int i = moveFrom.row; i > moveTo.row; --i)
//        {
//            gameBoard[i][moveFrom.col] = gameBoard[i - 1][moveTo.col];
//        }
//        gameBoard[moveTo.row][moveTo.col] = newValue;
//    }
//    //slide left
//    else if (moveTo.col > moveFrom.col)
//    {
//        for (int i = moveFrom.col; i < moveTo.col; ++i)
//        {
//            gameBoard[moveFrom.row][i] = gameBoard[moveFrom.row][i + 1];
//        }
//        gameBoard[moveTo.row][moveTo.col] = newValue;
//    }
//    //slide right
//    else if (moveTo.col < moveFrom.col)
//    {
//        for (int i = moveFrom.col; i > moveTo.col; --i)
//        {
//            gameBoard[moveFrom.row][i] = gameBoard[moveFrom.row][i - 1];
//        }
//        gameBoard[moveTo.row][moveTo.col] = newValue;
//    }
//}

////helper function to print one piece
//void printPiece(Block block)
//{
//    if (block == NONE)
//    {
//        cout << '#';
//    }
//    else if (block == X)
//    {
//        cout << 'X';
//    }
//    else
//    {
//        cout << 'O';
//    }
//}

////prints the entire board
//void printBoard()
//{
//    for (int i = 0; i < size; ++i)
//    {
//        for (int j = 0; j < size; ++j)
//        {
//            if (j == size - 1)
//            {
//                printPiece(gameBoard[i][j]);
//                cout << endl;
//            }
//            else
//            {
//                printPiece(gameBoard[i][j]);
//            }
//        }
//    }
//    cout << endl;
//}

//public:

//	//basically a constructor to initiate an empty 2D board
//	void createBoard(int size)
//{
//    this->size = size;
//    gameBoard.resize(size);
//    for (int i = 0; i < size; ++i)
//    {
//        gameBoard[i].resize(size);
//        for (int j = 0; j < size; ++j)
//        {
//            gameBoard[i][j] = Block::NONE;
//        }
//    }
//    /*gameBoard[0] = { O, NONE, NONE, NONE, NONE };
//    gameBoard[1] = { NONE, NONE, NONE, NONE, NONE };
//    gameBoard[2] = { NONE, NONE, NONE, NONE, NONE };
//    gameBoard[3] = { NONE, NONE, NONE, NONE, NONE };
//    gameBoard[4] = { NONE, NONE, NONE, NONE, NONE };*/

//}

////holds all the logic for getting a move, checking its validity, printing possible moves, and redrawing the board
////this is basically the entire game
//void makeMove()
//{

//    printBoard();
//    cout << "What piece would you like to move? Insert two space delimeted ints for Row Column 0 based.\n";

//    Point moveFrom = getMoveFrom();
//    while (!canPickPiece({ moveFrom.row, moveFrom.col })) {
//    cout << "You can't move that piece. Pick another one.\n";
//    moveFrom = getMoveFrom();
//}

//List<Point> possible = getPossibleMoves({moveFrom.row, moveFrom.col});
//cout << "Your possible moves are: \n";
//outputPossibleMoves(possible);

//cout << "Insert your move\n";
//Point moveTo = getMoveTo();
//while (!isValidMove(possible, moveTo))
//{
//    cout << "That is an invalid move. Pick another one.\n";
//    moveTo = getMoveTo();
//}

//redrawBoard(moveFrom, moveTo);
//isXTurn = !isXTurn;

//	}

//	//checks to see if a player has won, first checks rows and columns then the diagonals
//	List<Winner> checkForWinner()
//{
//    List<Winner> winners;

//    //check rows and cols
//    for (int i = 0; i < size; ++i)
//    {
//        bool possibleRowWinner = true;
//        bool possibleColWinner = true;
//        Block block = gameBoard[i][i];
//        if (block != NONE)
//        {
//            for (int j = 0; j < size; ++j)
//            {
//                //check rows
//                if (possibleRowWinner && gameBoard[i][j] != block)
//                {
//                    possibleRowWinner = false;
//                }
//                //check cols
//                if (possibleColWinner && gameBoard[j][i] != block)
//                {
//                    possibleColWinner = false;
//                }
//            }
//            if (possibleRowWinner || possibleColWinner)
//            {
//                if (block == X)
//                {
//                    winners.push_back(TEAMX);
//                }
//                else
//                {
//                    winners.push_back(TEAMO);
//                }
//            }
//        }
//    }

//    //check diagonals
//    Block topLeft = gameBoard[0][0];
//    Block bottomLeft = gameBoard[size - 1][0];
//    if (topLeft != NONE || bottomLeft != NONE)
//    {
//        bool possibleWinnerTop = false;
//        bool possibleWinnerBottom = false;
//        if (topLeft != NONE)
//        {
//            possibleWinnerTop = true;
//        }
//        if (bottomLeft != NONE)
//        {
//            possibleWinnerBottom = true;
//        }
//        for (int i = 0; i < size; ++i)
//        {
//            if (topLeft != NONE && possibleWinnerTop && gameBoard[i][i] != topLeft)
//            {
//                possibleWinnerTop = false;
//            }
//            if (bottomLeft != NONE && possibleWinnerBottom && gameBoard[size - i - 1][i] != bottomLeft)
//            {
//                possibleWinnerBottom = false;
//            }
//        }

//        if (possibleWinnerTop)
//        {
//            if (topLeft == X)
//            {
//                winners.push_back(TEAMX);
//            }
//            else
//            {
//                winners.push_back(TEAMO);
//            }
//        }
//        else if (possibleWinnerBottom)
//        {
//            if (bottomLeft == X)
//            {
//                winners.push_back(TEAMX);
//            }
//            else
//            {
//                winners.push_back(TEAMO);
//            }
//        }

//    }

//    return winners;

//}

////Prints the board and determines if there was one winner or a draw and outputs the results
//void outputWinner(const List<Winner> &winners)
//{

//    printBoard();

//    if (winners.size() == 1)
//    {
//        if (winners[0] == TEAMX)
//        {
//            cout << "Team X won.\n";
//        }
//        else
//        {
//            cout << "Team O won.\n";
//        }
//    }
//    else if (winners.size() > 1)
//    {
//        bool oneWinner = true;
//        for (int i = 0; i < winners.size(); ++i)
//        {
//            if (winners[i] != winners[0])
//            {
//                oneWinner = false;
//            }
//        }
//        if (oneWinner)
//        {
//            if (winners[0] == TEAMX)
//            {
//                cout << "Team X won.\n";
//            }
//            else
//            {
//                cout << "Team O won.\n";
//            }
//        }
//        else
//        {
//            cout << "Draw.\n";
//        }
//    }
//}

//};

//int main()
//{
//    Quixo quixo;
//    quixo.createBoard(5);

//    List<Winner> winners;

//    winners = quixo.checkForWinner();
//    while (winners.size() == 0)
//    {
//        quixo.makeMove();
//        winners = quixo.checkForWinner();
//    }
//    quixo.outputWinner(winners);

//}
//Collapse







