using System;
using System.Text.RegularExpressions;
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
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

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
    public static readonly Point zero = new Point(0, 0);
    public bool eq(Point p)
    {
        return row == p.row && col == p.col;
    }
}


public class QuixoClass : MonoBehaviour
{
    //####################################################################################################################################
    //# Unity Objects ####################################################################################################################
    //####################################################################################################################################
    public GameObject penguinPrefab;
    public GameObject boardObject;
    public GameObject xhat; // the hat worn by a penguin owned by the x player
    public GameObject ohat; // the hat worn by a penguin owned by the y player
    public Material highlightMat;
    public Material selectedMat;
    public Material defaultMat;
    public AI ai;
    [SerializeField] PhotonView photonView;
    [SerializeField] MenuSounds menuSounds;
    [SerializeField] TextMeshProUGUI playerTurn;
    [SerializeField] Button offerDrawButton;
    [SerializeField] GameObject rejectionMessage;
    [SerializeField] Tutorial tutorial;
    public static string p1Name = "Player 1";
    public static string p2Name = "Player 2";
    private Player[] playerList;


    //####################################################################################################################################
    //# Game Constants ###################################################################################################################
    //####################################################################################################################################
    public const int boardSize = 5;                         // the number of rows & cols in the board
    public const int boardCenter = (boardSize - 1) / 2;     // the centerpoint of the edge of the board
    public const float penguinSpace = 1;                 // amount of space a penguin takes up
    public const float penguinSep = 0.0f;               // separation between penguins
    public float spd = 1f;                    // the speed at which penguins move
    public float trnspd = 2f;                    // the speed at which penguins turn
    public const float acceptableOffset = 0.00001f;  // how far a penguin can be off from its target at the end of a slide
    private const float boardHeight = penguinSpace / 2;  // the y coordinate of the penguins in unity

    //####################################################################################################################################
    //# Game Logic Variables #############################################################################################################
    //####################################################################################################################################
    public Penguin[,] gameBoard;
    public Point from, to;
    public bool isTutorial = true;
    public Point pieceToClick = new Point( -1, -1 );
    public List<Point> poss;
    public bool isXTurn = true;
    public bool moveInProgress = false;
    public bool gameOver = false;
    public bool AIgame = false;
    public bool isOnline = true;
    public static bool isXWin = false;
    public static bool isOWin = false;
    public bool isLocked = false;
    public static bool isPlayer1 = false;
    private string[] movementAnimations = new string[] { "Walk", "Roll", "Walk", "Swim", "Walk", 
                                                       "Spin", "Walk", "Hit", "Walk", "Fly", "Walk"};
    public Point suggestedTo = new Point();
    public Point suggestedFrom = new Point();
    //adding more walks so it is randomly chosen more and others feel special
    private int randMovementIndex = 0;
    private int turncount = 1;
    private bool isQuick = false;



    //####################################################################################################################################
    //# Initialization ###################################################################################################################
    //####################################################################################################################################
    void Start()
    {
        isXWin = false;
        isOWin = false;
        if (isOnline) { playerTurn.text = $"{p1Name}'s turn"; }
        else { playerTurn.text = $"{EndGame.p1Name}'s turn"; }
        if(!isPlayer1)
        {
            isLocked = true;
        }

        if(!isOnline && !isTutorial)
        {
            if (PlayerPrefs.GetInt("quick") == 1)
            {
                spd = 50f;
                isQuick = true;
            }
        }
        //HANDLE FOR ONLINE GAME
        if (isOnline && OnlineCharacterCustomizationUI.XHAT != null && OnlineCharacterCustomizationUI.OHAT != null)
        {
            xhat = OnlineCharacterCustomizationUI.XHAT.gameObject;
            ohat = OnlineCharacterCustomizationUI.OHAT.gameObject;
        }
        else if (CharacterCustomizationUI.XHAT != null && CharacterCustomizationUI.OHAT != null)
        {
            xhat = CharacterCustomizationUI.XHAT.gameObject;
            ohat = CharacterCustomizationUI.OHAT.gameObject;
            AIgame = CharacterCustomizationUI.IS_AI_GAME;
        }

        if (AIgame && !isPlayer1)
        {
            StartCoroutine(aiMove());
        }

        gameBoard = new Penguin[boardSize, boardSize];
        for (int r = 0; r < boardSize; ++r)
        {
            for (int c = 0; c < boardSize; ++c)
            {
                Vector3 pos = getPos(r, c);
                GameObject nPenguin = Instantiate(penguinPrefab, pos, Quaternion.identity);
                Penguin nPenguinScript = nPenguin.GetComponent<Penguin>();

                nPenguinScript.Game = boardObject.GetComponent<QuixoClass>();
                nPenguinScript.penguin = nPenguin;

                nPenguinScript.row = r;
                nPenguinScript.col = c;

                gameBoard[r, c] = nPenguinScript;
            }
        }
        if (isTutorial)
        {
            HighlightSuggestedMove();
        }
    }

