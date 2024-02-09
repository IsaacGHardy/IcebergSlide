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
using System.Security.Permissions;
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
    //####################################################################################################################################
    //# Unity Objects ####################################################################################################################
    //####################################################################################################################################
    public GameObject CubePrefab;
    public GameObject boardObject;

    //####################################################################################################################################
    //# Game Constants ###################################################################################################################
    //####################################################################################################################################
    public const int boardSize = 5;                         // the number of rows & cols in the board
    public const float cubeSize = 1;                 // size of cubes
    public const float cubeSep = 0.0f;               // separation between cubes
    public const float spd = 10f;                    // the speed at which penguins move
    public const float acceptableOffset = 0.00001f;  // how far a penguin can be off from its target at the end of a slide
    private const float boardHeight = cubeSize / 2;  // the y coordinate of the cubes in unity

    //####################################################################################################################################
    //# Game Logic Variables #############################################################################################################
    //####################################################################################################################################
    public QuixoCube[,] gameBoard;
    public Point from, to;
    public List<Point> poss;
    public bool isXTurn = true;
    public bool moveInProgress = false;
    public bool gameOver = false;



    //####################################################################################################################################
    //# Initialization ###################################################################################################################
    //####################################################################################################################################
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
                ncubeScript.cube = ncube;

                ncubeScript.row = r;
                ncubeScript.col = c;
                
                gameBoard[r, c] = ncubeScript;
            }
        }
    }


    //####################################################################################################################################
    //# Boolean Checks ###################################################################################################################
    //####################################################################################################################################
    

    // checks if a point is within the bounds of the board
    private bool bounds(Point p)
    {
        return p.row >= 0 && p.row<boardSize && p.col >= 0 && p.col<boardSize;
    }
    private bool bounds(QuixoCube p)
    {
        return p.row >= 0 && p.row < boardSize && p.col >= 0 && p.col < boardSize;
    }
    private bool bounds(int r, int c)
    {
        return r >= 0 && r < boardSize && c >= 0 && c < boardSize;
    }

    // returns a function that can be used to check if a pair of ints match the row and column of the given point and returns true or false
    private Func<int, int, bool> PointTester(int r, int c) { return (row, col) => r == row && c == col; }
    private Func<Point, bool> PointTester(Point p) { return (Point np) => np.row == p.row && np.col == p.col; }

    // Checks if a given point is on one of the board's corners
    private bool isCorner(Point p) { return isCorner(p.row, p.col); }
    private bool isCorner(int row, int col)
    {
        if (row == 0)
            return col == 0 || col == boardSize - 1;
        else if (row == boardSize - 1)
            return col == 0 || col == boardSize - 1;
        return false;
    }

    // Checks if a given cube is valid for starting a move:
    public bool canPickPiece(int row, int col)
    {
        if (gameOver)
        {
            UnityEngine.Debug.Log($"The Game Is Over! No more moves can be made!");
            return false;
        }
        if (bounds(row, col))
        {
            if ((row == 0 || row == boardSize - 1) || (col == 0 || col == boardSize - 1))
            {
                if (isXTurn)
                {
                    if (gameBoard[row, col].face == 'X' || gameBoard[row, col].face == '_' || moveInProgress)
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
                    if (gameBoard[row, col].face == 'O' || gameBoard[row, col].face == '_' || moveInProgress)
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
        }
        UnityEngine.Debug.Log($"({row},{col}) is not a valid move, please select a cube on the edge of the board.");
        return false;
    }


    //####################################################################################################################################
    //# Position Calculation #############################################################################################################
    //####################################################################################################################################


    // takes a row or column, and translates it into its unity coordinate value
    private float real(float l) { return l * (cubeSep + cubeSize); }

    // Take a point and return the coresponding unity coordinates
    public Vector3 getPos(Point p)
    {
        return getPos(p.row, p.col);
    }
    public Vector3 getPos(float r, float c)
    {
        Vector3 root = boardObject.transform.position;
        return new Vector3(root.x - real(r), boardHeight, root.z + real(c));
    }


    //####################################################################################################################################
    //# Value Accessors ##################################################################################################################
    //####################################################################################################################################


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

    // returns the QuixoCube associated with a point on the board, or a GameObject
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

    // returns a functor that takes a point, and returns a Quixo cube that represents the next cube to slide after the one at the point
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


    //#####################################################################################################################################
    //# Primary Pre-Move Functions ########################################################################################################
    //#####################################################################################################################################


    //gets the moves for a corner piece
    public List<Point> getCornerMoves(int row, int col)
    {
        List<Point> possible = new List<Point>();
        if (row == 0 && col == 0)
        {
            possible.Add(new Point(0, boardSize - 1));
            possible.Add(new Point(boardSize - 1, 0));
        }
        else if (row == 0 && col == boardSize - 1)
        {
            possible.Add(new Point(0, 0));
            possible.Add(new Point(boardSize - 1, boardSize - 1));
        }
        else if (row == boardSize - 1 && col == 0)
        {
            possible.Add(new Point(0, 0));
            possible.Add(new Point(boardSize - 1, boardSize - 1));
        }
        else if (row == boardSize - 1 && col == boardSize - 1)
        {
            possible.Add(new Point(0, boardSize - 1));
            possible.Add(new Point(boardSize - 1, 0));
        }
        return possible;
    }

    //gets the moves for a middle piece
    public List<Point> getMiddleMoves(int row, int col)
    {
        List<Point> possible = new List<Point>();
        if (row == 0)
        {
            possible.Add(new Point(0, 0));
            possible.Add(new Point(0, boardSize - 1));
            possible.Add(new Point(boardSize - 1, col));
        }
        else if (row == boardSize - 1)
        {
            possible.Add(new Point(boardSize - 1, 0));
            possible.Add(new Point(boardSize - 1, boardSize - 1));
            possible.Add(new Point(0, col));
        }
        else if (col == 0)
        {
            possible.Add(new Point(0, 0));
            possible.Add(new Point(boardSize - 1, 0));
            possible.Add(new Point(row, boardSize - 1));
        }
        else if (col == boardSize - 1)
        {
            possible.Add(new Point(boardSize - 1, boardSize - 1));
            possible.Add(new Point(0, boardSize - 1));
            possible.Add(new Point(row, 0));
        }
        return possible;
    }

    //returns list of all possible moves based off given piece selected to move,
    //assumes cube has already been checked to make sure it is a valid move
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

    // tests if the current to and from points constitute a valid move
    public bool IsValidMove()
    {
        var test = PointTester(to);

        for (int i = 0; i < poss.Count; i++)
        {
            if (test(poss[i])) { return true; }
        }
        return false;
    }


    //####################################################################################################################################
    //# Primary Move Functions ###########################################################################################################
    //####################################################################################################################################


    // Moves a cube into position to start a move animation
    private Vector3 prepSlide()
    {
        if (from.row == to.row)
        {
            Data(from).row = to.row;
            Data(from).col = to.col == 0 ? -1 : boardSize;
            return getPos(to.row, to.col == 0 ? -1 : boardSize);
        }
        else
        {
            Data(from).row = to.row == 0 ? -1 : boardSize;
            Data(from).col = to.col;
            return getPos(to.row == 0 ? -1 : boardSize, to.col);
        }
    }

    // Returns a list of all of the cubes that will need to be involved in the current move's animation
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

    // Animates the move
    private IEnumerator doSlide(List<QuixoCube> toSlide)
    {

        bool moveDone = false;

        foreach (QuixoCube cube in toSlide)
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
    
    // Ties up any loose ends so that everything is in place for the next move
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

    // checks various win / tie conditions to see if the game is over
    private List<string> doWinCheck()
    {
        List<string> EndConditions = new List<string>();
        bool stalemate = true;

        // Check first diagonal
        bool diagonal1 = true;
        char test = gameBoard[0, 0].face;
        for (int i = 0; i < boardSize; i++)
        {
            if (gameBoard[i, i].face != test || gameBoard[i, i].face == '_')
            {
                diagonal1 = false;
                break;
            }
        }

        // Check second diagonal
        bool diagonal2 = true;
        test = gameBoard[0, boardSize - 1].face;
        for (int i = 0; i < boardSize; i++)
        {
            if (gameBoard[i, boardSize - 1 - i].face != test || gameBoard[i, boardSize - 1 - i].face == '_')
            {
                diagonal2 = false;
                break;
            }
        }

        if (diagonal1 && test != '_') { EndConditions.Add($"diagonal1{test}"); gameOver = true; }
        if (diagonal2 && test != '_') { EndConditions.Add($"diagonal2{test}"); gameOver = true; }



        for (int i = 0; i < boardSize; i++)
        {
            if (checkRow(i)) { EndConditions.Add($"row{i}{gameBoard[i, 0].face}"); gameOver = true; }
            if (checkCol(i)) { EndConditions.Add($"col{i}{gameBoard[0, i].face}"); gameOver = true; }
        }

        // Check for stalemate
        if (stalemate) { EndConditions.Add("stalemate"); gameOver = true; }

        // Functions to check rows and columns
        bool checkRow(int r)
        {
            char t = gameBoard[r, 0].face;
            for (int i = 0; i < boardSize; i++)
            {
                if (gameBoard[r, i].face == '_') { stalemate = false; return false; }// Adjust stalemate check
                if (gameBoard[r, i].face != t) return false;
            }
            return true;
        }

        bool checkCol(int c)
        {
            char t = gameBoard[0, c].face;
            if (t == '_') return false;
            for (int i = 0; i < boardSize; i++)
            {
                if (gameBoard[i, c].face != t) return false;
            }
            return true;
        }


        return EndConditions;
    }
    
    // uses the other primary move functions to carry out a move in its entirety
    public void makeMove(Point From, Point To)
    {
        from = From; 
        to = To;
        if (IsValidMove())
        {
            makeMove();
        }
    }
    public void makeMove()
    {
        char blockVal = isXTurn ? 'X' : 'O';

        Cube(from).SetActive(true);

        Data(from).Face(blockVal);

        List<QuixoCube> cubes = getCubesToSlide();
        //UnityEngine.Debug.Log($"Cubes to slide: {cubes.Count}");



        Cube(from).transform.position = prepSlide();
        StartCoroutine(doSlide(cubes));



        List<string> endCheck = doWinCheck();



        UnityEngine.Debug.Log($"Move complete! ({from.row},{from.col}) >> ({to.row},{to.col})");

    }


}