using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

// source for clicking ability: https://www.youtube.com/watch?v=kkkmX3_fvfQ&ab_channel=Andrew


public class QuixoCube : MonoBehaviour, IPointerClickHandler
{
    public GameObject cube;
    public QuixoClass Game;
    public int row = 0;
    public int col = 0;
    public char face = '_';
    private Point from, to;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        List<Point> moves = new List<Point>();
        if (Game.canPickPiece(row, col))
        {
            if (!Game.moveInProgress)
            {
                cube.SetActive(false);
                from = loc();
                Game.moveInProgress = true;
                moves = Game.GetPossibleMoves(row, col);
            }
            else
            {
                if (Game.IsValidMove(moves, row, col))
                {
                    cube.SetActive(false);
                    Game.moveInProgress = false;
                    to = loc();
                    Game.makeMove(from, to);
                }
            }
        }       
        
    }

    public Point loc() { return new Point(row, col); }

}
