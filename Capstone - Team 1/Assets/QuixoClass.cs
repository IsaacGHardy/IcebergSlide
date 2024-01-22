using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuixoClass : MonoBehaviour
{
    public int boardSize = 5;
    public GameObject[,] gameBoard;
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
