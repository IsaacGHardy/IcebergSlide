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
        UnityEngine.Debug.Log($"Selected block ({row},{col})");
        List<Point> moves = new List<Point>();
        if (Game.canPickPiece(row, col))
        {
            if (!Game.moveInProgress)
            {
                cube.SetActive(false);
                Game.moveInProgress = true;
                Game.FROM = cube;
                Game.f = loc();
            }
            else
            {
                Game.t = loc();
                if (Game.IsValidMove(Game.f, Game.t))
                {
                    Game.moveInProgress = false;
                    Game.makeMove(Game.f, Game.t);

                    UnityEngine.Debug.Log($"Move complete! ({Game.f.row},{Game.f.col}) >> ({row},{col})");
                }
            }
        }       
        
    }

    public Point loc() { return new Point(row, col); }

}
