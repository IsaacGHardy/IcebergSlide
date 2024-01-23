using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuixoClass : MonoBehaviour
{
    public int boardSize = 5;
    public GameObject[,] gameBoard;
    public bool activeTurn; 

    private bool p1 = true; // set activeTurn to this to indicate that it is player 1's turn
    private bool p2 = false; // set activeTurn to this to indicate that it is player 2's turn

    // Start is called before the first frame update
    void Start()
    {
        gameBoard = new GameObject[boardSize, boardSize];
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