    public void startTutorial()
    {
        isPlayer1 = true;
        isLocked = false;
    }

    public void toggleQuick()
    {
        if (!isOnline && !isTutorial)
        {
            if (PlayerPrefs.GetInt("quick") == 1)
            {
                spd = 50f;
                isQuick = true;
            }
            else
            {
                spd = 5f;
                isQuick = false;
            }
        }
    }


    //####################################################################################################################################
    //# Boolean Checks ###################################################################################################################
    //####################################################################################################################################


    // checks if a point is within the bounds of the board
    private bool bounds(Point p)
    {
        return p.row >= 0 && p.row < boardSize && p.col >= 0 && p.col < boardSize;
    }
    private bool bounds(Penguin p)
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

    private int rebound(int i) {
        if (i == -1) return 0;
        else if (i == boardSize) return boardSize - 1;
        else return i;
    }
    private Point rebound(Point p) {
        return new Point(rebound(p.row), rebound(p.col));
    }
    // Checks if a given point is on one of the board's corners
    private bool isCorner(Point p) {
        return isCorner(rebound(p.row), rebound(p.col));
    }
    private bool isCorner(int row, int col)
    {
        row = rebound(row); // if row is out of bounds, bring it back in-bounds
        col = rebound(col); // if col is out of bounds, bring it back in-bounds
        if (row == 0 || row == boardSize - 1) return col == 0 || col == boardSize - 1;
        return false;
    }

    // Checks if a given penguin is valid for starting a move:
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
                        UnityEngine.Debug.Log($"({row},{col}) is owned by the other player, please select a different penguin.");
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
                        //UnityEngine.Debug.Log($"({row},{col}) is owned by the other player, please select a different penguin.");
                        return false;
                    }
                }
            }
        }
        //UnityEngine.Debug.Log($"({row},{col}) is not a valid move, please select a penguin on the edge of the board.");
        return false;
    }


    //####################################################################################################################################
    //# Position Calculation #############################################################################################################
    //####################################################################################################################################


    // takes a row or column, and translates it into its unity coordinate value
    private float real(float l) { return l * (penguinSep + penguinSpace); }

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
    public GameObject Penguin(Point p)
    {
        return gameBoard[p.row, p.col].penguin;
    }
    public GameObject Penguin(int row, int col)
    {
        return gameBoard[row, col].penguin;
    }
    public GameObject Penguin(Penguin p)
    {
        return p.penguin;
    }

    // returns the Penguin associated with a point on the board, or a GameObject
    public Penguin Data(int row, int col)
    {
        return gameBoard[row, col];
    }
    public Penguin Data(Point p)
    {
        return gameBoard[p.row, p.col];
    }
    public Penguin Data(GameObject p)
    {
        return p.GetComponent<Penguin>();
    }

    // returns a functor that takes a point, and returns a penguin that represents the next penguin to slide after the one at the point
    private Func<Point, Penguin> getNextToSlide()
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
    //assumes penguin has already been checked to make sure it is a valid move
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

    public void playPossibleMoves()
    {
        foreach(Point p in poss)
        {
            Data(p).Play("Bounce");
        }
    }

    public void stopPossibleMoves()
    {
        if (poss != null)
        {
            foreach (Point p in poss)
            {
                Data(p).Play("Idle_A");
            }
        }
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


    // Moves a penguin into position to start a move animation
    private void prepSlide()
    {
        if (from.row == to.row)
        {
            Data(from).row = to.row;
            Data(from).col = to.col == 0 ? -1 : boardSize;
            //return getPos(to.row, to.col == 0 ? -1 : boardSize);
        }
        else
        {
            Data(from).row = to.row == 0 ? -1 : boardSize;
            Data(from).col = to.col;
            //return getPos(to.row == 0 ? -1 : boardSize, to.col);
        }
    }

    // Returns a list of all of the penguins that will need to be involved in the current move's animation
    private List<Penguin> getPenguinsToSlide()
    {
        List<Penguin> toslide = new List<Penguin>();
        Point cur = from;

        //UnityEngine.Debug.Log($"Plan To Slide ({cur.row},{cur.col})");
        //UnityEngine.Debug.Log($"TO: ({to.row},{to.col})");
        toslide.Add(Data(cur));

        var getNextPenguin = getNextToSlide();
        Penguin nextPenguin = getNextPenguin(cur);
        while (nextPenguin != null && bounds(nextPenguin))
        {
            toslide.Add(nextPenguin);
            //UnityEngine.Debug.Log($"Plan To Slide ({nextPenguin.row},{nextPenguin.col})");
            cur = nextPenguin.loc();
            nextPenguin = getNextPenguin(cur);
        }
        //string log = "Plan To Slide: ";
        /*for (int i = 0; i < toslide.Count; i++)
        {
            log += $"({toslide[i].row},{toslide[i].col})";
            if (i < toslide.Count - 1) { log += ", "; } // Add a comma unless this is the last element
        } */
        //UnityEngine.Debug.Log(log);
        return toslide;
    }

    private IEnumerator RotatePenguins(List<Penguin> penguins)
    {
        Vector3 targetDirection;
        if (from.row == to.row)
        {
            targetDirection = to.col > from.col ? Vector3.back : Vector3.forward;
        }
        else
        {
            targetDirection = to.row > from.row ? Vector3.right : Vector3.left;
        }

        // Ensure targetDirection is calculated based on the movement direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        bool allPenguinsRotated = false;
        while (!allPenguinsRotated)
        {
            allPenguinsRotated = true; // Assume all penguins have finished rotating until checked

            foreach (Penguin penguin in penguins)
            {
                // Rotate each penguin towards the target rotation
                penguin.transform.rotation = Quaternion.RotateTowards(penguin.transform.rotation, targetRotation, trnspd * 30 * Time.deltaTime);

                // If any penguin hasn't reached the target rotation, set flag to false
                if (Quaternion.Angle(penguin.transform.rotation, targetRotation) > acceptableOffset)
                {
                    allPenguinsRotated = false;
                }
            }

            // Log rotation values for the first penguin for debugging
            if (penguins.Count > 0)
            {
                Penguin firstPenguin = penguins[0];
                //UnityEngine.Debug.Log($"Current Rotation: {firstPenguin.transform.rotation.eulerAngles}");
                //UnityEngine.Debug.Log($"Target Rotation: {targetRotation.eulerAngles}");
            }

            // Wait for the next frame before continuing the loop
            yield return null;
        }
    }
    private IEnumerator RotatePenguins(List<Penguin> penguins, Quaternion targetRotation)
    {
        bool allPenguinsRotated = false;
        while (!allPenguinsRotated)
        {
            allPenguinsRotated = true;

            foreach (Penguin penguin in penguins)
            {
                penguin.transform.rotation = Quaternion.RotateTowards(penguin.transform.rotation, targetRotation, trnspd * 30 * Time.deltaTime);

                if (Quaternion.Angle(penguin.transform.rotation, targetRotation) > acceptableOffset)
                {
                    allPenguinsRotated = false;
                }
            }

            yield return null;
        }
    }

    public IEnumerator RotatePenguin(Point lookAt)
    {
        Penguin penguin = Data(from);
        Vector3 lookAtPosition = getPos(lookAt); // Target position to look at
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - penguin.transform.position); // Calculate the rotation needed to look at the target

        // Rotate the penguin to face the target position
        while (Quaternion.Angle(penguin.transform.rotation, targetRotation) > acceptableOffset)
        {
            penguin.transform.rotation = Quaternion.RotateTowards(penguin.transform.rotation, targetRotation, trnspd * 30 * Time.deltaTime);
            yield return null; // Wait until the next frame to continue
        }
    }

    private IEnumerator PenguinWalkAround()
    {

        Point final;
        Point firstTo;
        List<Point> path = new List<Point>();

        bool AddCornerToPath()
        {
            Point last = path.Last();

            if (!isCorner(last)) { // if last is on an edge
                Point corner1, corner2;
                int index;
                if (last.row == -1 || last.row == boardSize) // Last is on a top or bottom edge
                {
                    corner1 = new Point(last.row, -1); // Left corner
                    corner2 = new Point(last.row, boardSize); // Right corner
                    index = last.col;
                }
                else // Last is on a left or right edge
                {
                    corner1 = new Point(-1, last.col); // Top corner
                    corner2 = new Point(boardSize, last.col); // Bottom corner
                    index = last.row;
                }

                // Check if final aligns with either of the corners
                if (final.row == corner1.row || final.col == corner1.col)
                {
                    // corner1 one alligns with final
                    path.Add(corner1);
                    return false;
                }
                else if (final.row == corner2.row || final.col == corner2.col)
                {
                    // corner2 alligns with final
                    path.Add(corner2);
                    return false;
                }
                else {
                    if (index >= 1 && index <= boardCenter) {
                        // corner1 is closer to last, or neither corner is closer to last
                        path.Add(corner1);
                        return true;
                    }
                    else {
                        // corner2 is closer to last
                        path.Add(corner2);
                        return true;
                    }
                }
            }
            else { // if last is on a corner
                if (from.row == to.row) {
                    path.Add(new Point(last.row, final.col));
                }
                else {
                    path.Add(new Point(final.row, last.col));
                }
                return false;
            }
        }
        // Step 1. calculate to points of walk around based on to and from points

        if (from.row == to.row) // move line is along a row
        {
            final = new Point(to.row, to.col == 0 ? -1 : boardSize);
            if (!isCorner(from)) {
                if (from.col == 0 || from.col == boardSize - 1)
                    firstTo = new Point(from.row, from.col == 0 ? -1 : boardSize);
                else
                    firstTo = new Point(from.row == 0 ? -1 : boardSize, from.col);
            }
            else {
                firstTo = new Point(from.row == 0 ? -1 : boardSize, from.col);
            }
        }
        else // move line is along a col 
        {
            final = new Point(to.row == 0 ? -1 : boardSize, to.col);
            if (!isCorner(from)) {
                if (from.col == 0 || from.col == boardSize - 1)
                    firstTo = new Point(from.row, from.col == 0 ? -1 : boardSize);
                else
                    firstTo = new Point(from.row == 0 ? -1 : boardSize, from.col);
            }
            else {
                firstTo = new Point(from.row, from.col == 0 ? -1 : boardSize);
            }
        }
        path.Add(firstTo);
        while (AddCornerToPath()) { } // Keep adding corners until no more are needed
        path.Add(final);

        Penguin penguin = Data(from);
        penguin.Play(movementAnimations[randMovementIndex]);
        penguin.soundEffect.stopAllsounds();
        penguin.soundEffect.playPitter();

        foreach (Point p in path)
        {
            yield return StartCoroutine(RotatePenguin(p));

            while (Vector3.Distance(penguin.transform.position, getPos(p)) > acceptableOffset)
            {
                penguin.step(spd, getPos(p));
                yield return null; // Wait until next frame to continue
            }
        }

        penguin.Play("Idle_A");
    }

    // Animates the move
    private IEnumerator doSlide(List<Penguin> toSlide)
    {
        // Start the walking animation for each penguin
        foreach (Penguin penguin in toSlide)
        {
            penguin.Play("Walk");
        }

        // Rotate penguins to face the direction they will move
        yield return StartCoroutine(RotatePenguins(toSlide));

        // Move penguins towards their target positions
        bool moveDone = false;
        foreach (Penguin penguin in toSlide)
        {
            if (from.row == to.row)
            {
                int c = to.col < from.col ? 1 : -1;
                penguin.toPoint = (new Point(penguin.row, penguin.col + c));
            }
            else
            {
                int r = to.row < from.row ? 1 : -1;
                penguin.toPoint = (new Point(penguin.row + r, penguin.col));
            }
        }
        while (!moveDone)
        {
            moveDone = true;

            foreach (Penguin penguin in toSlide)
            {

                penguin.step(spd);

                if (penguin.dist() >= acceptableOffset) { moveDone = false; }

                penguin.Play("Idle_A");

            }

            yield return null;
        }


        // Rotate penguins back to their original direction after moving
        Quaternion originalRotation = Quaternion.identity; // Default orientation; adjust if needed
        yield return StartCoroutine(RotatePenguins(toSlide, originalRotation));

        // Set penguins to idle animation after rotating back
        foreach (Penguin penguin in toSlide)
        {
            penguin.GetComponent<Animator>().Play("Idle_A");
        }
    }


    // Ties up any loose ends so that everything is in place for the next move
    private void finalizeMove(List<Penguin> toSlide)
    {
        Penguin[,] tempBoard = new Penguin[boardSize, boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                tempBoard[i, j] = gameBoard[i, j];
            }
        }

        foreach (Penguin penguin in toSlide)
        {
            penguin.snap();
            penguin.soundEffect.stopAllsounds();
            tempBoard[penguin.row, penguin.col] = penguin;
        }
        gameBoard = tempBoard;

        doWinCheck();
        if (isXWin && isOWin)
        {
            tieDance();
        }
        else if (isXWin)
        {
            xDance();
        }
        else if (isOWin)
        {
            oDance();
        }

        if (isXWin || isOWin)
        {
            gameOver = true;
            menuSounds.playWin();
            StartCoroutine(LoadWinScene());
        }

        from = new Point(-1, -1);
        to = new Point(-1, -1);
        poss = null;
        moveInProgress = false; 
        isXTurn = !isXTurn;
        ++turncount;
        if(isTutorial && tutorial.isGoing()) { tutorial.advanceTurn(); }
        changePlayerTurn();
        if(isOnline || AIgame || isTutorial)
        {
            if (isPlayer1 == isXTurn)
            {
                isLocked = false;
            }
            else
            {
                isLocked = true;
            }
        }
        else
        {
            isLocked = false;
        }
    }

    private void changePlayerTurn()
    {
        offerDrawButton.gameObject.SetActive(turncount > 10);
        if (offerDrawButton.gameObject != null) { offerDrawButton.interactable = ((!AIgame && !isOnline && !isTutorial) || (isPlayer1 ? isXTurn : !isXTurn)); }

        if (isOnline)
        {
            if (!(isXWin || isOWin)) { playerTurn.text = $"{(isXTurn ? p1Name : p2Name)}'s turn"; }
            else { playerTurn.text = ""; }
        }
        else
        {
            if (!(isXWin || isOWin)) { playerTurn.text = $"{(isXTurn ? EndGame.p1Name : EndGame.p2Name)}'s turn"; }
            else { playerTurn.text = ""; }
        }
    }

    IEnumerator LoadWinScene()
    {
        yield return new WaitForSeconds(3);
        if (isOnline)
        {
            SceneManager.LoadScene("OnlineWinScene");
        }
        else
        {
            SceneManager.LoadScene("WinScene");
        }
    }

    // checks various win / tie conditions to see if the game is over
    private void doWinCheck()
    {
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

        if (diagonal1 && test != '_') { gameOver = true; getWinner(gameBoard[0, 0].face); }

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

        if (diagonal2 && test != '_') { gameOver = true; getWinner(gameBoard[0, boardSize - 1].face); }

        //check rows and columns
        for (int i = 0; i < boardSize; i++)
        {
            if (checkRow(i)) { gameOver = true; getWinner(gameBoard[i, 0].face); }
            if (checkCol(i)) { gameOver = true; getWinner(gameBoard[0, i].face); }
        }

        // Check for stalemate

        // Functions to check rows and columns
        bool checkRow(int r)
        {
            char t = gameBoard[r, 0].face;
            if (t == '_') return false;
            for (int i = 0; i < boardSize; i++)
            {
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

        void getWinner(char face)
        {
            if(face == 'X')
            {
                isXWin = true;
            }
            else if (face == 'O') {
                isOWin = true;
            }
        }

    }

    // uses the other primary move functions to carry out a move in its entirety

    [PunRPC]
    public void makeMove(int FromRow, int FromCol, int ToRow, int ToCol, int tempRandMovementIndex,
                        bool AiMove = false)
    {
        randMovementIndex = tempRandMovementIndex;
        from = new Point(FromRow, FromCol);
        to = new Point(ToRow, ToCol);
        Penguin penguin = gameBoard[from.row, from.col];
        penguin.onlineSetHat();
        makeMove();
    }

    public void passMove(bool autoMove = false)
    {
        if (offerDrawButton.gameObject != null) { offerDrawButton.interactable = false; }
        //UnityEngine.Debug.Log($"Moving: ({from.row},{from.col}) >> ({to.row},{to.col}) inside passMove");
        char blockVal = isXTurn ? 'X' : 'O';
        randMovementIndex = randomMovementAnimation();

        if (isOnline)
        {
            photonView.RPC("makeMove", RpcTarget.Others, from.row, from.col, to.row, to.col, randMovementIndex,
                                        false);
        }
        makeMove(autoMove);
    }
    public void makeMove(bool autoMove = false)
    {
        //UnityEngine.Debug.Log($"Moving: ({from.row},{from.col}) >> ({to.row},{to.col}) inside makeMove");
        char blockVal = isXTurn ? 'X' : 'O';

        Penguin(from).SetActive(true);

        Penguin peng = Data(from);
        peng.Face(blockVal);

        List<Penguin> penguins = getPenguinsToSlide();
        //UnityEngine.Debug.Log($"Penguins to slide: {penguins.Count}");


        StartCoroutine(ExecuteMoveSequence(penguins, blockVal, autoMove, peng));

        
    }

    private IEnumerator ExecuteMoveSequence(List<Penguin> penguins, char blockVal, bool autoMove, Penguin peng)
    {
        // First, execute the penguin walk around sequence
        if (isTutorial)
        {
            Data(suggestedTo).setMat(defaultMat);
            Data(suggestedFrom).setMat(defaultMat);
        }

        yield return StartCoroutine(PenguinWalkAround());

        
        prepSlide();


        // Once that's complete, start the slide sequence
        yield return StartCoroutine(doSlide(penguins));

        //UnityEngine.Debug.Log($"Move complete! ({from.row},{from.col}) >> ({to.row},{to.col})");
        
        finalizeMove(penguins);
        string boardStr = translateBoard();
        blockVal = isXTurn ? 'X' : 'O';
        if (AIgame && !autoMove && !gameOver){
            StartCoroutine(aiMove());
        }
        else if (isTutorial && !autoMove)
        {
            StartCoroutine(aiMove());
        }
        else if (isTutorial && autoMove && !gameOver)
        {
            HighlightSuggestedMove();
        }
        peng.setMat(defaultMat);
    }

    private IEnumerator aiMove()
    {
        if (!isQuick) { yield return new WaitForSeconds(.5f); }
        

        string boardStr = translateBoard();

        int difficulty = CharacterCustomizationUI.AI_DIFFICULTY;
        string AImove = ai.makeMove(boardStr + (isPlayer1 ? "O" : "X") + (!isTutorial ? difficulty.ToString() : '6'));
        readAImove(AImove);

        //simulate hover
        Data(from).aiHover();

        if (!isQuick) { yield return new WaitForSeconds(1f); }

        //simulate click
        Data(from).aiClick();
        Data(from).run(true);

        if (!isQuick) { yield return new WaitForSeconds(1f); }

        Data(to).aiClick();
        Data(to).run(true);
        moveInProgress = false;

    }

    public void offerDraw()
    {
        if(!AIgame && !isOnline && !isTutorial)
        {
            isXWin = true;
            isOWin = true;
            tieDance();
            StartCoroutine(LoadWinScene());
        }
        else if(AIgame)
        {
            if((int)CharacterCustomizationUI.AI_DIFFICULTY < 3)
            {
                goodAiDraw();
            }
            else
            {
                draw();
            }
        }
        else if(isTutorial)
        {
            goodAiDraw();
        }
    }

    private void goodAiDraw()
    {
        if(turncount >= 20 && !isActiveStreak())
        {
            draw();
        } 
        else if(turncount >= 30)
        {
            draw();
        }
        else
        {
            StartCoroutine(tellRejected());
        }
    }

    private bool isActiveStreak()
    {
        char aiFace = (isPlayer1 ? 'O' : 'X');
        int maxStreak = 0;

        //check rows and cols for active streak
        for(int i = 0; i < boardSize; ++i)
        {
            int active = 0;
            for(int j = 0; j < boardSize; ++j)
            {
                if (gameBoard[i,j].face == aiFace)
                {
                    ++active;
                }
            }
            maxStreak = (active > maxStreak ?  active : maxStreak);
        }

        for(int i = 0; i < boardSize; ++i)
        {
            int activeRight = 0;
            int activeLeft = 0;
            if (gameBoard[i, i].face == aiFace)
            {
                ++activeRight;
            }
            else if (gameBoard[i, boardSize - 1 - i].face == aiFace)
            {
                ++activeLeft;
            }
            maxStreak = (activeRight > maxStreak ? activeRight : maxStreak);
            maxStreak = (activeLeft > maxStreak ? activeLeft : maxStreak);
        }

        return maxStreak >= 4;
    }

    public void draw()
    {
        isXWin = true;
        isOWin = true;
        tieDance();
        StartCoroutine(LoadWinScene());
    }

    private IEnumerator tellRejected()
    {
        rejectionMessage.SetActive(true);

        yield return new WaitForSeconds(5f);

        rejectionMessage.SetActive(false);
    }



    //####################################################################################################################################
    //# AI Interfacing Functions #########################################################################################################
    //####################################################################################################################################

    private string translateBoard(){
        string board = "";
        for (int i = 0; i < boardSize; ++i)
        {
            for (int j = 0; j < boardSize; ++j)
            {
                board += Data(i, j).face;
            }
        }
        board = board.Replace('_', ' ');
        //UnityEngine.Debug.Log("Translated Board: " + board);
        return board;

    }
    private void readAImove(string move)
    {

        Regex regex = new Regex(@"\d+");

        MatchCollection matches = regex.Matches(move);

        int row1 = int.Parse(matches[0].Value);
        int col1 = int.Parse(matches[1].Value);
        int row2 = int.Parse(matches[2].Value);
        int col2 = int.Parse(matches[3].Value);
        from = new Point(row1, col1);
        to = new Point(row2, col2);
        
    }
    private void readAImove(ref Point outTo, ref Point outFrom, char dificulty)
    {
        string boardStr = translateBoard();
        boardStr += ("X" + dificulty);
        string move = ai.makeMove(boardStr);
        Regex regex = new Regex(@"\d+");

        MatchCollection matches = regex.Matches(move);

        int row1 = int.Parse(matches[0].Value);
        int col1 = int.Parse(matches[1].Value);
        int row2 = int.Parse(matches[2].Value);
        int col2 = int.Parse(matches[3].Value);
        outFrom = new Point(row1, col1);
        outTo = new Point(row2, col2);
        //else
        //{
        //    UnityEngine.Debug.LogError("The AI output string does not match the expected format.");
        //}
    }

    //####################################################################################################################################
    //# End Game Functions ###############################################################################################################
    //####################################################################################################################################

    private void xDance()
    {
        isLocked = true;
        foreach (Penguin penguin in gameBoard)
        {
            if(penguin.face == 'X')
            {
                penguin.Play("Jump");
            }
            else
            {
                penguin.Play("Death");
            }
        }
    }

    private void oDance()
    {
        isLocked = true;

        foreach (Penguin penguin in gameBoard)
        {
            if (penguin.face == 'O')
            {
                penguin.Play("Jump");
            }
            else
            {
                penguin.Play("Death");
            }
        }
    }

    private void tieDance()
    {
        isLocked = true;

        foreach (Penguin penguin in gameBoard)
        {
            if (penguin.face != '_')
            {
                penguin.Play("Fear");
            }
            else
            {
                penguin.Play("Death");
            }
        }
    }

    private int randomMovementAnimation()
    {
        System.Random random = new System.Random();

        int randomNumber = random.Next(0, movementAnimations.Length);
        return randomNumber;

    }

    //####################################################################################################################################
    //# Tutorial Functions ###############################################################################################################
    //####################################################################################################################################

    public void tutorialPieceClick()
    {
        tutorial.wasClicked();
    }

    public void resetMat(Point p)
    {
        Data(p).setMat(defaultMat);
    }

    void HighlightSuggestedMove()
    {
        readAImove(ref suggestedTo, ref suggestedFrom, '0');
        Data(suggestedFrom).setMat(highlightMat);
        pieceToClick = Data(suggestedFrom).loc() ;
        StartCoroutine(readyForTo());
    }

    private IEnumerator readyForTo()
    {
        while (!tutorial.readyForTo())
        {
            yield return new WaitForSeconds(.3f);
        }

        Data(suggestedTo).setMat(selectedMat);
        pieceToClick = Data(suggestedTo).loc();

    }
}